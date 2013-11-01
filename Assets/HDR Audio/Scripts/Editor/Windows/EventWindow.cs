using UnityEditor;
using UnityEngine;

public class EventWindow : HDRBaseWindow
{
    private AudioEventCreatorGUI audioEventCreatorGUI;

    void OnEnable()
    {
        BaseEnable();

        audioEventCreatorGUI = new AudioEventCreatorGUI(this);
        audioEventCreatorGUI.OnEnable();
    }


    [MenuItem("Window/HDR Audio System/Event Manager")]
    public static void Launch()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(EventWindow));
        window.Show();

        //window.minSize = new Vector2(500, 500);
        window.title = "Event Window";
    }

    void Update()
    {
        BaseUpdate();
        if (audioEventCreatorGUI != null && Manager != null)
            audioEventCreatorGUI.OnUpdate();
    }

    public void Find(AudioEvent toFind)
    {
        audioEventCreatorGUI.Find(toFind);
    }

    void OnGUI()
    {
        KeyboardWindowControls();
        if (!HandleMissingData())
        {
            return; 
        }
        
  
        if (audioEventCreatorGUI == null)
            audioEventCreatorGUI = new AudioEventCreatorGUI(this);

        isDirty = false;
        DrawTop(0);

        isDirty |= audioEventCreatorGUI.OnGUI(LeftWidth, (int)position.height - topHeight);

        if (isDirty)
            Repaint();

        PostOnGUI();
    }

    private void DrawTop(int topHeight)
    {
        EditorGUILayout.BeginVertical(GUILayout.Height(topHeight));
        EditorGUILayout.EndVertical();
    }

}
