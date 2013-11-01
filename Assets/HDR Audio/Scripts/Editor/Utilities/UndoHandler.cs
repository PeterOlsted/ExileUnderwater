using UnityEditor;
using UnityEngine;

public class UndoHandler : MonoBehaviour {

    private static bool guiChanged;
    private static Object src;
    private static Object[] srcObjs;
    private static bool listeningForGuiChanges;

    public static void CheckGUIChange()
    {
        if (GUI.changed)
        {
            guiChanged = true;
        }
    }

    public static void CheckUndo(Object target, string description = "Undo Audio Change")
    {
        src = target;
        srcObjs = null;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 || e.type == EventType.KeyUp && (e.keyCode == KeyCode.Tab))
        {
            // When the LMB is pressed or the TAB key is released,
            // store a snapshot, but don't register it as an undo
            // ( so that if nothing changes we avoid storing a useless undo)
            Undo.SetSnapshotTarget(src, description);
            Undo.CreateSnapshot();
            Undo.ClearSnapshotTarget();
            listeningForGuiChanges = true;
            guiChanged = false;
        }

        if (listeningForGuiChanges && guiChanged)
        {
            // Some GUI value changed after pressing the mouse.
            // Register the previous snapshot as a valid undo.
            Undo.SetSnapshotTarget(src, description);
            Undo.RegisterSnapshot();
            Undo.ClearSnapshotTarget();
            listeningForGuiChanges = false;
        }
    }

    public static void CheckUndo(Object[] target, string description = "Undo Audio Change")
    {
        src = null;
        srcObjs = target;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 || e.type == EventType.KeyUp && (e.keyCode == KeyCode.Tab))
        {
            // When the LMB is pressed or the TAB key is released,
            // store a snapshot, but don't register it as an undo
            // ( so that if nothing changes we avoid storing a useless undo)
            Undo.SetSnapshotTarget(srcObjs, description);
            Undo.CreateSnapshot();
            Undo.ClearSnapshotTarget();
            listeningForGuiChanges = true;
            guiChanged = false;
        }

        if (listeningForGuiChanges && guiChanged)
        {
            // Some GUI value changed after pressing the mouse.
            // Register the previous snapshot as a valid undo.
            Undo.SetSnapshotTarget(srcObjs, description);
            Undo.RegisterSnapshot();
            Undo.ClearSnapshotTarget();
            listeningForGuiChanges = false;
        }
    }
}
