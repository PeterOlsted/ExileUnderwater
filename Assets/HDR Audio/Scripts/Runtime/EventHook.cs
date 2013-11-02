using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[AddComponentMenu("HDR Audio/Event Hook")]
public class EventHook : MonoBehaviour
{
    [EventHookAttribute("On Enable")]
    public List<AudioEvent> OnEnableEvents = new List<AudioEvent>();
    [EventHookAttribute("On Start")]
    public List<AudioEvent> OnStartEvents = new List<AudioEvent>();
    [EventHookAttribute("On Disable")]
    public List<AudioEvent> OnDisableEvents = new List<AudioEvent>();
    [EventHookAttribute("On Destroy")]
    public List<AudioEvent> OnDestroyEvents = new List<AudioEvent>();
    [EventHookAttribute("On Visible")]
    public List<AudioEvent> OnVisibleEvents = new List<AudioEvent>();
    [EventHookAttribute("On Invisible")]
    public List<AudioEvent> OnInvisibleEvents = new List<AudioEvent>();

    void OnEnable()
    {
        for (int i = 0; i < OnEnableEvents.Count; ++i)
        {
            HDRSystem.PostEvent(gameObject, OnEnableEvents[i]);
        }
    }

    void Start()
    {
        for (int i = 0; i < OnStartEvents.Count; ++i)
        {
            HDRSystem.PostEvent(gameObject, OnStartEvents[i]);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < OnDisableEvents.Count; ++i)
        {
            HDRSystem.PostEvent(gameObject, OnDisableEvents[i]);
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < OnDestroyEvents.Count; ++i)
        {
            HDRSystem.PostEvent(gameObject, OnDestroyEvents[i]);
        }
    }

    void OnBecameVisible()
    {
        for (int i = 0; i < OnVisibleEvents.Count; ++i)
        {
            HDRSystem.PostEvent(gameObject, OnVisibleEvents[i]);
        }
    }

    void OnBecameInvisible()
    {
        for (int i = 0; i < OnInvisibleEvents.Count; ++i)
        {
            HDRSystem.PostEvent(gameObject, OnInvisibleEvents[i]);
        }
    }
}
