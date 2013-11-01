using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SignalMaster : MonoBehaviour
{
    public static SignalMaster Master { get; private set; }

    [SerializeField]
    private List<SignalStation> Stations;
    private int stationIndex;

    [SerializeField]
    private float _waitTimeToShow;

    public void OnEnable()
    {
        Master = this;
    }

    public void ActivateNextStation()
    {
        Debug.Log("master activaton");
        Stations[stationIndex].Activate();
        stationIndex += 1;
    }

    public SignalStation GetNext()
    {
        if (stationIndex >= Stations.Count)
        {
            return null;
        }
        return Stations[stationIndex];
    }
}
