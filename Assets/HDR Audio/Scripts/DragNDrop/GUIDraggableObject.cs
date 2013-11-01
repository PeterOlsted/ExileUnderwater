using UnityEngine;
using System.Collections;

public class GUIDraggableObject
{ 
    protected Vector2 position;
    private Vector2 dragStart;
    private bool dragging;

    public GUIDraggableObject(Vector2 position)
    {
        this.position = position;
    }

    public GUIDraggableObject()
    {
    }

    public Vector2 StartPosition
    {
        get { return dragStart; }
    }
   
    public bool IsDragging
    {
        get
        {
            return dragging;
        }
    }

    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value; 
        }
    }

    public void Drag(Rect draggingRect)
    {
        if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Escape)
        {
            dragging = false;
        }
        else if (Event.current.type == EventType.MouseDown && draggingRect.Contains(Event.current.mousePosition))
        {
            dragging = true;
            dragStart = Event.current.mousePosition;
            Event.current.Use();
        }
    }
}