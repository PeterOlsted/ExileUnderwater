using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(AudioSource))]
public class HDRAudioSource : MonoBehaviour {
    internal AudioSource attachedTo;
    //public LoudnessSource LoudenessSetting;

    [HideInInspector]
    public HDRController Manager;

    [Range(0,130)]
    public float Decibel = 60f;

    [Range(0, 1)]
    public float Volume = 1.0f;

    public bool ContributeToEnviroment = true;

    [CustomFalloffAttribute]
    public AnimationCurve CustomFalloffCurve = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0));

    // Use this for initialization
	void Start () {
        attachedTo = GetComponent<AudioSource>();
        Manager = HDRController.AudioManager; 

        if (Manager != null)
        {
            Manager.AddAudioSource(this);
        }
        else
        {
            Debug.LogError("HDR Audio Manager not found : " + name + " a " + transform.position);
        }
        
	}

    void OnDestroy()
    {
        Manager.RemoveAudioSource(this);
    }
}