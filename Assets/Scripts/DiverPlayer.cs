using UnityEngine;
using System.Collections;

public class DiverPlayer : MonoBehaviour
{
    private bool canMove = true;
    private Vector3 moveDir;
    private bool isMoving; 

    private CharacterController controller;
    

    [SerializeField]
    private Vector3 _acceleration;
    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private float _stepLength;
    [SerializeField]
    private float _timeBetweenSteps;

	// Use this for initialization
	void Awake ()
	{
	    controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (canMove && isInputPressed())
	    {
	        StartCoroutine(WaitForMove());
	    }

	    if (isMoving)
	    {


            controller.Move(moveDir * Time.deltaTime + Physics.gravity * Time.deltaTime);
	    }
        else
            controller.Move(Physics.gravity * Time.deltaTime);
	}

    IEnumerator WaitForMove()
    {
        isMoving = false;
        canMove = false;
        yield return new WaitForSeconds(_timeBetweenSteps);
        if (Input.GetKey(KeyCode.W))
        {
            moveDir = Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir = -Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += -Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }
        isMoving = true;
        yield return new WaitForSeconds(_stepLength);
        isMoving = false;
        canMove = true;
    }

    bool isInputPressed()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
