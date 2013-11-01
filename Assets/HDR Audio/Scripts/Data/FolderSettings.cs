using System;
using UnityEngine;

namespace HDRAudio
{
    public static class FolderSettings
    {
        public static string PathResources  = "Assets/HDR Audio/Resources/";
        public static string PathAssets     = "Assets/HDR Audio/Assets/";

        public static string IconPath = "HDR Audio/Icons/";

        public static string PathSaveEditorData     = "Assets/HDR Audio/Assets/HDREditorSave.prefab";
        public static string AudioDataPath          = "Assets/HDR Audio/Resources/HDRAudioSave.prefab";
        public static string EventDataPath          = "Assets/HDR Audio/Resources/HDREventSave.prefab";
        public static string BusDataPath            = "Assets/HDR Audio/Resources/HDRBusSave.prefab";
        public static string BankLinkDataPath       = "Assets/HDR Audio/Resources/HDRBankLinkSave.prefab";
         
        public static string AudioData      = "HDRAudioSave";
        public static string EventData      = "HDREventSave";
        public static string BusData        = "HDRBusSave";
        public static string BankLinkData   = "HDRBankLinkSave";

        
        public static string BankCreateFolder = "/HDR Audio/Resources/Banks/";
        public static string BankDeleteFolder = "/HDR Audio/Resources/Banks/";
        public static string BankSaveFolder = "Assets/HDR Audio/Resources/Banks/";
        public static string BankLoadFolder = "Banks/";

        public static string AudioManagerPath = "Assets/HDR Audio/Prefabs/HDRAudioManager.prefab";


        public static void DeleteFolderContent(string path)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);
            foreach (System.IO.FileInfo file in directory.GetFiles()) 
                file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) 
                subDirectory.Delete(true);
        }
    }
}
