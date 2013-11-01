using UnityEditor;
using UnityEngine;

public static class AudioBusDrawer
{
    public static void Draw(AudioBus node)
    {
        checkMouse(node);
        EditorGUILayout.BeginVertical();

        node.Name = EditorGUILayout.TextField("Name", node.Name);
        Undo.ClearSnapshotTarget();
        checkMouse(node);
        //In AuxWindow, update all bus volumes 
        node.Volume = EditorGUILayout.Slider("Volume", node.Volume, 0.0f, 1.0f);
        Undo.ClearSnapshotTarget();
        GUI.enabled = false;
        checkMouse(node);
        EditorGUILayout.Slider("Combined Volume", node.CombinedVolume, 0, 1.0f);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        //Undo.ClearSnapshotTarget();
    }

    static void checkMouse(AudioBus bus)
    {
        Event e = Event.current;
        if (e.button == 0 && e.isMouse)
        {
            Undo.SetSnapshotTarget(bus, "Changed Slider");
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
        }
    }
}
