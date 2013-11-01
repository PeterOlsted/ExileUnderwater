using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace HDRAudio.TreeDrawer
{
    public class GenericTreeNodeDrawer
    {
        private static GUIStyle noMargain;
        public static bool Draw<T>(T node, bool isSelected) where T : Object, ITreeNode<T>
        {
            if (noMargain == null)
            {
                noMargain = new GUIStyle();
                noMargain.margin = new RectOffset(0, 0, 0, 0);
            }

            Rect area = EditorGUILayout.BeginHorizontal();
            if(isSelected)
                GUI.DrawTexture(area, EditorResources.Background);

            GUILayout.Space(EditorGUI.indentLevel * 16);

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (folded || node.GetChildren.Count == 0)
                picture = EditorResources.Minus;
            else
                picture = EditorResources.Plus;

            GUILayout.Label(picture, noMargain, GUILayout.Height(EditorResources.Minus.height), GUILayout.Width(EditorResources.Minus.width));
            Rect foldRect = GUILayoutUtility.GetLastRect();
            if (Event.current.ClickedWithin(foldRect))
            {
                folded = !folded;
                Event.current.Use();
            }
            EditorGUILayout.LabelField("");


            EditorGUILayout.EndHorizontal();
            Rect labelArea = GUILayoutUtility.GetLastRect();
            Rect buttonArea = labelArea;
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
            TypeSpecificLabel(labelArea, node);

            return folded;
        }

        private static void TypeSpecificLabel<T>(Rect area, T node) where T : Object, ITreeNode<T>
        {
            if(node is AudioNode)
                EditorGUI.LabelField(area, (node as AudioNode).Type + " - " + node.GetName);
            else if (node is AudioBankLink)
                EditorGUI.LabelField(area, (node as AudioBankLink).Type + " - " + node.GetName);
            else
            {
                EditorGUI.LabelField(area, node.GetName);
            }
        }
    }
}
