using System;
using System.Collections.Generic;
using HDRAudio;
using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class NodeWorker  {
    public static bool IsChildOf<T>(T node, T potentialChild) where T : Object, ITreeNode<T>
    {
        if (node == potentialChild)
            return true;

        for (int i = 0; i < node.GetChildren.Count; ++i)
        {
            bool isChild = IsChildOf(node.GetChildren[i], potentialChild);
            if (isChild)
                return true;
        }
        return false;
    }

    public static void RemoveFromParent<T>(T node) where T : Object, ITreeNode<T>
    {
        node.GetParent.GetChildren.Remove(node);
    }

    public static void AssignParent<T>(T node, T newParent) where T : Object, ITreeNode<T>
    {
        if (node != null && newParent != null)
        {
            newParent.GetChildren.Add(node);
            node.GetParent = newParent;
        }
    }

    public static void ReasignNodeParent<T>(T current, T newParent) where T : Object, ITreeNode<T>
    {
        RemoveFromParent(current);
        AssignParent(current, newParent);
    }

    public static void ReasignNodeParent(AudioNode current, AudioNode newParent) 
    {
        if (current.Parent.Type == AudioNodeType.Random)
        {
            //Remove self from parent random
            int currentIndexInParent = current.Parent.Children.FindIndex(child => child == current);
            (current.Parent.NodeData as RandomData).weights.RemoveAt(currentIndexInParent);
        }
        RemoveFromParent(current);
        AssignParent(current, newParent);
        if (newParent.Type == AudioNodeType.Random)
        {
            (current.Parent.NodeData as RandomData).weights.Add(50);
        }
       
    }

    public static void MoveNodeOneUp<T>(T node) where T : Object, ITreeNode<T>
    {
        if (node is AudioNode)
        {
            Undo.RegisterUndo(new Object[] { node.GetParent, (node.GetParent as AudioNode).NodeData }, "Undo Reorder Of " + node.GetName);
        }
        else
        {
            Undo.RegisterUndo(new Object[] { node.GetParent }, "Undo Reorder Of " + node.GetName);
        }
        var children = node.GetParent.GetChildren;
        var index = children.IndexOf(node);
        if (index != 0 && children.Count > 0)
        {
            //TODO Remove hack
            if (node.GetType() == typeof (AudioNode))
            {
                var audioNode = node as AudioNode;
                if (audioNode.Parent.Type == AudioNodeType.Random)
                {
                    (audioNode.Parent.NodeData as RandomData).weights.SwapAtIndexes(index, index - 1);
                }
            }
            children.SwapAtIndexes(index, index - 1);            
        }
    }

  
    public static void MoveNodeOneDown<T>(T node) where T : Object, ITreeNode<T>
    {
        if (node is AudioNode)
        {
            Undo.RegisterUndo(new Object[] { node.GetParent, (node.GetParent as AudioNode).NodeData }, "Undo Reorder Of " + node.GetName);
        }
        else
        {
            Undo.RegisterUndo(new Object[] { node.GetParent }, "Undo Reorder Of " + node.GetName);    
        }
        
        var children = node.GetParent.GetChildren;
        var index = children.IndexOf(node);
        if (children.Count > 0 && index != children.Count - 1)
        {
            //TODO Remove hack
            if (node.GetType() == typeof(AudioNode))
            {
                var audioNode = node as AudioNode;
                if (audioNode.Parent.Type == AudioNodeType.Random)
                {
                    (audioNode.Parent.NodeData as RandomData).weights.SwapAtIndexes(index, index + 1);
                }
            }
            children.SwapAtIndexes(index, index + 1);
        }
    }

    public static void AssignToNodes(AudioNode node, Action<AudioNode> assignFunc)
    {
        assignFunc(node);
        for (int i = 0; i < node.Children.Count; ++i)
        {
            AssignToNodes(node.Children[i], assignFunc);
        }
    }

    public static T DuplicateHierarchy<T>(T toCopy, Action<T, T> elementAction = null) where T : Component, ITreeNode<T>
    {
        return CopyHierarchy(toCopy, toCopy.GetParent, elementAction);
    }

    private static T CopyHierarchy<T>(T toCopy, T parent, Action<T, T> elementAction) where T : Component, ITreeNode<T>
    {
        T newNode = CopyComponent(toCopy);
        AssignParent(newNode, parent);
        newNode.ID = GUIDCreator.Create();

        if(elementAction != null)
            elementAction(toCopy, newNode);
        
        int childrenCount = newNode.GetChildren.Count;
        for (int i = 0; i < childrenCount; i++)
        {
            CopyHierarchy(newNode.GetChildren[i], newNode, elementAction);
        }
        newNode.GetChildren.RemoveRange(0, childrenCount);
        return newNode;
    }

    public static T CopyComponent<T>(T toCopy) where T : Component
    {
        T newComp = toCopy.gameObject.AddComponent(toCopy.GetType()) as T;
        EditorUtility.CopySerialized(toCopy, newComp);

        return newComp;
    }

    public static void FindAllNodes<T>(T node, Func<T, bool> condition, List<T> foundNodes) where T : Object, ITreeNode<T>
    {
        if (condition(node))
        {
            foundNodes.Add(node);
        }
        for (int i = 0; i < node.GetChildren.Count; ++i)
        {
            FindAllNodes(node.GetChildren[i], condition, foundNodes);
        }
    }
}
