using HDRAudio;
using UnityEditor;
using UnityEngine;

public class HDRBaseWindow : EditorWindow
{
    public CommonDataManager Manager;

    protected int topHeight = 0;
    protected int LeftWidth = 350;

    protected bool isDirty;

    protected void BaseEnable()
    {
        autoRepaintOnSceneChange = true;
        EditorApplication.modifierKeysChanged += Repaint;

        Manager = InstanceFinder.DataManager;

        EditorResources.Reload();
    }

    protected void BaseUpdate()
    {
        if (Event.current != null && Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    Repaint();
                    break;
            }
        }
    }

    protected bool HandleMissingData()
    {
        if (Manager == null)
        {
            Manager = InstanceFinder.DataManager;
            if (Manager == null)
            {
                ErrorDrawer.MissingAudioManager();
            }
        }

        if (Manager != null)
        {
            bool areAnyMissing = ErrorDrawer.IsDataMissing(Manager);

            if (areAnyMissing)
            {
                Manager.Load();
            }
            if (ErrorDrawer.IsDataMissing(Manager))
            {
                ErrorDrawer.MissingData(Manager);
                return false;
            }
            else
            {
                return true;
            }
        }
        else 
            return false;
    }

    protected void KeyboardWindowControls()
    {
        if (IsKeyDown(KeyCode.Alpha1) && Event.current.alt)
        {
            EditorWindow.GetWindow(typeof(AudioWindow));
            Event.current.Use();
        }
        if (IsKeyDown(KeyCode.Alpha2) && Event.current.alt)
        {
            EditorWindow.GetWindow(typeof(EventWindow));
            Event.current.Use();
        }
        if (IsKeyDown(KeyCode.Alpha3) && Event.current.alt)
        {
            (EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow).SelectBusCreation();
            Event.current.Use();
        }
        if (IsKeyDown(KeyCode.Alpha4) && Event.current.alt)
        {
            (EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow).SelectBankCreation();
            Event.current.Use();
        }
        if (IsKeyDown(KeyCode.Alpha5) && Event.current.alt)
        {
            (EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow).SelectDataCreation();
            Event.current.Use();
        }

    }

    protected void PostOnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            GUIUtility.keyboardControl = 0;
        }
    }

    protected bool IsKeyDown(KeyCode code)
    {
        return Event.current.type == EventType.keyDown && Event.current.keyCode == code;
    }
}
