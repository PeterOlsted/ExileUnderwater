using UnityEngine;
using System.Collections;

public class DiverPlayer : MonoBehaviour
{
    private Vector3 moveDir;

    private CharacterController controller;

    [SerializeField]
    private float _stepSpeed;
    [SerializeField]
    private float _stepLength;
    [SerializeField]
    private float _stepWait;
    [SerializeField]
    private float _stepWaitMinMagnitude;
    [SerializeField]
    private AnimationCurve _stepCurve;
    [SerializeField]
    private Vector3 _acceleration;
    [SerializeField]
    private float _maxSpeed;

    
    private Vector3 _speed;

    private float _stepStartTime;
    private bool isMoving;
    private bool canMove = true;


	// Use this for initialization
	void Awake ()
	{
	    controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!isMoving && IsInputPressed() && canMove)
	    {
	        StartCoroutine(WaitForMove());
	    }
	    if (isMoving)
	    {
	        float endTime = _stepStartTime + _stepLength;
	        float stepPercentage = 1.0f - ((endTime - Time.time) / _stepLength);
	        //Vector3 move = Vector3.Scale(_acceleration, moveDir);
            Vector3 move = Vector3.one.Mul(moveDir) * _stepSpeed * _stepCurve.Evaluate(stepPercentage);
	        _speed += move.Mul(moveDir) * Time.deltaTime;
	    }
       /* if (Input.GetKey(KeyCode.W))
        {
            Vector3 accel = _stepCurve.Evaluate() 
            _speed += Vector3.Scale(transform.forward, accel) * Time.deltaTime;
            
        }
        if (Input.GetKey(KeyCode.S))
        {
            _speed += Vector3.Scale(-transform.forward, _acceleration)*Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _speed += Vector3.Scale(-transform.right, _acceleration) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _speed += Vector3.Scale(transform.right, _acceleration) * Time.deltaTime;
        }*/

	    if (!isMoving)
	    {
	        _speed.x = _speed.x - Mathf.Lerp(_speed.x, 0, 0.1f) * Time.deltaTime;
            _speed.z = _speed.z - Mathf.Lerp(_speed.z, 0, 0.1f) * Time.deltaTime;
	    }

	    if (_speed.magnitude > _maxSpeed)
	        _speed = Vector3.ClampMagnitude(_speed, _maxSpeed);

	    controller.Move(_speed*Time.deltaTime + Physics.gravity*Time.deltaTime);
	}

     IEnumerator WaitForMove()
     {
         canMove = false;
         isMoving = false;
         if (_speed.magnitude > _stepWaitMinMagnitude)
            yield return new WaitForSeconds(_stepWait);
        isMoving = true;
        _stepStartTime = Time.time;
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
        
        yield return new WaitForSeconds(_stepLength);
        isMoving = false;
         canMove = true;
     }

    bool IsInputPressed()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
