using UnityEngine;
using System.Collections;

public class DataObject : GUIDraggableObject 
{
    private string m_Name;
    private int m_Value;

    public DataObject(string name, int value, Vector2 position)
        : base(position)
    {
        m_Name = name;
        m_Value = value;
    }

    public void OnGUI()
    {
        Rect drawRect = new Rect(position.x, position.y, 100.0f, 100.0f);

        GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));
        GUILayout.Label(m_Name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

        Rect dragRect = GUILayoutUtility.GetLastRect();
        dragRect = new Rect(dragRect.x + position.x, dragRect.y + position.y, dragRect.width, dragRect.height);

        if (IsDragging)
        {
            GUILayout.Label("Wooo...");
        }
        else if (GUILayout.Button("Yes!"))
        {
            Debug.Log("Yes. It is " + m_Value + "!");
        }
        GUILayout.EndArea();

        Drag(dragRect);
    }
 }
