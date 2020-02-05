using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private float HeadOffset = 0.75f;

    private float CameraFocusPosLerpXZ = 1f;
    private float CameraFocusPosLerpY = 0.75f;

    [Range(-10f, 10f)] public float SensitivityX = 5f;
    [Range(-10f, 10f)] public float SensitivityY = -5f;

    //Camera Bounds
    /// <summary>
    /// It's inverted from what you'd think for some reason. ¯\_(ツ)_/¯
    /// </summary>
    public float XRotation = 0f;
    public float YRotation = 0f;

    //DoCameraShake()
    /// <summary>
    /// Automatically clamps itself between 0 and 10
    /// </summary>
    [Range(0f, 10f)] public float Trauma = 0f;
    [HideInInspector] public float TraumaDelta = 0f;
    private readonly float TraumaDecrement = 0.15f;
    private float ShakeSpeed = 10f;

    //Component references
    private GameObject PlayerGameObject;
    private Transform CameraFocusTransform;
    private Transform CameraTransform;

    private MenuManagementPause MenuManagementPause;

    void Start()
    {
        Initialise();
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
    public void MoveToFocus(Transform _HeadPosition)
    {
        float _OutX = Mathf.Lerp(CameraFocusTransform.position.x, _HeadPosition.position.x, CameraFocusPosLerpXZ);
        float _OutY = Mathf.Lerp(CameraFocusTransform.position.y, _HeadPosition.position.y + (HeadOffset * PlayerControl.MoveVector.CrouchScaleMultiplier), CameraFocusPosLerpY);
        float _OutZ = Mathf.Lerp(CameraFocusTransform.position.z, _HeadPosition.position.z, CameraFocusPosLerpXZ);

        CameraFocusTransform.position = new Vector3(_OutX, _OutY, _OutZ);
    }

    /// <summary>
    /// Rotates CameraFocusTransform by mouse movement * sensitivity
    /// </summary>
    public void RotateByMouse(Transform _FocusTransform)
    {
        XRotation += Input.GetAxis("Mouse Y") * SensitivityY;  //Up is negative, Down is positive. ¯\_(ツ)_/¯
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);

        YRotation += Input.GetAxis("Mouse X") * SensitivityX;

        _FocusTransform.rotation = Quaternion.Euler(XRotation, YRotation, 0f);
    }

    /// <summary>
    /// Adds TraumaDelta to Trauma. 
    /// Shakes the camera exponentially proportional to Trauma, within the bounds of +-ShakeMaxZ. 
    /// Then it decrements Trauma by TraumaDecrement * Timescale.
    /// </summary>
    public void DoCameraShake()

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