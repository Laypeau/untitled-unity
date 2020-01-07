using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PlayerControl
{
    public class CharacterControl : MonoBehaviour
    {
        //Player state machine
        public int[] PlayerState = new int[2];
        public enum PlayerStateGrounded
        {
            Idle = 0,
            Crouch = 1,
            Walk = 2,
            Dash = 3,
            Slide = 4,
        }
        public enum PlayerStateAerial
        {
            Grounded = 0,
            InAir = 1,
            Swing = 2,
            Dive = 3,
        }

        //Movement related
        public static KeyCode KeyCodeCrouch = KeyCode.LeftShift;

        DoubleTapHandler DoubleTapW = new DoubleTapHandler(KeyCode.W);

        //ObjectReferences (Set in Start)
        private static CameraController CameraControlScript;
        private static Transform CameraFocusTransform;
        public static GameObject MenuController;
        public static Rigidbody PlayerRigidBody;
        public static Transform PlayerTransform;
        public static Bounds PlayerBounds;

        //DEBUG
        //Remember to set in the inspector
        public Toggle DebugToggle;
        public GameObject DebugMenu;
        public Text DebugTextMoveVector;

        void Start()
        {
            PlayerRigidBody = GetComponent<Rigidbody>();
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
            CameraControlScript = GameObject.Find("CameraFocus").GetComponent<CameraController>();
            MenuController = GameObject.Find("Canvas").GetComponent<GameObject>();
            PlayerTransform = GetComponent<Transform>();
            PlayerBounds = GetComponent<Collider>().bounds;
        }

        void Update()
        {
            //DEBUG
            if (DebugToggle.isOn) 
            {
                DebugMenu.SetActive(true);
                DebugTextMoveVector.text = MoveVector.UnrotatedInput.ToString() + "\n" + (MoveVector.CalculatedXZ + MoveVector.CalculatedY).ToString() + "\n" + PlayerRigidBody.velocity.ToString();
            }
            else
            {
                DebugMenu.SetActive(false);
            }

            //Reload scene
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void FixedUpdate()
        {
            MoveVector.CalculateXZ(PlayerRigidBody.velocity);
            MoveVector.CalculateY(PlayerRigidBody.velocity.y);

            PlayerRigidBody.velocity = MoveVector.CalculatedXZ + MoveVector.CalculatedY;
            
        }

        public static class MoveVector
        {
            public static Vector3 UnrotatedInput = Vector3.zero;
            public static Vector3 RotatedInput = Vector3.zero;
            public static Vector3 CalculatedXZ = Vector3.zero;
            public static Vector3 CalculatedY = Vector3.zero;
            public static float MaxMoveMagnitude = 25f;
            public static float Friction = 1.8f;
            public static float MoveMultiplier = 3f;
            public static float Gravity = -35f;

            /// <summary>
            /// 
            /// </summary>
            public static void CalculateXZ(Vector3 _velocity)
            {
                float _horizontal = Input.GetAxisRaw("Horizontal");
                float _vertical = Input.GetAxisRaw("Vertical");
                Vector3 _input = new Vector3(_horizontal, 0f, _vertical).normalized;

                UnrotatedInput = _input * MoveMultiplier;
                RotatedInput = Quaternion.Euler(0f, CameraFocusTransform.rotation.eulerAngles.y, 0f) * UnrotatedInput;

                CalculatedXZ = Vector3.ClampMagnitude(_velocity - Vector3.ClampMagnitude(_velocity, Friction) + RotatedInput, MaxMoveMagnitude);
            }

            /// <summary>
            /// 
            /// </summary>
            public static void CalculateY(float _VelocityY)
            {
                if (CheckGrounded())
                {
                    CalculatedY.y = 0f;
                }
                else
                {
                    CalculatedY.y = _VelocityY + (-30f * Time.deltaTime);
                    Debug.Log(_VelocityY + (-30f * Time.deltaTime));
                }
            }

        }

        /// <summary>
        /// Sends a spherecast downwards and returns true if it hits anything
        /// </summary>
        public static bool CheckGrounded()
        {
            Ray ray = new Ray(PlayerTransform.position, new Vector3(0f, -1f, 0f));

            if (Physics.SphereCast(ray, 0.5f, 0.8f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///Sets PlayerState[0] to grounded state enums and PlayerState[1] to aerial state enums
        /// </summary>
        private void SetPlayerState()
        {
            if (DoubleTapW.DoubleTapCheck())
            {
                PlayerState[0] = (int)PlayerStateGrounded.Dash;
            }
            else if (Input.GetKey(KeyCodeCrouch))
            {
                PlayerState[0] = (int)PlayerStateGrounded.Crouch;
            }
            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                PlayerState[0] = (int)PlayerStateGrounded.Walk;
            }
            else
            {
                PlayerState[0] = (int)PlayerStateGrounded.Idle;
            }

            /*
            if (Jump.InAir)
            {
                PlayerState[1] = (int)PlayerStateAerial.InAir;
            }
            else
            {
                PlayerState[1] = (int)PlayerStateAerial.Grounded;
            }
            */

            
        }

        /// <summary>
        /// Makes a circle of rays downward around the player. Returns the average position of all the colliders below
        /// </summary>
        public static Vector3 FindFloorAverageDistance(int _NumberofSamples, float _RaycastLength, float _DistFromCentre)
        {
            Vector3 CumulativeFloorPosition = Vector3.zero;

            if (Physics.Raycast(PlayerTransform.position, Vector3.down, out RaycastHit hit, _RaycastLength))
            {
                CumulativeFloorPosition = hit.point;
            }

            //int NumOfRayhits = 0;

            for (int i = 1; i < _NumberofSamples + 1; i++)
            {
                Quaternion rotation = Quaternion.Euler(0f,(360f / _NumberofSamples) * i, 0f);
                Ray ray = new Ray(PlayerTransform.position + (rotation * new Vector3(0f, 0f, _DistFromCentre)), Vector3.down);

                Debug.DrawRay(PlayerTransform.position + (rotation * new Vector3(0f, 0f, _DistFromCentre)), Vector3.down * _RaycastLength, Color.magenta);

                if (Physics.Raycast(ray, out RaycastHit RayHit, _RaycastLength))
                {
                    CumulativeFloorPosition += RayHit.point;
                }
            }

            return CumulativeFloorPosition / _NumberofSamples;
        }
    }

    /// <summary>
    /// An instantiable class that compares the times of the same button being pressed twice
    /// </summary>
    public class DoubleTapHandler
    {
        private float DoubleTapSpeed = 0.5f;
        private bool DoubleTap = false;
        private float TimeOfFirstRelease = 0f;
        private KeyCode InputKeyCode;

        /// <summary>The key that this particular instance will keep track of</summary>
        /// <param name="KeyCode"></param>
        public DoubleTapHandler(KeyCode KeyCode)
        {
            InputKeyCode = KeyCode;
        }

        /// <summary>
        /// Stores the time that the key was pressed when called
        /// </summary>
        /// <returns>
        /// Returns True if a key has been pressed twice within DoubleTapSpeed
        /// </returns>
        public bool DoubleTapCheck()
        {
            if (Input.GetKeyUp(InputKeyCode))
            {
                DoubleTap = false;
                TimeOfFirstRelease = Time.time;
            }

            if (Input.GetKeyDown(InputKeyCode))
            {
                if (Time.time - TimeOfFirstRelease <= DoubleTapSpeed)
                {
                    DoubleTap = true;
                }
            }

            if (DoubleTap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

