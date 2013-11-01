using UnityEngine;

public class EventAudioAction : AudioEventAction
{
    public AudioNode Node;

    public override string ObjectName
    {
        get
        {
            if (Node != null)
                return Node.GetName;
            else
            {
                return "Missing Audio";
            }

        }
    }
}
