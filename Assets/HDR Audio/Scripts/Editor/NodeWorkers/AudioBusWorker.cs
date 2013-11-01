using System.Collections.Generic;
using UnityEngine;

namespace HDRAudio
{

public static class AudioBusWorker
{
    private static AudioBus CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<AudioBus>();
        node.GUID = guid;
        node.FoldedOut = true;
        node.Name = "Root";
        return node;
    }

    public static void RemoveAudioNode(AudioEvent audioevent, int index)
    {
        audioevent.ActionList.RemoveAt(index);
    }

    private static AudioBus CreateBus(GameObject go, AudioBus parent, int guid)
    {
        var node = go.AddComponent<AudioBus>();
        node.GUID = guid;
        node.AssignParent(parent);
        return node;
    }

    public static AudioBus CreateTree(GameObject go)
    {
        var tree = CreateRoot(go, GUIDCreator.Create());
        return tree;
    }

    public static void DeleteBus(AudioBus bus, AudioNode root)
    {
        HashSet<AudioBus> toDelete = new HashSet<AudioBus>();
        GetBussesToDelete(toDelete, bus);

        List<AudioNode> affectedNodes = new List<AudioNode>();
        NodeWorker.FindAllNodes(root, node => toDelete.Contains(node.Bus), affectedNodes);

        for (int i = 0; i < affectedNodes.Count; ++i)
        {
            affectedNodes[i].Bus = bus.Parent;
        }
        
        ActualDelete(bus);
    }

    private static void ActualDelete(AudioBus bus)
    {
        for (int i = 0; i < bus.Children.Count; ++i)
        {
            ActualDelete(bus.Children[i]);
        }
        bus.Parent.Children.Remove(bus);
        Object.DestroyImmediate(bus, true);
    }

    private static void GetBussesToDelete(HashSet<AudioBus> toDelete, AudioBus bus)
    {
        toDelete.Add(bus);
        for (int i = 0; i < bus.Children.Count; ++i)
        {
            GetBussesToDelete(toDelete, bus.Children[i]);
        }
    }

    public static AudioBus CreateNode(AudioBus parent)
    {
        var child = CreateBus(parent.gameObject, parent, GUIDCreator.Create());
        child.FoldedOut = true;
        child.Name = "Name";

        return child;
    }
}
}
