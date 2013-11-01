using System;
using System.Collections.Generic;
using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HDRAudio
{
public static class AudioNodeWorker  {
    public static AudioNode CreateNode(GameObject go, AudioNode parent, int guid, AudioNodeType type)
    {
        var node = go.AddComponent<AudioNode>();
        node.GUID = guid;
        node.Type = type;
        node.Bus = parent.Bus;
        NodeWorker.AssignParent(node, parent);

        return node;
    }

    public static AudioNode CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<AudioNode>();
        node.GUID = guid;
        node.Type = AudioNodeType.Root;
        node.FoldedOut = true;
        return node;
    }

    public static AudioNode CreateTree(GameObject go, int numberOfChildren, AudioBus bus)
    {
        var Tree = CreateRoot(go, GUIDCreator.Create());
        Tree.Bus = bus;
        for (int i = 0; i < numberOfChildren; ++i)
        {
            CreateNode(go, Tree, GUIDCreator.Create(), AudioNodeType.Folder);
        }
        return Tree;
    }

    public static AudioNode CreateNode(GameObject go, AudioNode parent, AudioNodeType type)
    {
        var newNode = CreateNode(go, parent, GUIDCreator.Create(), type);
        newNode.Name = "Name";
        AddDataClass(newNode);
        return newNode;
    }

    public static void AddDataClass(AudioNode node)
    {
        switch (node.Type)
        {
            case AudioNodeType.Audio:
                node.NodeData = node.gameObject.AddComponent<AudioData>();
                break;
            case AudioNodeType.Random:
                node.NodeData = node.gameObject.AddComponent<RandomData>();
                for (int i = 0; i < node.Children.Count; ++i)
                    (node.NodeData as RandomData).weights.Add(50);
                break;
            case AudioNodeType.Sequence:
                node.NodeData = node.gameObject.AddComponent<SequenceData>();
                break;
            case AudioNodeType.Multi:
                node.NodeData = node.gameObject.AddComponent<MultiData>();
                break;
        }
    }

    public static void AddNewParent(AudioNode node, AudioNodeType parentType)
    {   
        var newParent = CreateNode(node.gameObject, node.Parent, parentType);
        var oldParent = node.Parent;
        newParent.Bus = node.Bus;
        newParent.FoldedOut = true;
        newParent.BankLink = AudioBankWorker.GetParentBank(oldParent);
        int index = oldParent.Children.FindIndex(node);
        NodeWorker.RemoveFromParent(node);
        NodeWorker.AssignParent(node, newParent);

        OnRandomNode(newParent);

        NodeWorker.RemoveFromParent(newParent);
        oldParent.Children.Insert(index, newParent);
    }

    private static void OnRandomNode(AudioNode parent)
    {
        if (parent.Type == AudioNodeType.Random)
            (parent.NodeData as RandomData).weights.Add(50);
    }
     
    public static AudioNode CreateChild(AudioNode parent, AudioNodeType newNodeType)
    {
        Undo.RegisterSceneUndo("Child creation");
        OnRandomNode(parent);

        var child = CreateNode(parent.gameObject, parent, GUIDCreator.Create(), newNodeType);
        parent.FoldedOut = true;
        child.Name = "Name";
        child.BankLink = AudioBankWorker.GetParentBank(child);
        AddDataClass(child);
        return child;
    }

    public static void ConvertNodeType(AudioNode node, AudioNodeType newType)
    {
        if (newType == node.Type)
            return;

        Object.DestroyImmediate(node.NodeData, true);
        node.Type = newType;
        AddDataClass(node);
    }

    public static AudioNode Duplicate(AudioNode audioNode)
    {
        return NodeWorker.DuplicateHierarchy(audioNode, (@oldNode, newNode) =>
        {
            newNode.NodeData = audioNode.gameObject.AddComponent(newNode.NodeData.GetType()) as NodeTypeData;
            EditorUtility.CopySerialized(oldNode.NodeData, newNode.NodeData);
        });
    }

    public static void DeleteNode(AudioNode node)
    {
        for (int i = node.Children.Count - 1; i > 0; --i)
            DeleteNode(node.Children[i]);

        if (node.Parent.Type == AudioNodeType.Random) //We also need to remove the child from the weight list
        {
            var data = node.Parent.NodeData as RandomData;
            if(data != null)
                data.weights.RemoveAt(node.Parent.Children.FindIndex(node)); //Find in parent, and then remove the weight in the random node
        }

        AudioBankWorker.RemoveNodeFromBank(node);

        node.Parent.Children.Remove(node);
        Object.DestroyImmediate(node.NodeData, true);
        Object.DestroyImmediate(node, true);
    }

   
}
}
