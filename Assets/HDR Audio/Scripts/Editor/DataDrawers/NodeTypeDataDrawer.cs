using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class NodeTypeDataDrawer
{
    private static GameObject go;
    public static void Draw(AudioNode node)
    {
        if (go == null)
            go = (Resources.FindObjectsOfTypeAll(typeof(HDRSystem))[0] as HDRSystem).gameObject;
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(700));
        NodeTypeData baseData = node.NodeData;
        EditorGUILayout.Separator();
        
        baseData.SelectedArea = GUILayout.Toolbar(baseData.SelectedArea, new[] { "Audio", "Attenuation" });
        EditorGUILayout.Separator(); EditorGUILayout.Separator();

        if (baseData.SelectedArea == 0)
        {
            EditorGUILayout.IntField("ID", node.GUID);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            #region Volume
            baseData.RandomVolume = EditorGUILayout.Toggle("Random volume", baseData.RandomVolume);
            if (!baseData.RandomVolume)
            {
                baseData.MinVolume = EditorGUILayout.Slider("Volume", baseData.MinVolume, 0, 1);
                if (baseData.MinVolume > baseData.MaxVolume)
                    baseData.MaxVolume = baseData.MinVolume + 0.001f;
            }
            else
            {
                EditorGUILayout.MinMaxSlider(new GUIContent("Volume"), ref baseData.MinVolume,
                    ref baseData.MaxVolume, 0, 1);
                baseData.MinVolume = Mathf.Clamp(
                    EditorGUILayout.FloatField("Min volume", baseData.MinVolume), 0, baseData.MaxVolume);
                baseData.MaxVolume = Mathf.Clamp(
                    EditorGUILayout.FloatField("Max volume", baseData.MaxVolume), baseData.MinVolume, 1);
            }
            #endregion

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            #region Parent pitch

            baseData.RandomPitch = EditorGUILayout.Toggle("Random pitch", baseData.RandomPitch);
            float minPitch = 0.001f;
            float maxPitch = 3;
            if (!baseData.RandomPitch)
            {
                baseData.MinPitch = EditorGUILayout.Slider("Pitch", baseData.MinPitch, minPitch, maxPitch);
                if (baseData.MinPitch > baseData.MaxPitch)
                    baseData.MaxPitch = baseData.MinPitch + 0.001f;
                baseData.MaxPitch = Mathf.Clamp(baseData.MaxPitch, minPitch, 3.0f);
            }
            else
            {
                //EditorGUILayout.LabelField("Random pitch between ", baseData.MinPitch + " & " + baseData.MaxPitch);
                EditorGUILayout.MinMaxSlider(new GUIContent("Pitch"), ref baseData.MinPitch, ref baseData.MaxPitch,
                    minPitch, maxPitch);

                baseData.MinPitch = Mathf.Clamp(EditorGUILayout.FloatField("Min pitch", baseData.MinPitch), minPitch,
                    baseData.MaxPitch);
                baseData.MaxPitch = Mathf.Clamp(EditorGUILayout.FloatField("Max pitch", baseData.MaxPitch),
                    baseData.MinPitch, maxPitch);


            }


            #endregion

        
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            #region Delay

            baseData.RandomizeDelay = EditorGUILayout.Toggle("Randomize delay", baseData.RandomizeDelay);
            if (baseData.RandomizeDelay)
            {
                baseData.InitialDelayMin = Mathf.Clamp(
                    EditorGUILayout.FloatField("Min delay", baseData.InitialDelayMin), 0, baseData.InitialDelayMax);
                baseData.InitialDelayMax = Mathf.Clamp(
                    EditorGUILayout.FloatField("Max delay", baseData.InitialDelayMax), baseData.InitialDelayMin, float.MaxValue);
            }
            else
            {
                baseData.InitialDelayMin = EditorGUILayout.FloatField("Initial delay", baseData.InitialDelayMin);
                if (baseData.InitialDelayMin > baseData.InitialDelayMax)
                    baseData.InitialDelayMax = baseData.InitialDelayMin + 0.001f;
            }

            #endregion

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();


            #region Audio bus

            bool overrideParent = EditorGUILayout.Toggle("Override Parent Bus", node.OverrideParentBus);
            EditorGUILayout.BeginHorizontal();
            if (overrideParent != node.OverrideParentBus)
                Undo.RegisterUndo(node, "Override parent");
            node.OverrideParentBus = overrideParent;
            if (!node.OverrideParentBus)
                GUI.enabled = false;

            if (node.Bus != null)
                EditorGUILayout.TextField("Name", node.Bus.Name);
            else
            {
                GUILayout.Label("Missing node");
            }


            EditorGUILayout.BeginHorizontal(GUILayout.Width(175));
            if (GUILayout.Button("Find"))
            {
                SearchHelper.SearchFor(node.Bus);
            }

            EditorGUILayout.BeginVertical();
            GUI.enabled = true;
            GUILayout.Button("Drag bus here to assign");
            EditorGUILayout.EndVertical();

            var buttonArea = GUILayoutUtility.GetLastRect();
            var bus = HandleBusDrag(buttonArea);
            if (bus != null && bus != node.Bus)
            {
                Undo.RegisterUndo(node, "Assign bus");
                node.Bus = bus;
                node.OverrideParentBus = true;
                Event.current.Use();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            #region Loops

            GUILayout.BeginVertical();


            baseData.Loop = EditorGUILayout.Toggle("Loop", baseData.Loop);
            GUI.enabled = baseData.Loop;
            baseData.LoopInfinite = EditorGUILayout.Toggle("Loop infinite", baseData.LoopInfinite);
            if(baseData.Loop)
                GUI.enabled = !baseData.LoopInfinite;
            baseData.RandomizeLoops = EditorGUILayout.Toggle("Randomize loop count", baseData.RandomizeLoops);

            if (!baseData.RandomizeLoops)
            {
                baseData.MinIterations = (byte) Mathf.Clamp(EditorGUILayout.IntField("Loop count", baseData.MinIterations), 0, 255);
            }
            else
            {

                GUILayout.BeginHorizontal();
                baseData.MinIterations =
                    (byte) Mathf.Clamp(EditorGUILayout.IntField("Min Loop count", baseData.MinIterations), 0, 255);
                baseData.MaxIterations =
                    (byte) Mathf.Clamp(EditorGUILayout.IntField("Max Loop count", baseData.MaxIterations), 0, 255);

                baseData.MaxIterations = (byte) Mathf.Clamp(baseData.MaxIterations, baseData.MinIterations, 255);
                baseData.MinIterations = (byte) Mathf.Clamp(baseData.MinIterations, 0, baseData.MaxIterations);

                GUILayout.EndHorizontal();
            }


            GUI.enabled = true;

            GUILayout.EndVertical();

            #endregion
        }
        else
        {
            #region Attenuation
            baseData.OverrideAttenuation = GUILayout.Toggle(baseData.OverrideAttenuation, "Override parent");

            GUI.enabled = baseData.OverrideAttenuation;

            baseData.RolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff", baseData.RolloffMode);

            baseData.MinDistance = EditorGUILayout.FloatField("Min Distance", baseData.MinDistance);
            baseData.MaxDistance = EditorGUILayout.FloatField("Max Distance", baseData.MaxDistance);
            baseData.MinDistance = Mathf.Max(baseData.MinDistance, 0.00001f);
            baseData.MaxDistance = Mathf.Max(baseData.MaxDistance, baseData.MinDistance + 0.01f);

            if (baseData.RolloffMode == AudioRolloffMode.Custom)
            {
                EditorGUILayout.HelpBox(
                    "Unity does not support setting custom rolloff via scripts. Please select Logarithmic or Linear rolloff instead.",
                    MessageType.Error, true);
                GUI.enabled = false;
            }

            /*AudioSource targetComp = go.GetComponent<AudioSource>();

            if (targetComp != null)
            {
                targetComp.minDistance = baseData.MinDistance;
                targetComp.maxDistance = baseData.MaxDistance;
                targetComp.rolloffMode = baseData.RolloffMode;

                if (baseData.RolloffMode != AudioRolloffMode.Custom)
                {
                    EditorGUILayout.BeginScrollView(new Vector2(0, 325), GUIStyle.none, GUIStyle.none,
                        GUILayout.Width(400), GUILayout.Height(350));
                    Rect area = EditorGUILayout.BeginVertical();

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
                    GUI.Button(area, "", new GUIStyle());
                }
            }*/

            #endregion 

            GUI.enabled = true;
        }
        EditorGUILayout.EndVertical();
    }

    private static AudioBus HandleBusDrag(Rect area)
    {
        if (area.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            bool canDropObject = true;
            int clipCount = DragAndDrop.objectReferences.Count(obj => obj is AudioBus);

            if (clipCount != DragAndDrop.objectReferences.Length || clipCount == 0)
                canDropObject = false;

            if (canDropObject)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                {
                    return DragAndDrop.objectReferences[0] as AudioBus;
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
        return null;
    }
}
