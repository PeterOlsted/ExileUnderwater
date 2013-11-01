using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HDRAudio
{
public static class SaveAndLoad  //Only a monobehavior so that Instantiate(...) can be used
{
    private static Component[] GetComponents(GameObject go)
    {
        if (go != null)
        {
            return go.GetComponentsInChildren(typeof(MonoBehaviour), true);
        }
        return null;
    }

    public static void LoadManagerData(out Component[] audioData, out Component[] eventData, out Component[] busData, out Component[] bankLinkData)
    {
        GameObject eventDataGO      = Resources.Load(FolderSettings.EventData) as GameObject;
        GameObject audioDataGO      = Resources.Load(FolderSettings.AudioData) as GameObject;
        GameObject busDataGO        = Resources.Load(FolderSettings.BusData) as GameObject;
        GameObject bankLinkDataGO   = Resources.Load(FolderSettings.BankLinkData) as GameObject;


        busData = GetComponents(busDataGO);
        audioData = GetComponents(audioDataGO);
        eventData = GetComponents(eventDataGO);
        bankLinkData = GetComponents(bankLinkDataGO);
    }

    public static AudioBank LoadAudioBank(int id)
    {
        GameObject bankGO = Resources.Load(FolderSettings.BankLoadFolder + id) as GameObject;
        if(bankGO != null)
        {
            var components = bankGO.GetComponentsInChildren(typeof(AudioBank), true);
            if (components != null && components.Length > 0 && components[0] as AudioBank != null)
            {
                return components[0] as AudioBank;
            }
        }
        Debug.LogWarning("Audio Bank with id " + id + " could not be found");
        return null;
    }

    #if UNITY_EDITOR    
    public static void CreateDataPrefabs(GameObject AudioRoot, GameObject EventRoot, GameObject BusRoot, GameObject BankLinkRoot)
    {
        CreateAudioNodeRootPrefab(AudioRoot);
        CreateAudioEventRootPrefab(EventRoot);
        CreateAudioBusRootPrefab(BusRoot);
        CreateAudioBankLinkPrefab(BankLinkRoot);
    }

    public static void CreateAudioNodeRootPrefab(GameObject root)
    {
        PrefabUtility.CreatePrefab(FolderSettings.AudioDataPath, root);
        Object.DestroyImmediate(root);
    }
    public static void CreateAudioEventRootPrefab(GameObject root)
    {
        PrefabUtility.CreatePrefab(FolderSettings.EventDataPath, root);
        Object.DestroyImmediate(root);
    }
    public static void CreateAudioBusRootPrefab(GameObject root)
    {
        PrefabUtility.CreatePrefab(FolderSettings.BusDataPath, root);
        Object.DestroyImmediate(root);
    }
    public static void CreateAudioBankLinkPrefab(GameObject root)
    {
        PrefabUtility.CreatePrefab(FolderSettings.BankLinkDataPath, root);
        Object.DestroyImmediate(root);
    }

    public static AudioBank CreateAudioBank(int guid)
    {
        GameObject go = new GameObject(guid.ToString());
        var bank = go.AddComponent<AudioBank>();
        bank.GUID = guid;
        
        PrefabUtility.CreatePrefab(FolderSettings.BankSaveFolder + guid + ".prefab", go);
        Object.DestroyImmediate(go);
        return LoadAudioBank(guid);
    }


    #endif
}
}
