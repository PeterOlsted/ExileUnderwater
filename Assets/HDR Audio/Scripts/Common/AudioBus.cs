using System.Collections.Generic;
using UnityEngine;

public class AudioBus : MonoBehaviour, ITreeNode<AudioBus>
{
    //Volume set in the editor
    public float Volume = 1.0f;

    //The volume in the hiarchy
    public float CombinedVolume = 1.0f;

    public int GUID;

    public string Name;

    //Do we need to update the attach audio players?
    [System.NonSerialized]
    public bool Dirty;

    //The nodes during runtime that is in this bus
    [System.NonSerialized]
    public List<RuntimePlayer> NodesInBus = new List<RuntimePlayer>();

    //Current volume
    [System.NonSerialized]
    public float RuntimeVolume = 1.0f;


    //What the volume should be
    [System.NonSerialized]
    public float RuntimeTargetVolume = 1.0f;

    public List<RuntimePlayer> GetRuntimePlayers()
    {
        if (NodesInBus == null)
            NodesInBus = new List<RuntimePlayer>();
        return NodesInBus;
    }

    public AudioBus Parent;

    //If we loose the connection, we can rebuild
    public int ParentGUID;

    public List<AudioBus> Children = new List<AudioBus>();

#if UNITY_EDITOR
    public bool FoldedOut;

    public bool Filtered = false;
#endif

    public void AssignParent(AudioBus node)
    {
        node.Children.Add(this);
        Parent = node;
        ParentGUID = node.GUID;
    }

    public void RemoveFromParent()
    {
        Parent.Children.Remove(this);
    }

    public AudioBus GetParent
    {
        get { return Parent; }
        set { Parent = value; }
    }

    public List<AudioBus> GetChildren
    {
        get { return Children; }
    }


    public string GetName
    {
        get { return Name; }
    }

    public bool IsRoot
    {
        get { return Parent == null; }
    }

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }

    
    #if UNITY_EDITOR
    public bool IsFoldedOut
    {
        get
        {
            return FoldedOut;
        }
        set
        {
            FoldedOut = value;
        }
    }

    public bool IsFiltered
    {
        get
        {
            return Filtered;
        }
        set
        {
            Filtered = value;
        }
    }
    #endif

}
