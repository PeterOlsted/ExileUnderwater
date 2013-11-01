using System;
using UnityEngine;
using System.Collections;

public class AudioData : NodeTypeData {

#if UNITY_EDITOR
    public AudioClip EditorClip;
#endif
#if !UNITY_EDITOR
    [NonSerialized]
    public AudioClip RuntimeClip;
#endif

    public AudioClip Clip
    {
        get
        {
            #if UNITY_EDITOR
                return EditorClip;
            #endif
            #if !UNITY_EDITOR
                return RuntimeClip;
            #endif
        }

        set
        {
            #if UNITY_EDITOR
                EditorClip = value;
            #endif
            #if !UNITY_EDITOR
                RuntimeClip = value;
            #endif
        }
    }
}
