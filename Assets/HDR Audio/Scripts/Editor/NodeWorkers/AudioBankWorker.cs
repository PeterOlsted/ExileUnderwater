using System.Collections.Generic;
using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace HDRAudio
{
 
public static class AudioBankWorker {
    private static AudioBankLink CreateNode(GameObject go, AudioBankLink parent, int guid)
    {
        var node = go.AddComponent<AudioBankLink>();
        node.GUID = guid;
        node.Parent = parent;
        node.IsFoldedOut = true;
        node.Name = "Name";
        NodeWorker.AssignParent(node, parent);
        return node;
    }

    private static AudioBankLink CreateRoot(GameObject go, int guid)
    {
        var node = CreateNode(go, null, guid);
        node.Name = "Root";
        node.Type = AudioBankTypes.Folder;
        return node;
    }
 
    public static AudioBankLink CreateFolder(GameObject go, AudioBankLink parent, int guid)
    {
        var node = CreateNode(go, parent, guid);
        node.Name = "folder";
        node.Type = AudioBankTypes.Folder;
        return node;
    }

    private static AudioBankLink CreateBankLink(GameObject go, AudioBankLink parent, int guid)
    {
        var node = CreateNode(go, parent, guid);
        node.Name = "BankLink";
        node.Type = AudioBankTypes.Link;
        return node;
    }

    public static AudioBankLink CreateBank(GameObject go, AudioBankLink parent, int guid)
    {
        AudioBankLink link = CreateBankLink(go, parent, guid);

        SaveAndLoad.CreateAudioBank(guid);
        return link;
    }

    public static AudioBankLink CreateTree(GameObject go)
    {
        var root = CreateRoot(go, GUIDCreator.Create());
        return root;
    }

    public static void AddSingleNodeToBank(AudioNode node, AudioClip clip)
    {
        AddNodeToBank(node, clip);
    }

    public static bool SwapClipInBank(AudioNode node, AudioClip newClip)
    {
        var bank = GetParentBank(node);
        
        var clipTuple = bank.LazyBankFetch.Clips;

        for (int i = 0; i < clipTuple.Count; i++)
        {
            if (clipTuple[i].Node == node)
            {
                
                clipTuple[i].Clip = newClip;

                return true;
            }
        }
        return false;
    }

    public static void AddNodeToBank(AudioNode node, AudioClip clip)
    {
        var bank = GetParentBank(node).LazyBankFetch;
        bank.Clips.Add(CreateTuple(node, clip));
    }

    public static void RemoveNodeFromBank(AudioNode node)
    {
        var bank = GetParentBank(node).LazyBankFetch;
        bank.Clips.RemoveAll(p => p.Node == node);
    }

    private static BankTuple CreateTuple(AudioNode node, AudioClip clip)
    {
        BankTuple tuple = new BankTuple();
        tuple.Node = node;
        tuple.Clip = clip;
        return tuple;
    }

    public static void ChangeBankOverriding(AudioNode node)
    {
        
        AudioBankLink newBank;
        AudioBankLink currentBank = GetParentBank(node);

        if (node.OverrideParentBank)
            newBank = GetParentBank(node.Parent);
        else
        {
            newBank = node.BankLink;
        }

        node.OverrideParentBank = !node.OverrideParentBank;

        MoveBetweenBanks(node, currentBank, newBank);

        SetNewBankLink(node, newBank);
    }

    private static void MoveBetweenBanks(AudioNode node, AudioBankLink oldBank, AudioBankLink newBankLink)
    {
        var currentBank = oldBank.LazyBankFetch.Clips;
        var newBank = newBankLink.LazyBankFetch.Clips;
        var nodeSet = new HashSet<AudioNode>();
        BuildMoveSet(nodeSet, node);
        for (int i = 0; i < currentBank.Count; ++i)
        {
            if (nodeSet.Contains(currentBank[i].Node))
            {
                newBank.Add(currentBank[i]);
                currentBank.SwapRemoveAt(i);
            }
        }
    }

    public static void MoveNode(AudioNode movedNode, AudioBankLink oldParent)
    {
        MoveBetweenBanks(movedNode, oldParent, GetParentBank(movedNode));
    }

    public static void SetNewBank(AudioNode node, AudioBankLink newBankLink)
    {
        MoveBetweenBanks(node, GetParentBank(node), newBankLink);

        node.BankLink = newBankLink;
        SetNewBankLink(node, newBankLink);
    }

    private static void SetNewBankLink(AudioNode node, AudioBankLink newBankLink)
    {
        for (int i = 0; i < node.Children.Count; ++i)
        {
            var child = node.Children[i];
            if (child.Type == AudioNodeType.Folder && child.OverrideParentBank)
                continue;
            else
            {
                if(child.Type != AudioNodeType.Folder)
                    child.BankLink = newBankLink;
                SetNewBankLink(child, newBankLink);
            }
        }
    }

    private static void BuildMoveSet(HashSet<AudioNode> nodeSet, AudioNode node)
    {
        if (node.Type == AudioNodeType.Audio)
        {
            nodeSet.Add(node);
        }
        else
        {
            for (int i = 0; i < node.Children.Count; ++i)
            {
                var childNode = node.Children[i];
                if (childNode.Type == AudioNodeType.Folder && childNode.OverrideParentBank)
                    continue;
                else 
                    BuildMoveSet(nodeSet, childNode);
            }
        }
    }

    public static AudioBankLink GetParentBank(AudioNode node)
    {
        if (node.IsRoot)
            return node.BankLink;
        if (node.Type == AudioNodeType.Folder && node.OverrideParentBank)
            return node.BankLink;

        return GetParentBank(node.Parent);
    }
}
}
