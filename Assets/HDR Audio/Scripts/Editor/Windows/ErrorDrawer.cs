using UnityEditor;
using UnityEngine;

namespace  HDRAudio
{
    public static class ErrorDrawer {
        public static void MissingAudioManager()
        {
            EditorGUILayout.HelpBox("The audio manager could not be found in the scene\nClick the \"Fix it for me\" button or drag the prefab found at \"HDR Audio/Prefabs/HDRAudioManager\" from the Project window into the scene", MessageType.Error, true);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fix it"))
            {
                var go = Object.Instantiate(AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject);
                go.name = "HDRAudioManager";
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Find Audio Manager Prefab")) 
            {
                EditorApplication.ExecuteMenuItem("Window/Project");
                var go = AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    EditorGUIUtility.PingObject(go);
                    Selection.objects = new Object[] { go};
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static bool IsDataMissing(CommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingbus = manager.BusTree == null;
            bool missingBankLink = manager.BankLinkTree == null;

            return missingaudio || missingaudioEvent || missingbus || missingBankLink;
        }

        public static bool MissingData(CommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingbus = manager.BusTree == null;
            bool missingBankLink = manager.BankLinkTree == null;

            bool areAnyMissing = missingaudio || missingaudioEvent || missingbus || missingBankLink;

            if (areAnyMissing)
            {
                string missingAudioInfo = missingaudio ? "Missing Audio Data\n" : "";
                string missingEventInfo =  missingaudioEvent ? "Missing Event Data\n" : "";
                string missingBusInfo = missingbus ? "Missing Bus Data\n" : "";
                string missingBankLinkInfo = missingBankLink ? "Missing BankLink Data\n" : "";
                EditorGUILayout.HelpBox(missingAudioInfo + missingEventInfo + missingBusInfo + missingBankLinkInfo + "Please go to the Aux Window and create the missing data",
                    MessageType.Error, true);
                if (GUILayout.Button("Open Aux Window"))
                {
                    EditorWindow.GetWindow<AuxWindow>().SelectDataCreation();
                }
            }
            return areAnyMissing;
        }
        
    }   
}
