using System;
using System.Collections.Generic;
using System.Linq;
using HDRAudio;
using HDRAudio.TreeDrawer;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AudioEventCreatorGUI : BaseCreatorGUI<AudioEvent>
{
    public AudioEventCreatorGUI(EventWindow window)
        : base(window)
    {
    }

    private int leftWidth;
    private int height;

    public bool OnGUI(int leftWidth, int height)
    {
        BaseOnGUI();

        var root = InstanceFinder.DataManager.EventTree;
        int id = InstanceFinder.GuiUserPrefs.SelectedEventID;
        var selectedNode = UpdateSelectedNode(root, id);
        InstanceFinder.GuiUserPrefs.SelectedEventID = selectedNode != null ? selectedNode.ID : 0;

        this.leftWidth = leftWidth;
        this.height = height;


        EditorGUIHelper.DrawColums(DrawLeftSide, DrawRightSide);


        return isDirty;
    }

    private void DrawLeftSide(Rect area)
    {
        Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height ));
        DrawSearchBar();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        EditorGUILayout.BeginVertical();

        isDirty |= treeDrawer.DrawTree(InstanceFinder.DataManager.EventTree, treeArea);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawRightSide(Rect area)
    {
        EditorGUILayout.BeginVertical();

        if (SelectedNode != null)
        {
            isDirty |= AudioEventDrawer.Draw(SelectedNode);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            /*
                        if (GUILayout.Button("PlayAtPosition"))
                        {
                            //FindAudioPlayer();
                            //audioPlayer.PlayAtPosition(SelectedAudioNode);
                        }
                        if (GUILayout.Button("Stop"))
                        {
                            //audioPlayer.StoppedNode();
                        }
                        if (GUILayout.Button("Pause"))
                        {
                            //audioPlayer.Pause();
                        }*/
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    public override void OnEnable()
    {
        treeDrawer.OnNodeDraw = EventDrawer.EventFoldout;
        treeDrawer.OnContext = OnContext;
        treeDrawer.CanDropObjects = CanDropObjects;
        treeDrawer.OnDrop = OnDrop;
    }

    protected override void OnDrop(AudioEvent audioevent, Object[] objects)
    {
        AudioEventWorker.OnDrop(audioevent, objects);
    }

    protected override bool CanDropObjects(AudioEvent audioEvent, Object[] objects)
    {
        return AudioEventWorker.CanDropObjects(audioEvent, objects);
    }



    protected override void OnContext(AudioEvent node)
    {
        var menu = new GenericMenu();

        #region Duplicate
        if (!node.IsRoot)
            menu.AddItem(new GUIContent("Duplicate"), false, data => AudioEventWorker.Duplicate(data as AudioEvent), node);
        else
            menu.AddDisabledItem(new GUIContent("Duplicate"));
        #endregion

        menu.AddSeparator("");

        #region Create child

        if (node.Type == EventNodeType.Root)
        {
            menu.AddItem(new GUIContent(@"Create Child/Folder"), false, data => { CreateChild(node, EventNodeType.Folder); }, node);
            menu.AddItem(new GUIContent(@"Create Child/Event Group"), false, data => { CreateChild(node, EventNodeType.EventGroup); }, node);
            menu.AddDisabledItem(new GUIContent(@"Create Child/Event")); 
        }
        if (node.Type == EventNodeType.Folder)
        {
            menu.AddItem(new GUIContent(@"Create Child/Folder"), false, data => { CreateChild(node, EventNodeType.Folder); }, node);
            menu.AddItem(new GUIContent(@"Create Child/Event Group"), false, data => { CreateChild(node, EventNodeType.EventGroup); }, node);
            menu.AddItem(new GUIContent(@"Create Child/Event"), false,
                data => { CreateChild(node, EventNodeType.Event); }, node);
        }
        if (node.Type == EventNodeType.EventGroup)
        {
            menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Event Group"));
            menu.AddItem(new GUIContent(@"Create Child/Event"), false,
                data => { CreateChild(node, EventNodeType.Event); }, node);
        }
        if (node.Type == EventNodeType.Event)
        {
            menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Event Group"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Event"));
        }

        #endregion

        menu.AddSeparator("");

        menu.AddItem(new GUIContent(@"Delete"), false, data => AudioEventWorker.DeleteNode(node), node);

        menu.ShowAsContext();
    }
     
    private void CreateChild(AudioEvent node, EventNodeType type)
    {
        AudioEventWorker.CreateNode(node, type);
        node.FoldedOut = true;
    }
}
