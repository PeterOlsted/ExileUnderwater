using UnityEngine;
using System.Collections;

public class LandTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource _land;
    private bool hasTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject.GetComponent<TerrainCollider>() != null)
        {
            _land.Play();
            hasTriggered = true;
        }
        
    }
}
