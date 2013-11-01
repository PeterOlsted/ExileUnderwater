using UnityEngine;
using System.Collections;

public class PlayerSignal : MonoBehaviour
{
    [SerializeField]
    private AudioEvent SignalEvent;

    [SerializeField]
    private float _activationRadius;
    [SerializeField]
    private float _activationDelay;


    private bool waiting;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.E))
	    {
	        HDRSystem.PostEvent(gameObject, SignalEvent);
	        var signalStation = SignalMaster.Master.GetNext();
	        if (Vector3.Distance(signalStation.transform.position, transform.position) < _activationRadius && !waiting)
	        {
	            waiting = true;
                Debug.Log("yes");
	            StartCoroutine(ActivateNext());
	        }
	    }
	}

    IEnumerator ActivateNext()
    {
        yield return new WaitForSeconds(_activationDelay);
        Debug.Log("co signal");
        waiting = false;
        SignalMaster.Master.ActivateNextStation();
    }
}
