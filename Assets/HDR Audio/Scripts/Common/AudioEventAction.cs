using System;
using UnityEngine;

namespace HDRAudio
{
    public enum EventActionTypes
    {
        Play            = 1,
        Stop            = 2,
        Break           = 3, //Not implemented
        //Pause = 4, //Not implemented
        StopAll         = 5,
        //SetVolume     = 6,
        SetBusVolume    = 7,
        LoadBank        = 8,
        UnloadBank      = 9,
        StopAllInBus    = 10
    };
}

public abstract class AudioEventAction : MonoBehaviour
{
    public float Delay;
    public HDRAudio.EventActionTypes EventActionType;

    public abstract string ObjectName { get; }
}