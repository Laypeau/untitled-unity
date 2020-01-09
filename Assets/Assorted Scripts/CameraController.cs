using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private float CameraOffsetX = 0f;
    private float CameraOffsetY = 0f;
    private float CameraOffsetZ = 0f; //make sure its negative

    private float CameraFocusPosLerpXZ = 1f;
    private float CameraFocusPosLerpY = 0.5f;

    [Range(-10f, 10f)] public float SensitivityX = 5f;
    [Range(-10f, 10f)] public float SensitivityY = -5f;

    //DoCameraShake()
    [Range(0f, 10f)] public float Trauma = 0f;  //keep within 0 and 10
    [HideInInspector] public float TraumaDelta = 0f;
    private readonly float TraumaDecrement = 0.15f;
    private float ShakeSpeed = 10f;

    //Component references
    [SerializeField] private GameObject PlayerGameObject;
    [SerializeField] private Transform CameraFocusTransform;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private Camera CameraCamera;

    private MenuManagementPause MenuManagementPause;

    void Start()
    {
        Initialise();

        CameraTransform.localPosition = new Vector3(CameraOffsetX,0f,CameraOffsetZ);
        CameraFocusTransform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        UpdateCursorLock();
        if (!MenuManagementPause.IsPaused)
        {
            RotateByMouse(CameraFocusTransform);
            MoveToFocus(PlayerGameObject.transform);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TraumaDelta += 5;
        }

        //AngleZoom();
        //CameraBackToCollider(CameraTransform);

    }

    void LateUpdate()
    {
        DoCameraShake();    //Put in lateupdate so any other script wanting to change the trauma value can do it in Update and have the camera rotate the same frame
    }

    /// <summary>
    /// Does the references to CameraFocusTransform, CameraTransform, PlayerGameobject CameraCamera and MenuManagementPause
    /// </summary>
    public void Initialise()
    {
        PlayerGameObject = GameObject.Find("Player");
        CameraFocusTransform = GetComponent<Transform>();
        CameraTransform = CameraFocusTransform.Find("PlayerCamera");
        CameraCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

        MenuManagementPause = GameObject.Find("Canvas").GetComponent<MenuManagementPause>();
    }

    /// <summary>
    /// Locks the cursor depending on IsPaused in the MenuManagementPause script is true or not
    /// </summary>
    public void UpdateCursorLock()
    {
        if (MenuManagementPause.IsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //Maybe use a custom cursor image?
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Lerps the camera focus to the input transform by independent Y and XZ values
    /// </summary>
    public void MoveToFocus(Transform _InputTransform)
    {
        float _CurX = CameraFocusTransform.position.x;
        float _CurY = CameraFocusTransform.position.y;
        float _CurZ = CameraFocusTransform.position.z;

        float _OutX = Mathf.Lerp(_CurX, _InputTransform.position.x, CameraFocusPosLerpXZ);
        float _OutY = Mathf.Lerp(_CurY, _InputTransform.position.y, CameraFocusPosLerpY);
        float _OutZ = Mathf.Lerp(_CurZ, _InputTransform.position.z, CameraFocusPosLerpXZ);

        CameraFocusTransform.position = new Vector3(_OutX, _OutY, _OutZ);
    }

    /// <summary>
    /// Rotates CameraFocusTransform by mouse movement * sensitivity
    /// </summary>
    public void RotateByMouse(Transform _FocusTransform)
    {
        Quaternion TargetPitch = Quaternion.Euler(Input.GetAxis("Mouse Y") * SensitivityY, 0f,0f);
        Quaternion TargetYaw = Quaternion.Euler(0f, Input.GetAxis("Mouse X") * SensitivityX, 0f);

        //On the left means global, on the right means local
        _FocusTransform.rotation = TargetYaw * _FocusTransform.rotation;    //Happens under global co-ords
        _FocusTransform.rotation = _FocusTransform.rotation * TargetPitch;  //Happens under local co-ords
    }

    public void AngleBasedZoom()
    //Zooms the camera in or out depending on the angle from CameraFocusTransform to it
    {
        float angle = CameraFocusTransform.localRotation.eulerAngles.x;

        if (angle > 180) angle -= 360;

        CameraTransform.localPosition = new Vector3(CameraOffsetX, CameraOffsetY, CameraOffsetZ + (CameraOffsetZ * (angle/90)));
    }

    public void CameraBackToCollider(Transform _CameraTransform)
    {
        Physics.Raycast(CameraFocusTransform.position, CameraFocusTransform.rotation * new Vector3(0f, 0f, CameraOffsetZ), out RaycastHit RayHit);
        //Debug.DrawRay(CameraFocusTransform.position, CameraFocusTransform.rotation * new Vector3(0f, 0f, CameraOffsetZ), Color.red);

        //If colliding with "Terrain" tag, move _CameraTransform to the distance of the collision
        if (RayHit.collider.CompareTag("Terrain") && RayHit.distance < Mathf.Abs(CameraOffsetZ))
        {
            _CameraTransform.localPosition = new Vector3(CameraOffsetX, CameraOffsetY, -RayHit.distance + 0.1f); //change the float
        }
        else
        {
            _CameraTransform.localPosition = new Vector3(CameraOffsetX, CameraOffsetY, CameraOffsetZ);
        }
            
    }

    public void DoCameraShake()
    //Adds TraumaDelta to Trauma
    //Shakes the camera exponentially proportional to Trauma, within the bounds of +-ShakeMaxZ
    //Then it decrements Trauma by TraumaDecrement * Timescale
    {
        Trauma = Mathf.Clamp(Trauma + TraumaDelta, 0f, 10f);
        TraumaDelta = 0;

        //ranges from -0.5 to 0.5
        float _ShakeX = (Mathf.Pow(Trauma, 2f) / 100) * (Mathf.PerlinNoise(69f, Time.time * ShakeSpeed) - 0.5f);
        float _ShakeY = (Mathf.Pow(Trauma, 2f) / 100) * (Mathf.PerlinNoise(420f, Time.time * ShakeSpeed) - 0.5f);

        CameraTransform.localPosition = new Vector3(_ShakeX, _ShakeY, 0f);

        if (Trauma - TraumaDecrement * Time.timeScale <= 0)
        {
            Trauma = 0;
        }
        else
        {
            Trauma -= TraumaDecrement * Time.timeScale;
        }
    }
}