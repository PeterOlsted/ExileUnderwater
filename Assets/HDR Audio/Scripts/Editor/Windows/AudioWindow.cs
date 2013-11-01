using System;
using UnityEditor;
using UnityEngine;

public class AudioWindow : HDRBaseWindow
{
    private AudioCreatorGUI audioCreatorGUI;

    void OnEnable()
    {
        BaseEnable();
        if (audioCreatorGUI == null)
        {
            audioCreatorGUI = new AudioCreatorGUI(this);
            audioCreatorGUI.OnEnable();
        }
    }

    public void Find(Func<AudioNode, bool> filter)
    {
        audioCreatorGUI.FindAudio(filter);
    }

    public void Find(AudioNode toFind)
    {
        audioCreatorGUI.FindAudio(toFind);
    }

    [MenuItem("Window/HDR Audio System/Audio Creator")]
    public static void Launch()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(AudioWindow));
        window.Show(); 
        
        //window.minSize = new Vector2(800, 200);
        window.title = "Audio Window";

    }

    private GameObject cleanupGO;

    void Update()
    {
        if (cleanupGO == null)
        {
            cleanupGO = Resources.Load("PrefabGO") as GameObject;
            DontDestroyOnLoad(cleanupGO);
        }

        BaseUpdate();
        if (audioCreatorGUI != null && Manager != null) 
            audioCreatorGUI.OnUpdate();
    }

    void OnGUI()
    {
        KeyboardWindowControls();
        if (!HandleMissingData())
        {
            return;
        }

        if (audioCreatorGUI == null)
            audioCreatorGUI = new AudioCreatorGUI(this);

        isDirty = false;
        DrawTop(topHeight);
        
        isDirty |= audioCreatorGUI.OnGUI(LeftWidth, (int)position.height - topHeight);
   
        if(isDirty)
            Repaint();

        PostOnGUI();
    }

    private void DrawTop(int topHeight)
    {
        EditorGUILayout.BeginVertical(GUILayout.Height(topHeight));
        EditorGUILayout.EndVertical();
    }
}   
