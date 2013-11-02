using UnityEngine;
using System.Collections;

public class Wreckage : MonoBehaviour
{
    [SerializeField] 
    private float _cooldownBetweenActivation = 60;

    private bool canBeActivated = true;

    void OnTriggerEnter(Collider collider)
    {
        var diver = collider.gameObject.GetComponent<DiverPlayer>();
        if (canBeActivated && diver != null)
        {
            canBeActivated = false;
               Debug.Log("Hit wreckage");
        }
    }


    IEnumerator Reactivate()
    {
        yield return new WaitForSeconds(_cooldownBetweenActivation);
        canBeActivated = true;
    }
}
