using System.Collections.Generic;
using UnityEngine;

public class CommonDataManager : MonoBehaviour {
    
    [SerializeField]
    private AudioNode AudioRoot;
    [SerializeField]
    private AudioEvent EventRoot;
    [SerializeField]
    private AudioBus BusRoot;
    [SerializeField]
    private AudioBankLink BankLinkRoot;

    public AudioNode AudioTree
    {
        get { return AudioRoot; }
        set { AudioRoot = value; }
    }

    public AudioEvent EventTree
    {
        get { return EventRoot; }
        set { EventRoot = value; }
    }

    public AudioBus BusTree
    {
        get { return BusRoot; }
        set { BusRoot = value; }
    }

    public AudioBankLink BankLinkTree
    {
        get { return BankLinkRoot; }
        set { BankLinkRoot = value; }
    }

    //Only set in runtime
    private List<AudioBank> LoadedBanks = new List<AudioBank>();

    public void BankIsLoaded(AudioBank bank)
    {
        LoadedBanks.Add(bank);
    }

    public AudioBank GetLoadedBank(AudioBankLink link)
    {
        for (int i = 0; i < LoadedBanks.Count; ++i)
        {
            if (LoadedBanks[i].GUID == link.GUID)
            {
                return LoadedBanks[i];
            }
        }
        return null;
    }

    public void Load(bool forceReload = false)
    {
        if (AudioRoot == null || BankLinkRoot == null || BusRoot == null || EventRoot == null || forceReload)
        {
            Component[] audioData;
            Component[] eventData;
            Component[] busData;
            Component[] bankLinkData;
            HDRAudio.SaveAndLoad.LoadManagerData(out audioData, out eventData, out busData, out bankLinkData);
            BusRoot         = CheckData<AudioBus>(busData);
            AudioRoot       = CheckData<AudioNode>(audioData);
            EventRoot       = CheckData<AudioEvent>(eventData);
            BankLinkTree    = CheckData<AudioBankLink>(bankLinkData);
        }
    }

    //Does the components actually exist and does it have a root?
    private T CheckData<T>(Component[] data) where T : Object
    {
        if (data != null && data.Length > 0 && data[0] as T != null)
        {
            return data[0] as T;
        }
        return null; 
    } 

    void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        //Instance = this;
        Load();
    }
}
