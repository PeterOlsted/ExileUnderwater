using HDRAudio.ExtensionMethods;
using UnityEngine;

public static class AudioBusVolumeHelper {
    public static void SetTargetVolume(AudioBus bus, float volume, EventBusAction.VolumeSetMode setMode)
    {
        if (setMode == EventBusAction.VolumeSetMode.Absolute)
        {
            bus.RuntimeTargetVolume = volume;
            bus.Dirty = true;
        }
        else
        {
            bus.RuntimeTargetVolume = Mathf.Clamp(bus.RuntimeTargetVolume + volume, 0.0f, 1.0f);
            bus.Dirty = true;
        }
    }

    public static void UpdateDirtyBusses(AudioBus bus)
    {
        if (bus.Dirty)
        {
            bus.RuntimeVolume = bus.RuntimeTargetVolume*bus.CombinedVolume;
            var nodes = bus.GetRuntimePlayers();
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i] != null)
                    nodes[i].UpdateBusVolume(bus.RuntimeVolume);
                else
                {
                    nodes.SwapRemoveAt(i);
                }
            }
        }
        for (int i = 0; i < bus.Children.Count; ++i)
        {
            if (bus.Dirty)
                bus.Children[i].Dirty = true;
            UpdateDirtyBusses(bus.Children[i]);
        }  
    }

    public static void SetBusVolumes(AudioBus bus)
    {
        SetBusVolumes(bus, 1.0f);
    }

    private static void SetBusVolumes(AudioBus bus, float volume)
    {
        float newVolume = bus.Volume * volume;
        if(newVolume != bus.Volume)
        {
            
            bus.Dirty = true; //Non serialized, so will only stick while playing, will then get updated by the runtime system
        }
        bus.CombinedVolume = newVolume;

        for (int i = 0; i < bus.Children.Count; i++)
        {
            SetBusVolumes(bus.Children[i], bus.CombinedVolume);
        }
    }
}
