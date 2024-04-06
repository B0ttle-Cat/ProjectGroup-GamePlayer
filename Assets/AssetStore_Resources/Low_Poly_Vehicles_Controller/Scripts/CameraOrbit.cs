using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float aimingTime;
    public float cameraDistance = 10f;
    public float cameraFOV = 75f;
    public float aimingCameraFOV = 45f;
    public float aimingCameraDistance = 4f;
    public float MouseSensitivity = 4f;
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 10f;
    public float minRotation = -30f;
    public float maxRotation = 90f;
    public bool stabilizeWeapons;
    public bool CameraDisabled = false;

    private Camera cam;
    private Transform _XForm_Camera;
    private Transform _XForm_Parent;
    private Vector3 _LocalRotation;
    private float _CameraDistance;
    private float _CameraFov;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();

        _XForm_Camera = transform;
        _XForm_Parent = transform.parent;
    }

    void FixedUpdate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        transform.parent.position = target.position;

        if (!CameraDisabled)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

                if (_LocalRotation.y < minRotation)
                    _LocalRotation.y = minRotation;
                else if (_LocalRotation.y > maxRotation)
                    _LocalRotation.y = maxRotation;
            }
            if(Input.GetKey(KeyCode.Mouse1) && stabilizeWeapons)
            {
                _CameraDistance = Mathf.Lerp(_CameraDistance, aimingCameraDistance, aimingTime * Time.deltaTime);
                _CameraFov = Mathf.Lerp(_CameraFov, aimingCameraFOV, aimingTime * Time.deltaTime);
            }
            else
            {
                _CameraDistance = Mathf.Lerp(_CameraDistance, cameraDistance, aimingTime * Time.deltaTime);
                _CameraFov = Mathf.Lerp(_CameraFov, cameraFOV, aimingTime * Time.deltaTime);
            }
        }

        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);

        if(stabilizeWeapons)
            _XForm_Parent.rotation = Quaternion.Lerp(_XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);
        else
            _XForm_Parent.localRotation = Quaternion.Lerp(_XForm_Parent.localRotation, QT, Time.deltaTime * OrbitDampening);

        cam.fieldOfView = _CameraFov;

        if (_XForm_Camera.localPosition.z != _CameraDistance * -1f)
        {
            _XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(_XForm_Camera.localPosition.z, _CameraDistance * -1f, aimingTime * Time.deltaTime));
        }
    }
}