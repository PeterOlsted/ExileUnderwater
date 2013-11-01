using System;
using UnityEditor;
using UnityEngine;


public class TestWindow : EditorWindow
{

    [MenuItem("Window/Editor Window Test")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GetWindow(typeof(TestWindow));
    }

    private Vector2 vec = new Vector2(0, 322.95f);
    private float height = 315;
     
    void OnGUI() 
    {
        if (GUILayout.Button("Save skin"))
        {
            GUISkin skin = ScriptableObject.Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector)) as GUISkin;

            AssetDatabase.CreateAsset(skin, "Assets/EditorSkin.guiskin");
        }
        GameObject sel = Selection.activeGameObject;
        if (sel == null)
            return;
        AudioSource targetComp = sel.GetComponent<AudioSource>();

        vec = EditorGUILayout.Vector2Field("", vec);
        height = EditorGUILayout.FloatField(height); 
        if (targetComp != null)
        {
            
            EditorGUILayout.BeginScrollView(vec, GUIStyle.none, GUIStyle.none, GUILayout.Width(400), GUILayout.Height(height));
            //Rect area = EditorGUILayout.BeginVertical();
            try
            {
                
                var editor = Editor.CreateEditor(targetComp);
                editor.OnInspectorGUI();
            }
            catch (Exception)
            {
                throw;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            //GUI.Button(GUILayoutUtility.GetLastRect(), "", new GUIStyle());
        }
        
    }
}
