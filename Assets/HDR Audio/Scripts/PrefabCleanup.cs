using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// This is a script that cleans up the audio nodes prefab when you exit playmode. 
/// It is a workaround until Unity support undo/redo adding components to a prefab. 
/// </summary>
[ExecuteInEditMode]
public class PrefabCleanup : MonoBehaviour {
	// Use this for initialization
	void Start ()
	{
	    Object.DontDestroyOnLoad(gameObject);

	}


	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        //Debug.Log(Application.isLoadingLevel);
        if (Application.isEditor && !Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
          
            Debug.Log("Application quit!");
            EditorUtility.DisplayDialog("hi", "Destroy", "deal");
        }
    }
}
