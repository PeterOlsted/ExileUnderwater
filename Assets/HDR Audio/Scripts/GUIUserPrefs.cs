using UnityEngine;

public class GUIUserPrefs : MonoBehaviour {

    [SerializeField]
    int _selectedAudioNodeID;
    public int SelectedAudioNodeID
    {
        get
        {
            return _selectedAudioNodeID;
        }
        set
        {
            _selectedAudioNodeID = value;
        }
    }

    [SerializeField]
    int _selectedEventID;
    public int SelectedEventID
    {
        get
        {
            return _selectedEventID;
        }
        set
        {
            _selectedEventID = value;
        }
    }

    [SerializeField]
    int _selectedBusID;
    public int SelectedBusID
    {
        get
        {
            return _selectedBusID;
        }
        set
        {
            _selectedBusID = value;
        }
    }

    [SerializeField]
    int _selectedBankLinkID;
    public int SelectedBankLinkID
    {
        get
        {
            return _selectedBankLinkID;
        }
        set
        {
            _selectedBankLinkID = value;
        }
    }
}
