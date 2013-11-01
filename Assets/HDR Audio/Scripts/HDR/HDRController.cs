using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;

public class HDRController : MonoBehaviour
{
    public static HDRController AudioManager
    {
        get;
        private set;
    }

    #region Public settings

    public bool UseHDRAudio = true;

    public HDRAudio.ScalingType ScaleInRelationTo = HDRAudio.ScalingType.EnviromentLoudness;

    public AudioListener ActiveListener;

    [Range(0, 1)]
    public float Volume = 1.0f;

    [Range(0, 130)]
    public float ActivationThreshold = 60.0f;


    [Range(0, 130)]
    public float HDRWindow = 50.0f;

    public float DBReactionSpeed = 200.0f;

    //Should the audio be updated in the fixed or normal update loop?
    [HideInInspector]
    public AudioVelocityUpdateMode UpdateIn = AudioVelocityUpdateMode.Fixed;

    #endregion
    
    #region Public methods

    /// <summary>
    /// If you want to do manipulate the list directly, be careful though
    /// </summary>
    /// <returns></returns>
    public List<HDRAudio.AudioTuple> RawAudioSourceList()
    {
        return AudioSources;
    }

    //Add a HDR audio source to the collection, don't add the same twice
    public void AddAudioSource(HDRAudioSource source)
    {
        AudioSources.Add(new HDRAudio.AudioTuple(source));
    }

    //Remove a HDR audio source to the collection
    public void RemoveAudioSource(HDRAudioSource source)
    {
        int index = AudioSources.FindIndex(p => p.Audio == source);
        if (index != -1)
        {
            AudioSources.RemoveAt(index);
        }
    }

    #endregion

    #region Private variables

    private List<HDRAudio.AudioTuple> AudioSources = new List<HDRAudio.AudioTuple>();

    private const float DecibelEpsilon = 2.0f;

    #endregion

    #region Initialization
    void Awake () {
        AudioManager = this;
	}

    void Start()
    {
        //Find the active listener if no is already set
        if (ActiveListener == null)
        {
            var allListeners = FindObjectsOfType(typeof(AudioListener));
            foreach (var listener in allListeners)
            {
                if (((AudioListener)listener).enabled == true)
                {
                    ActiveListener = (AudioListener)listener;
                }
            }
        }

        var allSources = FindObjectsOfType(typeof(HDRAudioSource));
        foreach (var source in allSources)
        {
            AudioSources.Add(new HDRAudio.AudioTuple(source as HDRAudioSource));
        }

        if(ActiveListener == null)
            Debug.LogError("No audio listener found in the scene");
       
    }
    #endregion

    #region Working area

    private void UpdateLoudness()
    {
        if (AudioSources.Count <= 0 )
            return;
        
        if (!UseHDRAudio)
        {
            //Set the volume to what the HDR audio source volume is
            AudioSources.ForEach(p => p.Audio.attachedTo.volume = p.Audio.Volume);
            return;
        }

        HDRAudio.AudioTuple loudestSource;

        //How loud is this environment for the listener? 
        float loudnessSum = UpdateHDRLoudness(out loudestSource);

        //http://en.wikipedia.org/wiki/Decibel
        //Scale power ratio into dB
        float envLoudness = 10 * Mathf.Log10(loudnessSum);

        //Use minimum this HDR window to keep it from going too much up and down
        //envLoudness = Mathf.Max(envLoudness, WindowMinimumTop);


        if (ScaleInRelationTo == HDRAudio.ScalingType.EnviromentLoudness)
            ScaleToLoudness(envLoudness, Volume);
        else
            ScaleToLoudness(loudestSource.DynamicLoudness, Volume);
        
    }

    private void ScaleToLoudness( float scaleTo, float managerVolume )
    {
	    //Update the volume of the sources
        for(int i = 0; i < AudioSources.Count; ++i)
        {
            if (scaleTo - AudioSources[i].DynamicLoudness < HDRWindow && AudioSources[i].DynamicLoudness > DecibelEpsilon)
            {
                //Calc the sources volume based on its relative loudness to the environment loudness
                AudioSources[i].Audio.attachedTo.volume = Mathf.Pow(10, (AudioSources[i].DynamicLoudness - scaleTo) / 20) * AudioSources[i].Audio.Volume * managerVolume;
            }
            else
                AudioSources[i].Audio.attachedTo.volume = 0.0f;
        }
    }

