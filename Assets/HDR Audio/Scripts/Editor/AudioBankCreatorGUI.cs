using HDRAudio;
using HDRAudio.TreeDrawer;
using UnityEditor;
using UnityEngine;

public class AudioBankCreatorGUI : BaseCreatorGUI<AudioBankLink>
{
    public AudioBankCreatorGUI(AuxWindow window) : base(window)
    {}

    private int leftWidth;
    private int height;

    public bool OnGUI(int leftWidth, int height)
    {
        BaseOnGUI();

        var dataManager = InstanceFinder.DataManager;
        if (dataManager != null)
        {
            Undo.ClearSnapshotTarget();
            var root = dataManager.BankLinkTree;
            int id = InstanceFinder.GuiUserPrefs.SelectedBusID;
            var selectedNode = UpdateSelectedNode(root, id);
            InstanceFinder.GuiUserPrefs.SelectedBusID = selectedNode != null ? selectedNode.ID : 0;
        }

        this.leftWidth = leftWidth;
        this.height = height; 

        EditorGUIHelper.DrawColums(DrawLeftSide, DrawRightSide);

        return isDirty;
    }


    private void DrawLeftSide(Rect area)
    {
        Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height - 5));
        DrawSearchBar();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        EditorGUILayout.BeginVertical();

        isDirty |= treeDrawer.DrawTree(InstanceFinder.DataManager.BankLinkTree, treeArea);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawRightSide(Rect area)
    {
        EditorGUILayout.BeginVertical();

        if (SelectedNode != null)
        {
            AudioBankLinkDrawer.Draw(SelectedNode);
        }

        EditorGUILayout.EndVertical();
    }

    protected override bool CanDropObjects(AudioBankLink node, Object[] objects)
    {
        if (node == null || objects == null)
            return false;

        if (objects.Length > 0 && objects[0] as AudioBankLink != null && node.Type != AudioBankTypes.Link)
        {
            return !NodeWorker.IsChildOf(objects[0] as AudioBankLink, node);
        }
        return false;
    }

    protected override bool OnNodeDraw(AudioBankLink node, bool isSelected)
    {
        return GenericTreeNodeDrawer.Draw(node, isSelected);
    }

    protected override void OnDrop(AudioBankLink node, Object[] objects)
    {
        AudioBankLink target = objects[0] as AudioBankLink;
        //Undo.RegisterUndo(new Object[] { node, target, target.Parent}, "Drag and Drop bank move");
        NodeWorker.ReasignNodeParent(target, node);
    }

    protected override void OnContext(AudioBankLink node)
    {
        if (node == null)
            return;
        var menu = new GenericMenu();

        if(node.Type == AudioBankTypes.Folder)
        {
            menu.AddItem(new GUIContent(@"Create Child/Folder"), false, data => CreateBank(node, AudioBankTypes.Folder), node);
            menu.AddItem(new GUIContent(@"Create Child/Bank"), false, data => CreateBank(node, AudioBankTypes.Link), node);
        }
        else if (node.Type == AudioBankTypes.Link) 
        {
            menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
            menu.AddDisabledItem(new GUIContent(@"Create Child/Bank"));
        }

        menu.AddSeparator("");

        if (!node.IsRoot)
            menu.AddItem(new GUIContent(@"Delete"), false, data => { /*DeleteNode(audioBus);*/ }, node);
        else
            menu.AddDisabledItem(new GUIContent(@"Delete"));
        menu.ShowAsContext();
    }

    private void CreateBank(AudioBankLink parent, AudioBankTypes type)
    {
        if (type == AudioBankTypes.Folder)
            AudioBankWorker.CreateFolder(parent.gameObject, parent, GUIDCreator.Create());
        else
            AudioBankWorker.CreateBank(parent.gameObject, parent, GUIDCreator.Create());
    }
}
