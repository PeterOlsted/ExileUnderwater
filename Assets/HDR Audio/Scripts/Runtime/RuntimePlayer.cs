using System;
using HDRAudio;
using HDRAudio.ExtensionMethods;
using HDRAudio.RuntimeHelperClass;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RuntimePlayer : MonoBehaviour
{
    public void Play(AudioNode node)
    {
        currentIndex = 0;
        attachedToBus = RuntimeHelper.GetBus(node);
        busVolume = attachedToBus.RuntimeVolume;

        //This is to queue the next playing node, as the first clip will not yield a waitforseconds
        firstClip = true;

        //Play this node 40 ms(depending on framerate) in the future
        StartCoroutine(StartPlay(node, node, new DSPTime(AudioSettings.dspTime + 0.50)));
    }

    public void Break()
    {
        breakLoop = true;
    }

    public void Stop()
    {
        for (int i = 0; i < audioSources.Length; ++i)
        {
            audioSources[i].SetScheduledEndTime(AudioSettings.dspTime - 2); //Set to end in the past
        }
        StopAllCoroutines();
    }

    public void UpdateBusVolume(float newVolume)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources == null)
                continue;
            audioSources[i].volume = originalVolume[i] * newVolume;
        }
    }

    public void Initialize(AudioGOPool spawnedFrom)
    {
        this.spawnedFrom = spawnedFrom;
        if (audioSources == null)
            audioSources = GetComponents<AudioSource>();
        if (endTimes == null)
            endTimes = new double[audioSources.Length];
        if(originalVolume == null)
            originalVolume = new float[audioSources.Length];
    }


    private AudioSource[] audioSources;
    private double[] endTimes;
    private float[] originalVolume;

    private bool firstClip = true;

    private int currentIndex = 0;

    private AudioGOPool spawnedFrom;

    private AudioNode beloningToNode;

    private float busVolume;

    private AudioBus attachedToBus;

    private bool breakLoop;

    private AudioSource Current
    {
        get
        {
            return audioSources[currentIndex];
        }
    }

    private IEnumerator StartPlay(AudioNode root, AudioNode current, DSPTime endTime)
    {
        breakLoop = false;
        beloningToNode = current;
        current.Bus.GetRuntimePlayers().Add(this);

        yield return StartCoroutine(FindNext(root, current, endTime));
        //Debug.Log(endTime.CurrentEndTime + " - " + AudioSettings.dspTime + " = " + (endTime.CurrentEndTime - AudioSettings.dspTime));
        yield return new WaitForSeconds((float)(endTime.CurrentEndTime - AudioSettings.dspTime));
        //Clean up object
        StopAndCleanup();
    }

    private IEnumerator FindNext(AudioNode root, AudioNode current, DSPTime endTime)
    {
        byte loops = 0;
        var nodeData = current.NodeData;
        bool loopInfinite = nodeData.LoopInfinite;
        if(!loopInfinite)
            loops = RuntimeHelper.GetLoops(current);

        endTime.CurrentEndTime += RuntimeHelper.InitialDelay(nodeData);

        if (nodeData.Loop == false)
        {
            loops = 0;
            loopInfinite = false;
        }
        for (int i = 0; i < 1 + loops || loopInfinite; ++i) //For at least once
        {
            if (breakLoop)
            {
                loops = 0;
                loopInfinite = false;
            }

            if (current.Type == AudioNodeType.Audio)
            {
                NextFreeAudioSource();
                float nodeVolume;
                
                float length = PlayScheduled(root, current, endTime.CurrentEndTime, out nodeVolume);
                
                originalVolume[currentIndex] = nodeVolume;
                
                endTime.CurrentEndTime += length;

                if (!firstClip)
                    yield return new WaitForSeconds((float)(endTime.CurrentEndTime - AudioSettings.dspTime) - length + 0.050f);

                firstClip = false;
            }
            else if (current.Type == AudioNodeType.Random)
            {
                yield return StartCoroutine(FindNext(root, RuntimeHelper.SelectRandom(current), endTime));
            }
            else if (current.Type == AudioNodeType.Sequence)
            {
                for (int j = 0; j < current.Children.Count; ++j)
                {
                    yield return StartCoroutine(FindNext(root, current.Children[j], endTime));
                }
            }
            else if (current.Type == AudioNodeType.Multi)
            {
                Coroutine[] toStart = new Coroutine[current.Children.Count];
                DSPTime[] childTimes = new DSPTime[current.Children.Count];

                for (int j = 0; j < childTimes.Length; ++j)
                {
                    childTimes[j] = new DSPTime(endTime.CurrentEndTime);
                }
                for (int j = 0; j < current.Children.Count; ++j)
                {
                    toStart[j] = StartCoroutine(FindNext(root, current.Children[j], childTimes[j]));
                }
                for (int j = 0; j < childTimes.Length; ++j)
                {
                    if (endTime.CurrentEndTime < childTimes[j].CurrentEndTime)
                        endTime.CurrentEndTime = childTimes[j].CurrentEndTime;
                }
            }
        }
    }

    private float PlayScheduled(AudioNode startNode, AudioNode currentNode, double playAtDSPTime, out float volume)
    {
        var audioData = currentNode.NodeData as AudioData;
        float length = 0;
        volume = 1;
        if (audioData.Clip != null)
        {
            length = audioData.Clip.samples/(float) audioData.Clip.frequency;

            Current.clip = audioData.Clip;
            Current.volume = RuntimeHelper.ApplyVolume(startNode, currentNode);
            volume = Current.volume;
            Current.volume *= busVolume;

            Current.pitch = RuntimeHelper.ApplyPitch(startNode, currentNode);
            RuntimeHelper.ApplyAttentuation(startNode, currentNode, Current);

            length = RuntimeHelper.LengthFromPitch(length, Current.pitch);
            endTimes[currentIndex] = playAtDSPTime + length;


            Current.PlayScheduled(playAtDSPTime);

        }
        
        return length;
    }

    private void NextFreeAudioSource()
    {
        double dspTime = AudioSettings.dspTime;

        if (endTimes == null)
        {
            Initialize(spawnedFrom); 
        }
         
        for (int i = 0; i < audioSources.Length; ++i)
        {
            if (endTimes[i] < dspTime)
            { 
                currentIndex = i;
                return;
            }

        }
        Array.Resize(ref endTimes, endTimes.Length + 1);
        Array.Resize(ref originalVolume, originalVolume.Length + 1);
        
        
        currentIndex = audioSources.Length;
        gameObject.AddComponent<AudioSource>();
        audioSources = GetComponents<AudioSource>();
    }

    private void StopAndCleanup()
    {
        Stop();
        spawnedFrom.ReleaseObject(gameObject);
        beloningToNode.Bus.GetRuntimePlayers().Remove(this);
        beloningToNode = null;
    }
}

namespace HDRAudio.RuntimeHelperClass
{
    internal class DSPTime
    {
        public double CurrentEndTime;

        public DSPTime(double currentEndTime)
        {
            CurrentEndTime = currentEndTime;
        }
        public DSPTime(DSPTime time)
        {
            CurrentEndTime = time.CurrentEndTime;
        }
    }
}
