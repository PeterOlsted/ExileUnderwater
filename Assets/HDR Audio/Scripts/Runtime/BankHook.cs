using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BankHook : MonoBehaviour
{
    [BankHookAttribute("Load On Enable")]
    public List<AudioBankLink> OnEnableEvents = new List<AudioBankLink>();
    [BankHookAttribute("Load On Start")]
    public List<AudioBankLink> OnStartEvents = new List<AudioBankLink>();
    [BankHookAttribute("Load On Disable")]
    public List<AudioBankLink> OnDisableEvents = new List<AudioBankLink>();
    [BankHookAttribute("Load On Destroy")]
    public List<AudioBankLink> OnDestroyEvents = new List<AudioBankLink>();

    [BankHookAttribute("Unload On Enable")]
    public List<AudioBankLink> OnEnableUnloadEvents = new List<AudioBankLink>();
    [BankHookAttribute("Unload On Start")]
    public List<AudioBankLink> OnStartUnloadEvents = new List<AudioBankLink>();
    [BankHookAttribute("Unload On Disable")]
    public List<AudioBankLink> OnDisableUnloadEvents = new List<AudioBankLink>();
    [BankHookAttribute("Unload On Destroy")]
    public List<AudioBankLink> OnDestroyUnloadEvents = new List<AudioBankLink>();

    void OnEnable()
    {
        for (int i = 0; i < OnEnableEvents.Count; ++i)
        {
            HDRSystem.LoadBank(OnEnableEvents[i]);
        }
        for (int i = 0; i < OnEnableUnloadEvents.Count; ++i)
        {
            HDRSystem.UnloadBank(OnEnableUnloadEvents[i]);
        }
    }

    void Start()
    {
        for (int i = 0; i < OnStartEvents.Count; ++i)
        {
            HDRSystem.LoadBank(OnStartEvents[i]);
        }
        for (int i = 0; i < OnStartUnloadEvents.Count; ++i)
        {
            HDRSystem.UnloadBank(OnStartUnloadEvents[i]);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < OnDisableEvents.Count; ++i)
        {
            HDRSystem.LoadBank(OnDisableEvents[i]);
        }
        for (int i = 0; i < OnDisableUnloadEvents.Count; ++i)
        {
            HDRSystem.UnloadBank(OnDisableUnloadEvents[i]);
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < OnDestroyEvents.Count; ++i)
        {
            HDRSystem.LoadBank(OnDestroyEvents[i]);
        }
        for (int i = 0; i < OnDestroyUnloadEvents.Count; ++i)
        {
            HDRSystem.UnloadBank(OnDestroyUnloadEvents[i]);
        }
    }
}
