using System;
using System.Text;
using HDRAudio;
using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AudioEventDrawer
{
    private static AudioEvent lastEvent;
    private static AudioEventAction audioEventAction = null;
    private static AudioEventAction drawnAudioEventAction = null;
    private static Vector2 scrollPos;
    private static Rect drawArea;
    public static bool Draw(AudioEvent audioevent)
    {
        UndoCheck.Instance.CheckUndo(audioevent);
        audioevent.Name = EditorGUILayout.TextField("Name", audioevent.Name);
        
        bool repaint = false;
        UndoCheck.Instance.CheckDirty(audioevent);
        if (audioevent.Type == EventNodeType.Event)
        {
            EditorGUILayout.IntField("ID", audioevent.GUID);

            EditorGUILayout.Separator();
            //
            UndoCheck.Instance.CheckUndo(audioevent);
            audioevent.Delay = Mathf.Max(EditorGUILayout.FloatField("Delay", audioevent.Delay), 0);
            UndoCheck.Instance.CheckDirty(audioevent);
          
            Rect entireArea = EditorGUILayout.BeginVertical();

            NewEventArea(audioevent);
            GUILayoutUtility.GetLastRect();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
            repaint = DrawContent(audioevent, entireArea);
            EditorGUILayout.EndScrollView();

            DrawSelected(drawnAudioEventAction);

            EditorGUILayout.EndVertical(); 
        }
        
        
        if(audioevent != lastEvent)
        {
            lastEvent = audioevent;
            if (audioevent.ActionList.Count > 0)
                audioEventAction = audioevent.ActionList[0];
            else
                audioEventAction = null;
            drawArea = new Rect();
            audioEventAction = null;
            scrollPos = new Vector2();
            repaint = true;
        }
         
        if (drawnAudioEventAction != audioEventAction)
        {
            repaint = true;
            drawnAudioEventAction = audioEventAction;
        }
      
        return repaint;
    }

    private static void DrawSelected(AudioEventAction eventAction)
    {
        if (eventAction != null)
        {
            UndoCheck.Instance.CheckUndo(eventAction);
            //UndoCheck.Instance.CheckUndo(eventAction, "Audio Event Action Change");    
            Rect thisArea = EditorGUILayout.BeginVertical(GUILayout.Height(100));
            EditorGUILayout.LabelField("");
            var buttonArea = thisArea;
            buttonArea.height = 16;

            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            eventAction.Delay = Mathf.Max(EditorGUI.FloatField(buttonArea, "Seconds Delay", eventAction.Delay), 0);
            
            buttonArea.y += 33;
            var busAction = eventAction as EventBusAction;
            if (busAction != null)
            {
                if (busAction.VolumeMode == EventBusAction.VolumeSetMode.Relative)
                    busAction.Volume = EditorGUI.Slider(buttonArea, "Relative Volume", busAction.Volume, -1.0f, 1.0f);
                else
                    busAction.Volume = EditorGUI.Slider(buttonArea, "Volume", busAction.Volume, 0.0f, 1.0f);

                buttonArea.y += 25;
                busAction.VolumeMode = (EventBusAction.VolumeSetMode)EditorGUI.EnumPopup(buttonArea, "Volume Mode", busAction.VolumeMode);
            }
            EditorGUILayout.EndVertical();
            UndoCheck.Instance.CheckDirty(eventAction);    
        }
        
    }

    private static void NewEventArea(AudioEvent audioevent)
    {
        var defaultAlignment = GUI.skin.label.alignment;

        EditorGUILayout.BeginHorizontal();

        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndHorizontal();
        Rect lastArea = GUILayoutUtility.GetLastRect();
        lastArea.height *= 1.5f;

        if (GUI.Button(lastArea, "Click or drag here to add event"))
        {
            ShowCreationContext(audioevent);
        }
        if (Event.current.type != EventType.Repaint)
        {
            var dragging = DragAndDrop.objectReferences;
            OnDragging.OnDraggingObject(dragging, lastArea,
                objects => AudioEventWorker.CanDropObjects(audioevent, dragging),
                objects => AudioEventWorker.OnDrop(audioevent, dragging));
        }
        GUI.skin.label.alignment = defaultAlignment;
    }

    private static bool DrawContent(AudioEvent audioevent, Rect area)
    {
        Rect selectedBackground = drawArea;
        selectedBackground.y += 2;
        DrawBackground(selectedBackground);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        bool repaint = false;
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < audioevent.ActionList.Count; ++i)
        {
            var currentAction = audioevent.ActionList[i];
            Rect lastArea = EditorGUILayout.BeginHorizontal(GUILayout.Height(20));

            if (i == 0)
            {
                Rect actionArea = lastArea;
                actionArea.width = 100;
                actionArea.height = 20;
                actionArea.y -= 20;
                
                EditorGUI.LabelField(actionArea, "Action", EditorStyles.boldLabel);
            }

            if (currentAction != null)
            {
                if (GUILayout.Button(
                        Enum.GetName(typeof (EventActionTypes), (int) currentAction.EventActionType)
                            .AddSpacesToSentence(),
                        GUILayout.Width(110)))
                {
                    ShowChangeContext(audioevent, currentAction);
                    Event.current.Use();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Missing");

                GUI.enabled = false;
            }

            EditorGUILayout.BeginVertical(GUILayout.Height(20));
            GUILayout.Label("", GUILayout.Height(0)); //Aligns it to the center by creating a small vertical offset, by setting the height to zero
            if (currentAction != null && currentAction.EventActionType != EventActionTypes.StopAll)
                EditorGUILayout.LabelField(currentAction.ObjectName);
            EditorGUILayout.EndHorizontal();

            Rect dragArea = GUILayoutUtility.GetLastRect();

            if (i == 0)
            {
                Rect actionArea = dragArea;
                actionArea.width = 100;
                actionArea.height = 20;
                actionArea.y -= 20;

                EditorGUI.LabelField(actionArea, "Data", EditorStyles.boldLabel);
            }

            if (currentAction is EventAudioAction)
            {
                AudioNode dragged = OnDragging.DraggingObject<AudioNode>(dragArea, node => node.IsPlayable);

                if (dragged != null)
                {
                    (currentAction as EventAudioAction).Node = dragged;
                }
            }
            else if (currentAction is EventBankAction)
            {
                AudioBankLink dragged = OnDragging.DraggingObject<AudioBankLink>(dragArea, bank => bank.Type == AudioBankTypes.Link);
                if (dragged != null)
                    (currentAction as EventBankAction).BankLink = dragged;
            }
            else if (currentAction is EventBusAction)
            {
                AudioBus dragged = OnDragging.DraggingObject<AudioBus>(dragArea, bus => true);
                if (dragged != null)
                    (currentAction as EventBusAction).Bus = dragged;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            Rect rightArea = EditorGUILayout.BeginHorizontal(GUILayout.Width(area.width - 100));
            rightArea.x -= 150;
            rightArea.y += 3;
            rightArea.width = 80;
            //rightARea.height -= 6;
            if (GUI.Button(rightArea, "Find"))
            {
                SearchHelper.SearchFor(currentAction);
            }

            rightArea.x += 90;
            rightArea.width = 30;
            if (GUI.Button(rightArea, "X"))
            {
                if (audioevent.ActionList[i] != null)
                    AudioEventWorker.DeleteActionAtIndex(audioevent, i);
                else
                {
                    audioevent.ActionList.SwapRemoveAt(i);
                    --i;
                }
            }

            if (Event.current.ClickedWithin(lastArea))
            {
                drawArea = lastArea;
                audioEventAction = currentAction;
                Event.current.Use();
            }
            //if (audioEventAction == null || ( Event.current.type == EventType.MouseDown && lastArea.Contains(Event.current.mousePosition)))
            //{
            //    drawArea = lastArea;
            //    audioEventAction = currentAction;
            //}
            GUILayout.Label("");
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
          
            GUI.enabled = true;

            
        }
        EditorGUILayout.EndVertical();
        return repaint;
    }


    private static void DrawBackground(Rect area)
    {
        GUI.depth += 10;
        GUI.DrawTexture(area, EditorResources.Background);
        GUI.depth -= 10;
    }

    private static void ShowChangeContext(AudioEvent audioEvent, AudioEventAction action)
    {
        var menu = new GenericMenu();
        foreach (EventActionTypes currentType in EnumUtil.GetValues<EventActionTypes>())
        {
            var enumType = currentType;
            menu.AddItem(
                new GUIContent(currentType.FormatedName()),
                false, 
                f => ChangeAction(audioEvent,action, enumType), 
                currentType
                );
        }
        menu.ShowAsContext();
    }

    private static void ChangeAction(AudioEvent audioEvent, AudioEventAction action, EventActionTypes newEnumType)
    {
        for (int i = 0; i < audioEvent.ActionList.Count; ++i)
        {
            if (audioEvent.ActionList[i] == action)
            {
                Type oldType = AudioEventWorker.ActionEnumToType(action.EventActionType);
                Type newType = AudioEventWorker.ActionEnumToType(newEnumType);
                if (oldType != newType)
                    AudioEventWorker.ReplaceActionDestructiveAt(audioEvent, newEnumType, i);
                else
                    action.EventActionType = newEnumType;
                break;
            }
        }
    }

    private static void ShowCreationContext(AudioEvent audioevent)
    {
        var menu = new GenericMenu();
        foreach (EventActionTypes currentType in EnumUtil.GetValues<EventActionTypes>())
        {
            Type newType = AudioEventWorker.ActionEnumToType(currentType);
            var enumType = currentType;
            menu.AddItem(new GUIContent(currentType.FormatedName()), false, f =>
                AudioEventWorker.AddEventAction(audioevent, newType, enumType), currentType);
        }
        menu.ShowAsContext();
    }
}
