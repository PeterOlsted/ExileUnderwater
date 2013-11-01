using UnityEngine;
using System.Collections;

public class EventBankAction : AudioEventAction
{
    public AudioBankLink BankLink;

    public override string ObjectName
    {
        get
        {
            if (BankLink != null)
                return BankLink.GetName;
            else
            {
                return "Missing Bank";
            }
        }
    }
}
