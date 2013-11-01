using System;
using System.Linq;
using HDRAudio;
using HDRAudio.TreeDrawer;
using UnityEditor;
using UnityEngine;

public class AudioCreatorGUI : BaseCreatorGUI<AudioNode>
{
    public AudioCreatorGUI(AudioWindow window) : base(window)
    {
        this.window = window;
    }

    private int leftWidth;
    private int height;

    public bool OnGUI(int leftWidth, int height)
    {
        BaseOnGUI();
        var root = InstanceFinder.DataManager.AudioTree;
        int id = InstanceFinder.GuiUserPrefs.SelectedAudioNodeID;
        var selectedNode = UpdateSelectedNode(root, id);
        InstanceFinder.GuiUserPrefs.SelectedAudioNodeID = selectedNode != null ? selectedNode.ID : 0;

        this.leftWidth = leftWidth;
        this.height = height;
    
        EditorGUIHelper.DrawColums(DrawLeftSide, DrawRightSide);
        
        return isDirty;
    }

    private void DrawLeftSide(Rect area) 
    {
        Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height));
        DrawSearchBar();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true); //Why minus 27? trial & error
        EditorGUILayout.BeginVertical(); 

        isDirty |= treeDrawer.DrawTree(window.Manager.AudioTree, treeArea);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawRightSide(Rect area)
    {
        EditorGUILayout.BeginVertical();

        if (SelectedNode != null)
        {
            //UndoHandler.CheckUndo(new UnityEngine.Object[] { SelectedNode, SelectedNode.NodeData});
            //UndoHandler.CheckGUIChange();
            DrawTypeControls(SelectedNode);
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawTypeControls(AudioNode node)
    {
        var type = node.Type;
        if (node.NodeData != null)
        {
            switch (type)
            {
                case AudioNodeType.Audio:
                    AudioDataDrawer.Draw(node);
                    break;
                case AudioNodeType.Sequence:
                    SequenceDataDrawer.Draw(node);
                    break;
                case AudioNodeType.Random:
                    RandomDataDrawer.Draw(node);
                    break;
                case AudioNodeType.Multi:
                    MultiDataDrawer.Draw(node);
                    break;
            }
        } else if (SelectedNode.Type == AudioNodeType.Folder || SelectedNode.Type == AudioNodeType.Root)
        {
            FolderDrawer.Draw(node);
        }
    }

    protected override bool CanDropObjects(AudioNode node, UnityEngine.Object[] objects)
    {
        if (node == null || objects == null)
            return false;

        if (node.Type == AudioNodeType.Voice)
            return false;

        int clipCount = DragAndDrop.objectReferences.Count(p => p is AudioClip);
        int nodeCount = DragAndDrop.objectReferences.Count(p => p is AudioNode);

        if (DragAndDrop.objectReferences.Length == 0)
            return false;

        if (clipCount == objects.Length) //Handle clip count
        {
            if (node.Type == AudioNodeType.Audio && clipCount != 1)
                return false;

            return true;
        } else if (nodeCount == objects.Length) //Handle audio node drag n drop
        {
            if (node.Type == AudioNodeType.Audio) //Can't drop on an audionode as it can't have children
                return false;

            if (node == objects[0] as AudioNode)
                return false;

            return !NodeWorker.IsChildOf(objects[0] as AudioNode, node);
        }
        return false;
    }

    protected override void OnDrop(AudioNode node, UnityEngine.Object[] objects)
    {
        if (objects[0] as AudioNode != null) //Drag N Drop internally in the tree, change the parent
        {
            node.IsFoldedOut = true;
            var nodeToMove = objects[0] as AudioNode;
            var oldBank = AudioBankWorker.GetParentBank(nodeToMove);
            AudioBankLink newBank = null;
            if (nodeToMove.OverrideParentBank)
                newBank = AudioBankWorker.GetParentBank(nodeToMove);
            else
                newBank = AudioBankWorker.GetParentBank(node);
            Undo.RegisterUndo(new UnityEngine.Object[]{node, nodeToMove, nodeToMove.Parent, oldBank, newBank}, "Audio Node Move");
            NodeWorker.ReasignNodeParent(nodeToMove, node);
            AudioBankWorker.MoveNode(nodeToMove, oldBank);
        }
        else if (node.Type != AudioNodeType.Audio) //Create new audio nodes when we drop clips
        {
            for (int i = 0; i < objects.Length; ++i)
            {
                var clip = objects[i] as AudioClip;
                var child = AudioNodeWorker.CreateChild(node, AudioNodeType.Audio);
                var path = AssetDatabase.GetAssetPath(clip);
                try
                {
                    //Try and get the name of the clip. Gets the name and removes the end. Assets/IntroSound.mp3 -> IntroSound
                    int lastIndex = path.LastIndexOf('/') + 1; 
                    child.Name = path.Substring(lastIndex, path.LastIndexOf('.') - lastIndex);
                }
                catch (Exception) //If it happens to be a mutant path. Not even sure if this is possible, but better safe than sorry
                {
                    child.Name = node.Name + " Child";
                }
                
                (child.NodeData as AudioData).EditorClip = clip;
                AudioBankWorker.AddNodeToBank(child, clip);
                Event.current.Use();
            }
        } 
        else //Then it must be an audio clip dropped on an audio node, so assign the clip to that node
        {
            var bank = AudioBankWorker.GetParentBank(node);
            Undo.RegisterUndo(new UnityEngine.Object[] { node, bank.LazyBankFetch, node.NodeData }, "Undo Changing Node In Bank");
            (node.NodeData as AudioData).EditorClip = objects[0] as AudioClip;
            AudioBankWorker.SwapClipInBank(node, objects[0] as AudioClip);
        }
    }

    protected override void OnContext(AudioNode node) 
    {
        var menu = new GenericMenu();

        #region Duplicate
        if (!node.IsRoot)
            menu.AddItem(new GUIContent("Duplicate"), false, data => AudioNodeWorker.Duplicate(node), node);
        else
            menu.AddDisabledItem(new GUIContent("Duplicate"));
        #endregion

        menu.AddSeparator("");

        #region Create child

        if (node.Type == AudioNodeType.Audio || node.Type == AudioNodeType.Voice) //If it is a an audio source, it cannot have any children
        {
            menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Audio"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Random"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Sequence"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Multi"));
            //menu.AddDisabledItem(new GUIContent(@"Create Child/Track"));
            //menu.AddDisabledItem(new GUIContent(@"Create Child/Voice"));
        }
        else
        {
            if (node.Type == AudioNodeType.Root || node.Type == AudioNodeType.Folder)
                menu.AddItem(new GUIContent(@"Create Child/Folder"), false, (obj) => CreateChild(node, AudioNodeType.Folder), node);
            else
                menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
            menu.AddItem(new GUIContent(@"Create Child/Audio"), false,      (obj) =>CreateChild(node, AudioNodeType.Audio), node);
            menu.AddItem(new GUIContent(@"Create Child/Random"), false,     (obj) =>CreateChild(node, AudioNodeType.Random), node);
            menu.AddItem(new GUIContent(@"Create Child/Sequence"), false,   (obj) =>CreateChild(node, AudioNodeType.Sequence), node);
            menu.AddItem(new GUIContent(@"Create Child/Multi"), false,      (obj) =>CreateChild(node, AudioNodeType.Multi), node);
            //menu.AddItem(new GUIContent(@"Create Child/Track"), false,      (obj) => CreateChild(node, AudioNodeType.Track), node);
            //menu.AddItem(new GUIContent(@"Create Child/Voice"), false,      (obj) => CreateChild(node, AudioNodeType.Voice), node);
        }

        #endregion

        menu.AddSeparator("");

        #region Add new parent

        if (node.Parent != null && (node.Parent.Type == AudioNodeType.Folder || node.Parent.Type == AudioNodeType.Root))
            menu.AddItem(new GUIContent(@"Add Parent/Folder"), false, (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Folder), node);
        else
            menu.AddDisabledItem(new GUIContent(@"Add Parent/Folder"));
        menu.AddDisabledItem(new GUIContent(@"Add Parent/Audio"));
        if (node.Parent != null && node.Type != AudioNodeType.Folder)
        {
            menu.AddItem(new GUIContent(@"Add Parent/Random"), false, (obj) =>      AudioNodeWorker.AddNewParent(node, AudioNodeType.Random), node);
            menu.AddItem(new GUIContent(@"Add Parent/Sequence"), false, (obj) =>    AudioNodeWorker.AddNewParent(node, AudioNodeType.Sequence), node);
            menu.AddItem(new GUIContent(@"Add Parent/Multi"), false, (obj) =>       AudioNodeWorker.AddNewParent(node, AudioNodeType.Multi), node);
            //menu.AddItem(new GUIContent(@"Add Parent/Track"), false, (obj) =>       AudioNodeWorker.AddNewParent(node, AudioNodeType.Track), node);
        }
        else
        {
            menu.AddDisabledItem(new GUIContent(@"Add Parent/Random"));
            menu.AddDisabledItem(new GUIContent(@"Add Parent/Sequence"));
            menu.AddDisabledItem(new GUIContent(@"Add Parent/Multi"));
            //menu.AddDisabledItem(new GUIContent(@"Add Parent/Track"));
        }
        //menu.AddDisabledItem(new GUIContent(@"Add Parent/Voice"));

        #endregion

        menu.AddSeparator("");

        #region Convert to

        if (node.Children.Count == 0)
            menu.AddItem(new GUIContent(@"Convert To/Audio"), false, (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Audio), node);
        else
            menu.AddDisabledItem(new GUIContent(@"Convert To/Audio"));
        if(node.Type != AudioNodeType.Root) 
        {
            menu.AddItem(new GUIContent(@"Convert To/Random"), false, (obj) =>      AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Random), node);
            menu.AddItem(new GUIContent(@"Convert To/Sequence"), false, (obj) =>    AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Sequence), node);
            menu.AddItem(new GUIContent(@"Convert To/Multi"), false, (obj) =>       AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Multi), node);
            //menu.AddItem(new GUIContent(@"Convert To/Track"), false, (obj) =>       AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Track), node);
        }
        else
        {
            menu.AddDisabledItem(new GUIContent(@"Convert To/Random"));
            menu.AddDisabledItem(new GUIContent(@"Convert To/Sequence"));
            menu.AddDisabledItem(new GUIContent(@"Convert To/Multi"));
            //menu.AddDisabledItem(new GUIContent(@"Add Parent/Track"));
        }

        /*if (node.Children.Count == 0)
            menu.AddItem(new GUIContent(@"Convert To/Voice"), false, (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Audio), node);
        else
            menu.AddDisabledItem(new GUIContent(@"Convert To/Voice"));*/

        #endregion

        menu.AddSeparator("");

        #region Delete
        if(node.Type != AudioNodeType.Root)
            menu.AddItem(new GUIContent("Delete"), false, obj => AudioNodeWorker.DeleteNode(node), node);
        else
            menu.AddDisabledItem(new GUIContent("Delete"));
        #endregion

        menu.ShowAsContext();
    }

    private void CreateChild(AudioNode parent, AudioNodeType type)
    {
        var newNode = AudioNodeWorker.CreateChild(parent, type);
        if (type == AudioNodeType.Audio)
        {
            AudioBankWorker.AddNodeToBank(newNode, null);
        }
    }

    public void FindAudio(AudioNode node)
    {
        searchingFor = node.GUID.ToString();
        lowercaseSearchingFor = searchingFor.ToLower().Trim();
        treeDrawer.Filter(ShouldFilter);   
    }

    protected override bool OnNodeDraw(AudioNode node, bool isSelected)
    {
        return GenericTreeNodeDrawer.Draw(node, isSelected);
    }

    internal void FindAudio(Func<AudioNode, bool> filter)
    {
        searchingFor = "Finding nodes in bank";
        lowercaseSearchingFor = "Finding nodes in bank";
        treeDrawer.Filter(filter);   
    }
}
