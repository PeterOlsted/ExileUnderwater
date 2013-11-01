using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace HDRAudio.TreeDrawer 
{
public class BusDrawer
{
    private static GUIStyle noMargain;
    public static bool Draw(AudioBus node, bool isSelected)
    {
        if (noMargain == null)
        {
            noMargain = new GUIStyle();
            noMargain.margin = new RectOffset(0, 0, 0, 0);
        }

        Rect area = EditorGUILayout.BeginHorizontal();
        if (isSelected)
            GUI.DrawTexture(area, EditorResources.Background);
        GUILayout.Space(EditorGUI.indentLevel * 16);
        bool folded = node.FoldedOut;

        Texture picture;
        if (folded || node.Children.Count == 0)
            picture = HDRAudio.EditorResources.Minus;
        else
            picture = HDRAudio.EditorResources.Plus;

        GUILayout.Label(picture, noMargain, GUILayout.Height(EditorResources.Minus.height), GUILayout.Width(EditorResources.Minus.width));
        Rect foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.ClickedWithin(foldRect))
        {
            folded = !folded;
            Event.current.Use();
        }
        EditorGUILayout.EndHorizontal();

        Rect labelArea = GUILayoutUtility.GetLastRect();
        Rect buttonArea = labelArea;
        Rect sliderArea = buttonArea;
        
        
        if (!node.IsRoot)
        {
            buttonArea.x = buttonArea.x + 45 + EditorGUI.indentLevel * 16;
            buttonArea.width = 20;
            buttonArea.height = 14;
            GUI.Label(buttonArea, EditorResources.Up, noMargain);
            if (Event.current.ClickedWithin(buttonArea))
            {
                NodeWorker.MoveNodeOneUp(node);
                Event.current.Use();
            }
            buttonArea.y += 15;
            GUI.Label(buttonArea, EditorResources.Down, noMargain);
            if (Event.current.ClickedWithin(buttonArea))
            {
                NodeWorker.MoveNodeOneDown(node);
                Event.current.Use();
            }
        }
        labelArea.y += 6;
        labelArea.x += 80;
        EditorGUI.LabelField(labelArea, node.Name);
        GUI.enabled = false;
        sliderArea.y += 6;
        sliderArea.x = labelArea.x + 100;
        sliderArea.height = 16;
        sliderArea.width = 180;
        node.CombinedVolume = EditorGUI.Slider(sliderArea, node.CombinedVolume, 0.0f, 1.0f);
        GUI.enabled = true;

        return folded;
    }
}
}
