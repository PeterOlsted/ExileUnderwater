using UnityEngine;
using System.Collections.Generic;
using HDRAudio;

public class AudioNode : MonoBehaviour, ITreeNode<AudioNode>
{
    public int GUID;

    public AudioNodeType Type;

    public NodeTypeData NodeData;
  
    #if UNITY_EDITOR
    public bool Filtered = false;

    public bool FoldedOut;

    #endif
    //If we loose the connection, we can rebuild 
    public int ParentGUID;


    public string Name;

    public AudioNode Parent;

    public bool OverrideParentBus;
    public AudioBus Bus;

    public bool OverrideParentBank;
    public AudioBankLink BankLink;

    public List<AudioNode> Children = new List<AudioNode>();

    public AudioNode GetParent
    {
        get { return Parent; }
        set { Parent = value; }
    }

    public List<AudioNode> GetChildren
    {
        get { return Children; }
    }

    public string GetName
    {
        get { return Name; }
    }

    public bool IsRoot
    {
        get { return Type == AudioNodeType.Root; }
    }

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }

    public bool IsPlayable
    {
        get { return Type != AudioNodeType.Root && Type != AudioNodeType.Folder; }
    }

#if UNITY_EDITOR
    public bool IsFoldedOut
    {
        get { return FoldedOut; }
        set { FoldedOut = value; }
    }

    public bool IsFiltered
    {
        get { return Filtered; }
        set { Filtered = value; }
    }
#endif
}

namespace HDRAudio
{
    public enum AudioNodeType
    {
        Root,
        Folder,
        Audio,
        Random,
        Sequence, 
        Voice,
        Multi,
        Track
    }
}
