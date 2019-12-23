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
        public MovementVectorHandler MoveVector = new MovementVectorHandler();
        private static float MoveMultiplierInterpolate = 0.5f;      //MoveMultiplier lerps between different movement speeds by this
        public static KeyCode KeyCodeCrouch = KeyCode.LeftShift;
        public static KeyCode KeyCodeJump = KeyCode.Space;
        private static float GravityMultiplier = 4f;
        public static float JumpForce = 20f;
        public static float SlopeMagic = -15f;      //Stick to downward slopes. Remember to change i
        public static bool JumpCancellingEnabled = false;       //Kinda clunky
        private CollisionFlags CollisionFlags;

        private JumpHandler Jump = new JumpHandler();

        private DoubleTapHandler DoubleTapW = new DoubleTapHandler(KeyCode.W);

        //ObjectReferences (Set in Start)
        private static CharacterController CharacterController;
        private static CameraController CameraControlScript;
        private static Transform CameraFocusTransform;
        private static GameObject MenuController;

        //DEBUG
        //Remember to set in the inspector
        public Toggle DebugToggle;
        public GameObject DebugMenu;
        public Text DebugTextMoveVector;


        void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            CameraFocusTransform = GameObject.Find("CameraFocus").GetComponent<Transform>();
            CameraControlScript = GameObject.Find("CameraFocus").GetComponent<CameraController>();
            MenuController = GameObject.Find("Canvas").GetComponent<GameObject>(); 
        }
        
        void Update()
        {

            //DEBUG
            if (DebugToggle.isOn) 
            {
                DebugMenu.SetActive(true);
                DebugTextMoveVector.text = MoveVector.Unrotated.ToString();
            }
            else
            {
                DebugMenu.SetActive(false);
            }
            
            SetPlayerState();

            MoveVector.CalculateRaw();

            MoveVector.CalculateRotated(CameraFocusTransform.rotation.eulerAngles.y);

            //MoveVector.y = Jump.SetJumpInput(MoveVector.y);

            CollisionFlags = CharacterController.Move(MoveVector.Rotated * Time.deltaTime);
        }

        void OnTriggerEnter(Collider TriggerCollider)
        //Reload scene if fallen offstage
        {
            if (TriggerCollider.tag == "Kill")  //organise tag names
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

            if (Jump.InAir)
            {
                PlayerState[1] = (int)PlayerStateAerial.InAir;
            }
            else
            {
                PlayerState[1] = (int)PlayerStateAerial.Grounded;
            }

            
        }

        /// <summary>
        /// Handles everything related to jumping
        /// </summary>
        public class JumpHandler
        {
            public bool InAir = false;
            public bool WasGrounded = true;
            public float TerminalVelocity = 90f;
            public int MaxMidairJumps = 1;   //This is the important variable
            private int _CurrentMidairJumps = 1;
            public int CurrentMidairJumps
            //adds a set property that doesn't allow for invalid CurrentMidairJumps
            {
                get
                {
                    return _CurrentMidairJumps;
                }
                set
                {
                    if (value > MaxMidairJumps || value < 0)
                    {
                        _CurrentMidairJumps = MaxMidairJumps;
                    }
                    else
                    {
                        _CurrentMidairJumps = value;
                    }
                }
            }

            public float SetJumpInput(float _VectorY)    //Could probably use out or ref keywords, but I don't know how
            //Takes an input and uses it to find if it should be replaced by a jump force
            {
                if (CharacterController.isGrounded)
                {
                    if (Input.GetKeyDown(KeyCodeJump))
                    {
                        //Do things related to jumping off ground
                        WasGrounded = false; //Don't break the jump
                        InAir = true;
                        return JumpForce;
                    }
                    else
                    {
                        if (!WasGrounded) //Do things related to just landing on the ground
                        {
                            WasGrounded = true;
                            CurrentMidairJumps = MaxMidairJumps;    //Add VFX
                            InAir = false;
                            //CameraControlScript.TraumaDelta += Mathf.Abs(MoveVector.Unrotated.y)/10;
                        }
                        //Do things every frame related to being on ground
                        return SlopeMagic;
                    }
                }
                else
                {
                    if (CurrentMidairJumps > 0 && Input.GetKeyDown(KeyCodeJump))
                    {
                        CurrentMidairJumps -= 1;
                        //Do things related to jumping midair
                        WasGrounded = false;
                        return JumpForce;
                    }

                    //If pressing jump key AND not _VectorY is between 0 and JumpForce-2 AND JumpCancellingEnabled
                    if (Input.GetKeyUp(KeyCodeJump) && _VectorY > 0 && _VectorY < JumpForce-2 && JumpCancellingEnabled)    //Use predefined bounds
                    {
                        WasGrounded = false;
                        return 0;
                    }

                    if (WasGrounded)
                    {
                        WasGrounded = false;
                        return 0f;
                    }
                    else
                    {
                        if (_VectorY >= TerminalVelocity)
                        {
                            return TerminalVelocity;
                        }
                        else
                        {
                            return _VectorY + (Physics.gravity.y * GravityMultiplier * Time.deltaTime);
                        }
                    }
                }
            }

            //public static void PlayJumpSound()
        }
    }

    /// <summary>
    /// Contains the movement vector and all it's components
    /// </summary>
    public class MovementVectorHandler
    {
        public Vector3 Unrotated = Vector3.zero;
        public Vector3 Rotated = Vector3.zero;

        public static float CrouchSpeed = 10f;
        public static float WalkSpeed = 20f;
        public static float DashSpeed = 35f;

        /// <summary>
        /// Calcucates the unrotated XZ vector from input axis
        /// </summary>
        public void CalculateRaw()
        {
            float MaxMoveMagnitude = 20f;
            //float MoveMultiplierWalk = 10f;
            //float MoveMultiplierCrouch = 5f;
            float friction = 1.8f;

            float _horizontal = Input.GetAxisRaw("Horizontal");
            float _vertical = Input.GetAxisRaw("Vertical");
            Vector3 _input = new Vector3(_horizontal, 0f, _vertical).normalized;

            //Apply friction
            //Changing MoveVector directly might lead to some jank
            if (_input.x == 0 && Unrotated.x != 0)
            {
                if (Mathf.Abs(Unrotated.x) - friction < 0)
                {
                    //_input.x = -1 * MoveVector.x;
                    Unrotated.x = 0f;
                }
                else
                {
                    //_input.x = Mathf.Sign(MoveVector.x) * -1 * friction;
                    Unrotated.x += Mathf.Sign(Unrotated.x) * -1 * friction;
                }
            }
            if (_input.z == 0 && Unrotated.z != 0)
            {
                if (Mathf.Abs(Unrotated.z) - friction < 0)
                {
                    //_input.z = -1 * MoveVector.y;
                    Unrotated.z = 0f;
                }
                else
                {
                    //_input.z = Mathf.Sign(MoveVector.z) * -1 * friction;
                    Unrotated.z += Mathf.Sign(Unrotated.z) * -1 * friction;
                }
            }

            Unrotated = Vector3.ClampMagnitude((_input * 3f) + Unrotated, MaxMoveMagnitude);
        }

        /// <summary>
        /// Rotates the raw input vector along y axis by the input and stores the result in Rotated
        /// </summary>
        public void CalculateRotated(float Rotation)
        {
            Rotated = Quaternion.Euler(0f, Rotation, 0f) * Unrotated;
        }
    }

    /// <summary>
    /// An instantiable class that compares the times of the same button being pressed twice
    /// </summary>
    public class DoubleTapHandler
    {
        private float DoubleTapSpeed = 0.5f;
        private bool DoubleTap = false;
        private float TimeOfFirstPress = 0f;
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
                TimeOfFirstPress = Time.time;
            }

            if (Input.GetKeyDown(InputKeyCode))
            {
                if (Time.time - TimeOfFirstPress <= DoubleTapSpeed)
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

