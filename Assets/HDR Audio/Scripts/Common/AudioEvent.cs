using HDRAudio;
using UnityEngine;
using System.Collections.Generic;

public class AudioEvent : MonoBehaviour, ITreeNode<AudioEvent>
{
    public int GUID;

    public EventNodeType Type;

    public bool FoldedOut;

    public string Name;

    public AudioEvent Parent;

    public float Delay;

    public bool Filtered;

    public List<AudioEvent> Children = new List<AudioEvent>();

    public List<AudioEventAction> ActionList = new List<AudioEventAction>();

    public void AssignParent(AudioEvent node)
    {
        node.Children.Add(this);
        Parent = node;
    }

    public AudioEvent GetParent
    {
        get { return Parent; }
        set { Parent = value; }
    }

    public List<AudioEvent> GetChildren
    {
        get { return Children; }
    }

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

    public string GetName
    {
        get { return Name; }
    }

    public bool IsRoot
    {
        get { return Type == EventNodeType.Root; }
    }

    public bool IsFiltered
    {
        get { return Filtered; }
        set { Filtered = value; }
    }

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }
}

namespace HDRAudio
{
    public enum EventNodeType
    {
        Root,
        Folder,
        EventGroup,
        Event
    }
}
