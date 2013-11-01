using UnityEngine;

public class BankHookAttribute : PropertyAttribute
{
    public string EventType;
    public BankHookAttribute(string eventType)
    {
        EventType = eventType;
    }

}
