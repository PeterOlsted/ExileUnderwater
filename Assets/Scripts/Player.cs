using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player PlayerControl
    {
        get; private set;
    }

    private CharacterController controller;

    [SerializeField] 
    private Vector3 _acceleration;
    [SerializeField]
    private float _maxSpeed;

    private Vector3 accel;

    [SerializeField] private float _stopSpeed;

    void OnEnable()
    {
        PlayerControl = this;
        controller = GetComponent<CharacterController>();
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
            move = -transform.forward*_acceleration.z/2.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += -transform.right * _acceleration.x;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += transform.right * _acceleration.x;
        }

        move = move * Time.deltaTime;
        
        accel += move;
        //move += Physics.gravity * Time.deltaTime;
        if (accel.magnitude > _maxSpeed)
            accel = Vector3.ClampMagnitude(accel, _maxSpeed);
        
        controller.Move(accel + Physics.gravity * Time.deltaTime);
        float moveSpeed = controller.velocity.magnitude / _maxSpeed;


        /*if (move == Vector3.zero)
            accel -= ;*/
        /*/rigidbody.AddRelativeForce(move * Time.deltaTime, ForceMode.Force);

        if (rigidbody.velocity.magnitude > _maxSpeed)
        {
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, _maxSpeed);
        }*/
    }

}
