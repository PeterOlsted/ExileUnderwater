using UnityEngine;
using System.Collections;

public class SignalStation : MonoBehaviour {

	

    public void Activate()
    {
        Debug.Log("Station activated");
        transform.GetChild(0).gameObject.SetActive(true);    
    }
}
