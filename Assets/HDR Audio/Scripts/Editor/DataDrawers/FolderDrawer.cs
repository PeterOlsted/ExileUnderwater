using HDRAudio;
using UnityEditor;
using UnityEngine;

public static class FolderDrawer
{
    public static void Draw(AudioNode node)
    {
        EditorGUILayout.BeginVertical();
        UndoHandler.CheckUndo(new UnityEngine.Object[] { node, node.NodeData });
        node.Name = EditorGUILayout.TextField("Name", node.Name);
        UndoHandler.CheckGUIChange();

        if (node.Type != AudioNodeType.Root)
        {
            bool overrideparent = EditorGUILayout.Toggle("Override Parent", node.OverrideParentBank);
            if (overrideparent != node.OverrideParentBank)
            { 
                AudioBankWorker.ChangeBankOverriding(node);
            }
        }
        else
            EditorGUILayout.LabelField(""); //To fill out the area from the toggle

        if (node.OverrideParentBank == false && node.Type != AudioNodeType.Root)
        {
            GUI.enabled = false;
        }

        EditorGUILayout.BeginHorizontal();

        var parentLink = FindParentBank(node);
        if (node.OverrideParentBank)
        {
            if (node.BankLink != null)
            {
                EditorGUILayout.LabelField("Bank", node.BankLink.GetName);
            }
            else
            {
                EditorGUILayout.LabelField("Bank", "Missing Bank, using parent bank" + parentLink.GetName);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Using Bank", parentLink.GetName); 
        }

        bool wasEnabled = GUI.enabled;
        GUI.enabled = true;
        if(GUILayout.Button("Find", GUILayout.Width(50)))
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(parentLink);
        }
        
        Rect findArea = GUILayoutUtility.GetLastRect();
        findArea.y += 20;
        if (GUI.Button(findArea, "Find"))
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(node.BankLink);
        }

        GUI.enabled = wasEnabled;

        GUILayout.Button("Drag new bank here", GUILayout.Width(140));

        var newBank = HandleDragging(GUILayoutUtility.GetLastRect());
        if (newBank != null)
        {
            if(node.OverrideParentBank)
                AudioBankWorker.SetNewBank(node, newBank);
            else
            {
                node.BankLink = newBank;
            }
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUI.enabled = false;
        EditorGUILayout.LabelField("Node Bank", node.BankLink.GetName);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
    }

    private static AudioBankLink HandleDragging(Rect area)
    {
        if (area.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated ||
            Event.current.type == EventType.DragPerform)
        {
            if (DragAndDrop.objectReferences.Length != 0)
            {
                var bankLink = DragAndDrop.objectReferences[0] as AudioBankLink;
                if (bankLink != null && bankLink.Type == AudioBankTypes.Link)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        return DragAndDrop.objectReferences[0] as AudioBankLink;
                    }
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
        return null;
    }

    private static AudioBankLink FindParentBank(AudioNode node)
    {
        if (node.OverrideParentBank)
        {
            return node.BankLink;
        }
        else if (node.IsRoot)
        {
            return node.BankLink;
        }
        else
            return FindParentBank(node.Parent);
    }
}
