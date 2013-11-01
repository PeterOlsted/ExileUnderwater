using System.Collections.Generic;
using HDRAudio;
using UnityEngine;
using System.Collections;

namespace HDRAudio
{

    public enum AudioBankTypes
    {
        Folder, Link
   }
}

public class AudioBankLink : MonoBehaviour, ITreeNode<AudioBankLink>
{
    public int GUID;

    public AudioBankTypes Type;

    public bool FoldedOut;

    public bool Filtered = false;

    public string Name;

    public AudioBankLink Parent;

    public List<AudioBankLink> Children = new List<AudioBankLink>();

    public bool AutoLoad = false;

    [System.NonSerialized]
    public AudioBank LoadedBank;

    public AudioBank LazyBankFetch
    {
        get
        {
            if (LoadedBank == null)
            {
                LoadedBank = BankLoader.Load(this);
            }

            return LoadedBank;
        }
    }

    public AudioBankLink GetParent
    {
        get
        {
            return Parent;
        }
        set
        {
            Parent = value;
        }
    }

    public List<AudioBankLink> GetChildren
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
        get { return Parent == null; }
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

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }
}
