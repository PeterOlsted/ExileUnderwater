using UnityEditor;
using UnityEngine;
using System.Collections;

public static class SearchHelper  {

    public static void SearchFor(AudioEventAction action)
    {
        var audioAction = action as EventAudioAction;
        if (audioAction != null && audioAction.Node != null)
        {
            EditorWindow.GetWindow<AudioWindow>().Find(audioAction.Node);
        }
        var busAction = action as EventBusAction;
        if (busAction != null && busAction.Bus != null)
        {
            EditorWindow.GetWindow<AuxWindow>().FindBus(busAction.Bus);
        }
        var bankAction = action as EventBankAction;
        if (bankAction != null && bankAction.BankLink != null)
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(bankAction.BankLink);
        }
    }

    public static void SearchFor(AudioBus bus)
    {
        EditorWindow.GetWindow<AuxWindow>().FindBus(bus);
      
    }
}
