using HDRAudio.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace HDRAudio.TreeDrawer
{
    public static class EventDrawer
    {
        private static GUIStyle noMargain;
        public static bool EventFoldout(AudioEvent node, bool isSelected)
        {
            if (noMargain == null)
            {
                noMargain = new GUIStyle();
                noMargain.margin = new RectOffset(0, 0, 0, 0);
            }

            Rect area = EditorGUILayout.BeginHorizontal();
            if(isSelected)
                GUI.DrawTexture(area, EditorResources.Background);
            
            if (node.Type != EventNodeType.Event)
                GUILayout.Space(EditorGUI.indentLevel * 16);
            else
                GUILayout.Space(EditorGUI.indentLevel * 24);
            bool folded = node.IsFoldedOut;

            if (node.Type != EventNodeType.Event)
            {
                Texture picture;
                if (folded || node.Children.Count == 0)
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
            }

      
            GUILayout.Space(30);
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();

            Rect labelArea = GUILayoutUtility.GetLastRect();
            Rect buttonArea = labelArea;
            if (!node.IsRoot)
            {
                buttonArea.x = buttonArea.x + 45 + EditorGUI.indentLevel*16;
                buttonArea.width = 20;
                buttonArea.height = 14;
                if (node.Type != EventNodeType.Event)
                {
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
                else
                {
                    buttonArea.x -= 10;
                    GUI.Label(buttonArea, EditorResources.Up, noMargain);
                    if (Event.current.ClickedWithin(buttonArea))
                    {
                        NodeWorker.MoveNodeOneUp(node);
                        Event.current.Use();
                    }
                    buttonArea.x += 15;
                    GUI.Label(buttonArea, EditorResources.Down, noMargain);
                    if (Event.current.ClickedWithin(buttonArea))
                    {
                        NodeWorker.MoveNodeOneDown(node);
                        Event.current.Use();
                    }
                }

            }
            if (node.Type != EventNodeType.Event)//As events are smaller
                labelArea.y += 6;
            labelArea.x += 80;
            EditorGUI.LabelField(labelArea, node.Type + " - " + node.Name);

            return folded;
        }
    }


}
