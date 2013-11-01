using UnityEngine;
using System.Collections;

public class DiverPlayer : MonoBehaviour
{
    private bool canMove = true;
    private Vector3 moveDir;
    private bool isMoving; 

    private CharacterController controller;

    [SerializeField]
    private AnimationCurve _speedCurve;
    [SerializeField]
    private Vector3 _movementSpeed;
    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private float _stepLength;
    [SerializeField]
    private float _timeBetweenSteps;

    [SerializeField]
    private float _nonStepSpeed;

    private float _stepStartTime;

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
	        float endTime = _stepStartTime + _stepLength;
	        float stepPercentage = (endTime - Time.time) / _stepLength;
            Debug.Log(1 - stepPercentage + "  " + _speedCurve.Evaluate(1 - stepPercentage));
            controller.Move(Vector3.Scale(moveDir, _movementSpeed) * _speedCurve.Evaluate(1 - stepPercentage) * Time.deltaTime + Physics.gravity * Time.deltaTime);
	    }
	    else
	    {
	        Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                move = Vector3.forward * _nonStepSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                move += -Vector3.forward * _nonStepSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                move += -Vector3.right  * _nonStepSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                move += Vector3.right  * _nonStepSpeed;
            }
	        move = Vector3.Scale(move, _movementSpeed)*Time.deltaTime;
	        controller.Move(move + Physics.gravity*Time.deltaTime);
	    }
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
        _stepStartTime = Time.time;
        yield return new WaitForSeconds(_stepLength);
        isMoving = false;
        canMove = true;
    }

    bool isInputPressed()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
