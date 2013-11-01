using System;
using System.Collections.Generic;
using System.Linq;
using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HDRAudio
{
public static class AudioEventWorker  {
    private static AudioEvent CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<AudioEvent>();
        node.Type = EventNodeType.Root;
        node.GUID = guid;
        node.FoldedOut = true;
        return node;
    }

    private static AudioEvent CreateFolder(GameObject go, int guid, AudioEvent parent)
    {
        var node = go.AddComponent<AudioEvent>();
        node.Type = EventNodeType.Folder;
        node.GUID = guid;
        node.AssignParent(parent);
        return node;
    }

    public static void DeleteNode(AudioEvent node)
    {
        for (int i = 0; i < node.Children.Count; ++i)
        {
            DeleteNode(node.Children[i]);
        }
        node.Parent.Children.Remove(node);

        foreach (var data in node.ActionList)
            Object.DestroyImmediate(data, true);
        Object.DestroyImmediate(node, true);
    }

   
    private static AudioEvent CreateEvent(GameObject go, AudioEvent parent, int guid, EventNodeType type)
    {
        var node = go.AddComponent<AudioEvent>();
        node.Type = type;
        node.GUID = guid;
        node.AssignParent(parent);
        return node;
    }

    public static AudioEvent CreateTree(GameObject go, int levelSize)
    {
        var tree = CreateRoot(go, GUIDCreator.Create());

        for (int i = 0; i < levelSize; ++i)
        {
            CreateFolder(go, GUIDCreator.Create(), tree);
        }

        return tree;
    }

    public static AudioEvent CreateNode(AudioEvent audioEvent, EventNodeType type)
    {
        var child = CreateEvent(audioEvent.gameObject, audioEvent, GUIDCreator.Create(), type);
        child.FoldedOut = true;
        child.Name = "Name";
        
        return child;
    }

    public static void ReplaceActionDestructiveAt(AudioEvent audioEvent, EventActionTypes enumType, int toRemoveAndInsertAt)
    {
        float delay = audioEvent.ActionList[toRemoveAndInsertAt].Delay;
        var newActionType = ActionEnumToType(enumType);
        DeleteActionAtIndex(audioEvent, toRemoveAndInsertAt);
        var added = AddEventAction(audioEvent, newActionType, enumType);
        added.Delay = delay;
        audioEvent.ActionList.Insert(toRemoveAndInsertAt, added);
        audioEvent.ActionList.RemoveLast();
    }

    public static T AddEventAction<T>(AudioEvent audioevent, EventActionTypes enumType) where T : AudioEventAction
    {
        var eventAction = audioevent.gameObject.AddComponent<T>();
        audioevent.ActionList.Add(eventAction);
        eventAction.EventActionType = enumType;
        return eventAction;
    }

    public static AudioEventAction AddEventAction(AudioEvent audioevent, Type eventActionType, EventActionTypes enumType) 
    {
        var eventAction = audioevent.gameObject.AddComponent(eventActionType) as AudioEventAction;
        audioevent.ActionList.Add(eventAction);
        eventAction.EventActionType = enumType;

        return eventAction;
    }

    public static AudioEvent DeleteActionAtIndex(AudioEvent audioevent, int index)
    {
        AudioEventAction eventAction = audioevent.ActionList.TryGet(index);
        if (eventAction != null)
        {
            audioevent.ActionList.FindSwapRemove(eventAction);
            Object.DestroyImmediate(eventAction, true);
        }

        return audioevent;
    }

    public static AudioEvent Duplicate(AudioEvent audioEvent)
    {
        return NodeWorker.DuplicateHierarchy(audioEvent, (@oldNode, newNode) =>
        {
            newNode.ActionList.Clear();
            for (int i = 0; i < oldNode.ActionList.Count; i++)
            {
                newNode.ActionList.Add(NodeWorker.CopyComponent(oldNode.ActionList[i]));
            }
        });
    }


    public static Type ActionEnumToType(EventActionTypes actionType)
    {
        switch(actionType)
        {
            case EventActionTypes.Play:
                return typeof( EventAudioAction);
            case EventActionTypes.Stop:
                return typeof( EventAudioAction);
            case EventActionTypes.StopAll:
                return typeof( EventAudioAction);
            case EventActionTypes.LoadBank:
                return typeof( EventBankAction);
            case EventActionTypes.UnloadBank:
                return typeof(EventBankAction);
            case EventActionTypes.SetBusVolume:
                return typeof( EventBusAction);
            case EventActionTypes.Break:
                return typeof( EventAudioAction);
            case EventActionTypes.StopAllInBus:
                return typeof( EventBusAction);
        }
        return null;
    }

    public static bool CanDropObjects(AudioEvent audioEvent, Object[] objects)
    {
        if (objects.Length == 0 || audioEvent == null)
            return false;

        if (audioEvent.Type == EventNodeType.Event)
        {
            var audioNodes = GetConvertedList<AudioNode>(objects.ToList());
            bool audioNodeDrop = audioNodes.TrueForAll(node => node != null && node.IsPlayable);

            var audioBankLinks = GetConvertedList<AudioBankLink>(objects.ToList());
            bool bankLinkDrop = audioBankLinks.TrueForAll(node => node != null && node.Type == AudioBankTypes.Link);

            var busNodes = GetConvertedList<AudioBus>(objects.ToList());
            bool audioBusDrop = busNodes.TrueForAll(node => node != null);

            return audioNodeDrop | bankLinkDrop | audioBusDrop;
        }
        else if (audioEvent.Type == EventNodeType.Folder || audioEvent.Type == EventNodeType.Root)
        {
            var draggingEvent = objects[0] as AudioEvent;
            if (draggingEvent == null)
                return false;

            if (draggingEvent.Type == EventNodeType.Event)
                return true;
            if ((draggingEvent.Type == EventNodeType.Folder && !NodeWorker.IsChildOf(draggingEvent, audioEvent)) || draggingEvent.Type == EventNodeType.EventGroup)
                return true;
        }
        else if (audioEvent.Type == EventNodeType.EventGroup)
        {
            var draggingEvent = objects[0] as AudioEvent;
            if (draggingEvent == null)
                return false;
            if (draggingEvent.Type == EventNodeType.Event)
                return true;
        }

        return false;
    }

    private static List<T> GetConvertedList<T>(List<Object> toConvert) where T : class
    {
        return toConvert.ConvertAll(obj => obj as T);
    }

    public static void OnDrop(AudioEvent audioevent, Object[] objects)
    {
        if (objects.Length == 1)
        {
            if (objects[0] as AudioEvent)
            {
                var movingEvent = objects[0] as AudioEvent;
                Undo.RegisterUndo(new Object[] { audioevent, movingEvent, movingEvent.Parent }, "Event Move");
                NodeWorker.ReasignNodeParent((AudioEvent)objects[0], audioevent);
                audioevent.IsFoldedOut = true;
            }

            var audioNode = objects[0] as AudioNode;
            if (audioNode != null && audioNode.IsPlayable)
            {
                var action = AddEventAction<EventAudioAction>(audioevent,
                    EventActionTypes.Play);
                action.Node = audioNode;
            }

            var audioBank = objects[0] as AudioBankLink;
            if (audioBank != null)
            {
                var action = AddEventAction<EventBankAction>(audioevent,
                    EventActionTypes.LoadBank);
                action.BankLink = audioBank;
            }

            var audioBus = objects[0] as AudioBus;
            if (audioBus != null)
            {
                var action = AddEventAction<EventBusAction>(audioevent,
                    EventActionTypes.SetBusVolume);
                action.Bus = audioBus;
            }
            Event.current.Use();
        }
    }

}
}