    //http://en.wikipedia.org/wiki/Decibel Returns the power ratio sum and finds the loudest source
    private float UpdateHDRLoudness(out HDRAudio.AudioTuple loudestSource)
    {
        float loudnessSum = 0.0f;
        Vector3 listenerPos = ActiveListener.transform.position;

        loudestSource = AudioSources[0];
        for (int i = 0; i < AudioSources.Count; ++i)
        {
            HDRAudioSource current = AudioSources[i].Audio;
            var audio = current.attachedTo;

            AudioSources[i].ActualLoudness = 0;

            if (IsSourceActive(current))
            {
                float listenerDistance = Vector3.Distance(current.transform.position, listenerPos);

                float distanceBasedVolume = 0.0f;

                if (listenerDistance > audio.maxDistance)
                {
                    distanceBasedVolume = 0.0f;
                }
                else if (audio.rolloffMode == AudioRolloffMode.Logarithmic)
                {
                    //volume = min distance / distance
                    //Clamp volume to max 1
                    distanceBasedVolume = Mathf.Min(audio.minDistance / listenerDistance, 1);
                }
                else if (audio.rolloffMode == AudioRolloffMode.Linear)
                {
                    //volume = linear falloff between min and max distance, 1 to 0
                    distanceBasedVolume = Mathf.Clamp(1 - (listenerDistance - audio.minDistance) / (audio.maxDistance - audio.minDistance), 0, 1);
                }
                else if (audio.rolloffMode == AudioRolloffMode.Custom)
                {
                    distanceBasedVolume = current.CustomFalloffCurve.Evaluate(listenerDistance / audio.maxDistance);
                }

                AudioSources[i].ActualLoudness = distanceBasedVolume * current.Decibel;
            }

            float target = AudioSources[i].ActualLoudness;
            float from = AudioSources[i].DynamicLoudness;

            //Make sure it the volume doesn't transition too fast
            AudioSources[i].DynamicLoudness = LerpClamp(from, target, Time.deltaTime, DBReactionSpeed);

            if (AudioSources[i].DynamicLoudness > loudestSource.DynamicLoudness)
                loudestSource = AudioSources[i];

            //As x^y, where y < 1 && y >= 0, the result would be one, we don't want inaudible sources to contribute loudness
            if (current.ContributeToEnviroment && AudioSources[i].DynamicLoudness > 0.0f)
            {
                loudnessSum += Mathf.Pow(10, AudioSources[i].DynamicLoudness/ 10.0f);
            }
        }
        return loudnessSum;
    }

    #endregion

    #region Unity update functions
    void Update()
    {
        //Where should we update the audio?
        if (UpdateIn == AudioVelocityUpdateMode.Dynamic)
            UpdateLoudness();
    }

    void FixedUpdate()
    {
        if (UpdateIn == AudioVelocityUpdateMode.Fixed || UpdateIn == AudioVelocityUpdateMode.Auto)
            UpdateLoudness();
    }

    #endregion

    #region Auxiliary functions

    private bool IsSourceActive(HDRAudioSource audio)
    {
        return audio.enabled && audio.attachedTo.isPlaying && !audio.attachedTo.mute;
    }

    //Lerps to target, but max x amounts, time is also deltatime, not a target as the normal lerp
    private float LerpClamp(float from, float to, float dTime, float maxDeltaPerSecond)
    {
        float d = Mathf.Abs(to - from) / maxDeltaPerSecond;
        return Mathf.Lerp(from, to, dTime / d);
    }

    #endregion

    #region Cleanup

    void OnDestroy()
    {
        AudioSources.Clear();
    }

    #endregion
}

namespace HDRAudio
{
    //Should the audio be scales to the environment loudness or to the loudest object?
    public enum ScalingType
    {
        LoudestSource,
        EnviromentLoudness
    }
}