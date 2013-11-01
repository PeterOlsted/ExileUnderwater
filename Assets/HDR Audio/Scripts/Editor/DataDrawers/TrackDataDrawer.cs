using UnityEditor;
using UnityEngine;

public static class TrackDataDrawer
{
    public static void Draw(AudioNode node)
    {
        UndoHandler.CheckUndo(new UnityEngine.Object[] { node, node.NodeData });
        node.Name = EditorGUILayout.TextField("Name", node.Name);
        NodeTypeDataDrawer.Draw(node);
        UndoHandler.CheckGUIChange();
    }
}
