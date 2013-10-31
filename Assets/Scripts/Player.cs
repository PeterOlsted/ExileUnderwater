using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    //private CharacterController controller;

    [SerializeField] 
    private Vector3 _acceleration;
    [SerializeField]
    private float _maxSpeed;

    void Awake()
    {
        //controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move = transform.forward * _acceleration.z;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move = -transform.forward * _acceleration.z;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += -transform.right * _acceleration.x;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += transform.right * _acceleration.x;
        }

        //move = Vector3.Scale(move, _acceleration * Time.deltaTime);

        rigidbody.AddRelativeForce(move * Time.deltaTime, ForceMode.Force);

        if (rigidbody.velocity.magnitude > _maxSpeed)
        {
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, _maxSpeed);
        }
    }
}
