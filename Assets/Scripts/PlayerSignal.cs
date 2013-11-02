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

    private bool canSignal = true;
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.E))
	    {
	        if (canSignal)
	        {
	            canSignal = false;
	            HDRSystem.PostEvent(gameObject, SignalEvent);
	            StartCoroutine(SignalReset());
	        }
	        var signalStation = SignalMaster.Master.GetLastActivated();
	        if (Vector3.Distance(signalStation.transform.position, transform.position) < _activationRadius && !waiting)
	        {
	            waiting = true;
                
	            StartCoroutine(ActivateNext());
	        }
	    }
	}

    IEnumerator SignalReset()
    {
        yield return new WaitForSeconds(6);
        canSignal = true;
    }

    IEnumerator ActivateNext()
    {
        yield return new WaitForSeconds(1);
        //(Object.FindObjectOfType(typeof(DiverPlayer)) as DiverPlayer).Presentation();
        yield return new WaitForSeconds(_activationDelay);
        waiting = false;
        SignalMaster.Master.ActivateNextStation();
    }
}
