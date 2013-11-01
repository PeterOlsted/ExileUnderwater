using UnityEngine;

public class EventHookAttribute : PropertyAttribute
{
    public string EventType;    
    public EventHookAttribute(string eventType)
    {
        EventType = eventType;
    }

}

