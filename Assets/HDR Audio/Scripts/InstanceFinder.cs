using UnityEngine;

public class InstanceFinder : MonoBehaviour
{
    private static CommonDataManager _dataManager;
    public static CommonDataManager DataManager
    {
        get
        {
            if (_dataManager == null)
            {
                _dataManager = FindObjectOfType(typeof(CommonDataManager)) as CommonDataManager;
                if(_dataManager != null)
                    _dataManager.Load();
            }
            return _dataManager;
        }        
    }

    private static RuntimeAudioData _runtimeAudioData;
    public static RuntimeAudioData RuntimeAudioData
    {
        get
        {
            if (_runtimeAudioData == null)
            {
                _runtimeAudioData = FindObjectOfType(typeof(RuntimeAudioData)) as RuntimeAudioData;
            }
            return _runtimeAudioData;
        }
    }

#if UNITY_EDITOR
    private static GUIUserPrefs _guiUserPref;
    public static GUIUserPrefs GuiUserPrefs
    {
        get
        {
            if (_guiUserPref == null)
            {
                _guiUserPref = FindObjectOfType(typeof(GUIUserPrefs)) as GUIUserPrefs;

            }
            return _guiUserPref;
        }
    }
#endif
}
