using System;
using System.Linq;
using HDRAudio;
using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public static class OnDragging  {

    public static T DraggingObject<T>(Rect area, Func<T, bool> predicate) where T : Object
    {
        if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() && DragAndDrop.objectReferences.Length > 0)
        {
            T casted = DragAndDrop.objectReferences[0] as T;

            if (casted != null && predicate(casted))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                {
                    return DragAndDrop.objectReferences[0] as T;
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
        return null;
    }

    public static void OnDraggingObject(Object[] objects, Rect area, Func<Object[], bool> predicate, Action<Object[]> OnDrop)
    {
        if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() && DragAndDrop.objectReferences.Length > 0)
        {
            if (predicate(objects))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                {
                    OnDrop(objects);
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
    }
}
