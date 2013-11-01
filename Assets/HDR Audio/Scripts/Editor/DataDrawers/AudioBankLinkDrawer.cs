using HDRAudio;
using UnityEditor;
using UnityEngine;

public static class AudioBankLinkDrawer
{
    public static void Draw(AudioBankLink node)
    {
        UndoHandler.CheckUndo(node);
        //UndoCheck.Instance.CheckUndo(node, "Audio Bank:" + node.Name);
        EditorGUILayout.BeginVertical();
        node.Name = EditorGUILayout.TextField("Name", node.Name);
        if (node.Type != AudioBankTypes.Folder)
        {
            EditorGUILayout.IntField("ID", node.GUID);
            EditorGUILayout.Separator();
            node.AutoLoad = EditorGUILayout.Toggle("Auto load", node.AutoLoad);
        }


        Rect lastArea = GUILayoutUtility.GetLastRect();
        lastArea.y += 28;
        lastArea.width = 200;
        if(GUI.Button(lastArea, "Find Folders using this bank"))
        {
            EditorWindow.GetWindow<AudioWindow>().Find(audioNode => AudioBankWorker.GetParentBank(audioNode) != node);
        }

        EditorGUILayout.EndVertical();
        //UndoCheck.Instance.CheckDirty(node);
        UndoHandler.CheckGUIChange();
    }
}
