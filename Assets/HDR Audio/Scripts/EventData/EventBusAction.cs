public class EventBusAction : AudioEventAction
{
    public AudioBus Bus;
    public float Volume = 1.0f;
    public VolumeSetMode VolumeMode = VolumeSetMode.Absolute;

    public enum VolumeSetMode
    {
        Relative, Absolute
    }

    public override string ObjectName
    {
        get
        {
            if (Bus != null)
                return Bus.GetName;
            else
            {
                return "Missing Bus";
            }
        }
    }
}
