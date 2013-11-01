using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProgressBar))]
public class ProgressBarDrawer : PropertyDrawer
{
    // Here you must define the height of your property drawer. Called by Unity.
    public override float GetPropertyHeight(SerializedProperty prop,
                                             GUIContent label)
    {
        return 80;
    }

    // Provide easy access to the RegexAttribute for reading information from it.
    ProgressBar progressBar { get { return ((ProgressBar)attribute); } }

    GUIStyle style;
    Texture2D texture;

    public ProgressBarDrawer()
    {
        style = new GUIStyle();
        texture = new UnityEngine.Texture2D(1, 1);
        Color r = Color.red;
        r.a = 0;
        texture.SetPixel(0,0,r);
        texture.SetPixel(1, 1, r);
        style.normal.background = texture;
    }


    // Here you can define the GUI for your property drawer. Called by Unity.
    public override void OnGUI(Rect position,
                                SerializedProperty prop,
                                GUIContent label)
    {
        float value = EditorGUI.Slider(position, prop.floatValue, progressBar.MinValue, progressBar.MaxValue);


        GUI.Button(position, "", style);
        

        
        prop.floatValue = value;
        position.y += 20;
        position.x += 22;
        position.xMax -= 78;
        EditorGUI.ProgressBar(position, value, progressBar.TextLabel);
    }
}
