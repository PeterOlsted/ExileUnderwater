using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SignalMaster : MonoBehaviour
{
    public static SignalMaster Master { get; private set; }

    [SerializeField]
    private List<SignalStation> Stations;

    [SerializeField]
    private Spotlight _spotlight;
    private int stationIndex;

    [SerializeField]
    private float _waitTimeToShow;

    public void OnEnable()
    {
        Master = this;
    }

    public void ActivateNextStation()
    {
        stationIndex += 1;
        if (stationIndex < Stations.Count)
        {
            _spotlight.Activate();
            Stations[stationIndex].Activate();
        }
        else
            stationIndex -= 1;
    }

    public SignalStation GetNextToActivate()
    {
        if (stationIndex + 1 >= Stations.Count)
        {
            return null;
        }
        return Stations[stationIndex + 1];
    }

    public SignalStation GetLastActivated()
    {
        if (stationIndex >= Stations.Count)
        {
            return null;
        }
        return Stations[stationIndex];
    }
}
