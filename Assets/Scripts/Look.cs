using UnityEngine;
using System.Collections;

public class Look : MonoBehaviour
{

    [SerializeField] 
    private float _xSpeed;
    [SerializeField]
    private float _ySpeed;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float lookSpeed;

    float rotationY = 0F;

    void Update()
    {
        var smooth = GetComponent<SmoothLookAt>();
        

        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _xSpeed;

            rotationY += Input.GetAxis("Mouse Y") * _ySpeed;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

            //smooth.target = new Vector3(-rotationY, rotationX, 0);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        { 
            transform.Rotate(0, Input.GetAxis("Mouse X") * _xSpeed, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * _ySpeed;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }
}
