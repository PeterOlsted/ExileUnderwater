using UnityEngine;
using System.Collections;

public class SignalStation : MonoBehaviour
{
    [SerializeField] private GameObject _toActivate;

    public void Activate()
    {
        
        _toActivate.SetActive(true);    
    }
}
