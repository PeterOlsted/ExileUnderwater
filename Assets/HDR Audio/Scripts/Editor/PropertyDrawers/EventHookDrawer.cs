using System.Linq;
using HDRAudio;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EventHookAttribute))]
public class EventHookDrawer : PropertyDrawer
{
    EventHookAttribute EventAttribute { get { return ((EventHookAttribute)attribute); } }

    private float LineHeight = 22;
    private float DragHeight = 20;

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        return base.GetPropertyHeight(prop, label) + prop.arraySize*LineHeight + DragHeight + 25;
    }

    public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label)
    {
        var labelPos = pos;
        Color backgroundColor = GUI.color;

        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        var labelStyle = GUI.skin.GetStyle("label");
        //int fontSize = labelStyle.fontSize;
        var eventTypeStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        //eventTypeStyle.fontSize = fontSize + 1;

        //labelStyle.fontSize = fontSize;
        //eventTypeStyle.fontSize = 12;
        labelPos.height = 14;
        eventTypeStyle.fontStyle = FontStyle.Bold;
        GUI.Label(labelPos, EventAttribute.EventType, eventTypeStyle);
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        for (int i = 0; i < prop.arraySize; ++i)
        {
            labelPos.y += LineHeight;
            labelPos.height = 20;
            AudioEvent audioEvent = prop.GetArrayElementAtIndex(i).objectReferenceValue as AudioEvent;
            if(audioEvent != null)
                GUI.Label(labelPos, audioEvent.GetName, labelStyle);
            else
                GUI.Label(labelPos, "Missing event", labelStyle);

            Rect buttonPos = labelPos;            
            buttonPos.x = pos.width - 100; //Align to right side
            buttonPos.width = 50;
            if (audioEvent == null)
                GUI.enabled = false;
            
            if (GUI.Button(buttonPos, "Find"))
            {
                EditorWindow.GetWindow<EventWindow>().Find(audioEvent);
            }
            GUI.enabled = true;
            buttonPos.x = pos.width - 44;
            buttonPos.width = 35;
            if (GUI.Button(buttonPos, "X"))
            {
                DeleteAtIndex(prop, i);
                
            }
        }
        labelPos.y += DragHeight + 4;
        labelPos.height = DragHeight;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.color = backgroundColor;
        GUI.Button(labelPos, "Drag event here to add to " + EventAttribute.EventType.ToLower() + " event");
        if (labelPos.Contains(Event.current.mousePosition))
        {
            HandleDrag(prop);
        }

        GUI.color = backgroundColor;
        
        labelPos.height += 1;
        //GUI.Label(labelPos, "Drag event here to add");

    }

    private void DeleteAtIndex(SerializedProperty prop, int index)
    {
        int arraySize = prop.arraySize;
        prop.DeleteArrayElementAtIndex(index);
        for (int i = index; i < arraySize - 1; ++i)
        {
            prop.GetArrayElementAtIndex(i).objectReferenceValue = prop.GetArrayElementAtIndex(i + 1).objectReferenceValue;
        }
        prop.arraySize--;
    }
    private void HandleDrag(SerializedProperty prop)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            bool canDropObject = true;
            int clipCount = DragAndDrop.objectReferences.Count(obj =>
            {
                var audioEvent = obj as AudioEvent;
                if (audioEvent == null)
                    return false;
                return audioEvent.Type == EventNodeType.Event;
            });

            if (clipCount != DragAndDrop.objectReferences.Length || clipCount == 0)
                canDropObject = false;

            if (canDropObject)
            {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        int arraySize = prop.arraySize;
                        prop.arraySize++;

                        prop.GetArrayElementAtIndex(arraySize - 1).objectReferenceValue = DragAndDrop.objectReferences[0];
                    }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
    }
}
