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

    [SerializeField]
    private GameObject _NonHelmet;

    [SerializeField]
    private AnimationCurve _headYCurve;
    [SerializeField]
    private float _headYMovement;
    [SerializeField]
    private AnimationCurve _headXCurve;
    [SerializeField]
    private float _headXMovement;

    private Vector3 _speed;

    private float _stepStartTime;
    private bool isMoving;
    private bool canMove = true;

    private bool rightStep = true;

    [SerializeField] private AudioClip[] _collisionClips;


    // Use this for initialization
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving && IsInputPressed() && canMove)
        {
            StartCoroutine(WaitForMove());
        }
        if (isMoving)
        {
            float endTime = _stepStartTime + _stepLength;
            float stepPercentage = 1.0f - ((endTime - Time.time) / _stepLength);
            Vector3 move = moveDir.Mul(_acceleration) * _stepCurve.Evaluate(stepPercentage);
            _speed += move * Time.deltaTime;

            _NonHelmet.transform.localPosition = _NonHelmet.transform.localPosition.Y(_headYCurve.Evaluate(stepPercentage) * _headYMovement);

            float xOffset = _headXCurve.Evaluate(stepPercentage);
            if (!rightStep)
                xOffset = -xOffset;
            _NonHelmet.transform.localPosition = _NonHelmet.transform.localPosition.X(xOffset * _headXMovement);
        }

        if (!isMoving)
        {
            _speed.x = _speed.x - Mathf.Lerp(_speed.x, 0, 0.1f) * Time.deltaTime;
            _speed.z = _speed.z - Mathf.Lerp(_speed.z, 0, 0.1f) * Time.deltaTime;
        }

        if (_speed.magnitude > _maxSpeed)
            _speed = Vector3.ClampMagnitude(_speed, _maxSpeed);

        controller.Move(_speed * Time.deltaTime + Physics.gravity * Time.deltaTime);
    }

    IEnumerator WaitForMove()
    {
        canMove = false;
        isMoving = false;
        if (_speed.magnitude > _stepWaitMinMagnitude)
            yield return new WaitForSeconds(_stepWait);
        isMoving = true;
        _stepStartTime = Time.time;

        Vector3 forward = _NonHelmet.transform.forward;
        Vector3 right = _NonHelmet.transform.right;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir = forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir = -forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += -right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += right;
        }

        yield return new WaitForSeconds(_stepLength);
        rightStep = !rightStep;
        isMoving = false;
        canMove = true;
    }

    bool IsInputPressed()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }


    public void WreckageBump()
    {
        var source = GetComponent<AudioSource>();
        source.clip = _collisionClips.RandomElement();
        source.Play();
    }
}
