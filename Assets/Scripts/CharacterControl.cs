using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace PlayerControl
{
    public class CharacterControl : MonoBehaviour
    {
        //Grapple
        SpringJoint PlayerSpringJoint;
        private float MaxGrappleDistance = 30f;
        public float Spring = 10f; //temp
        public float Damper = 5f;  //temp
        LineRenderer LineRender;
        private Vector3[] linepos = new Vector3[2];

        //ObjectReferences (Set in Start)
        private static CameraController CameraControlScript;
        private static Transform CameraFocusTransform;
        public static GameObject MenuController;
        public static Rigidbody PlayerRigidBody;
        public static Transform PlayerTransform;
        public static CapsuleCollider PlayerCapsuleCollider;
        public static Mesh PlayerCapsuleMesh;
        public static Mesh PlayerCrouchMesh;
        public static Material LineMat;

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
            PlayerCapsuleCollider = GetComponent<CapsuleCollider>();
            PlayerCapsuleMesh = GetComponent<MeshFilter>().mesh;
            PlayerCrouchMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/suzanne.fbx");

            LineRender = GetComponent<LineRenderer>();
            LineRender.useWorldSpace = true;
            LineMat = LineRender.material;

            MoveVector.Initialise(PlayerRigidBody, CameraFocusTransform, PlayerCapsuleCollider);
        }

        void Update()
        {
            //DEBUG
            if (DebugToggle.isOn) 
            {
                DebugMenu.SetActive(true);
                if (TryGetComponent(out SpringJoint _))
                {
                    DebugTextMoveVector.text = "Desired Distance: " + PlayerSpringJoint.maxDistance + "\n" + "Actual Distance: " + Vector3.Distance(PlayerRigidBody.position, linepos[1]);
                }
                else
                {
                    DebugTextMoveVector.text = "";
                }
            }
            else
            {
                //DebugMenu.SetActive(false);
            }

            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            //jump
            if (CheckGrounded() && Input.GetKeyDown(KeyCode.Space))
            {
                MoveVector.ApplyJumpForce();
            }

            // Create SpringJoint
            Ray PlayerDirection = new Ray(CameraFocusTransform.position, CameraFocusTransform.rotation * Vector3.forward);
            if (Input.GetMouseButtonDown(1) && Physics.SphereCast(PlayerDirection, 0.3f, out RaycastHit RayHit, MaxGrappleDistance))
            {
                CreateSpringJoint(out PlayerSpringJoint, RayHit);
            }

            // Destroy SpringJoint
            if (Input.GetMouseButtonUp(1))
            {
                Destroy(PlayerSpringJoint);
            }

            //linerendering
            if (TryGetComponent(out SpringJoint _))
            {
                linepos[0] = PlayerRigidBody.transform.position + new Vector3(0f, 0.69f, 0f) + (CameraFocusTransform.rotation * new Vector3(-0.7f, 0f, 0.7f));

                if (PlayerSpringJoint.connectedBody != null)
                {
                    linepos[1] = PlayerSpringJoint.connectedBody.gameObject.transform.TransformPoint(PlayerSpringJoint.connectedAnchor);
                }
                else
                {
                    linepos[1] = PlayerSpringJoint.connectedAnchor;
                }

                LineRender.SetPositions(linepos);
                LineMat.SetFloat("_Stretch", Mathf.Abs(Vector3.Distance(PlayerRigidBody.position, linepos[1]) - PlayerSpringJoint.maxDistance));
            }
            else
            {
                linepos[0] = Vector3.zero;
                linepos[1] = Vector3.zero;
                LineRender.SetPositions(linepos);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                MoveVector.StartSlide();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                MoveVector.EndSlide();
            }
        }

        void FixedUpdate()
        {
            MoveVector.CalculateRaw();

            if (CheckGrounded())
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MoveVector.ApplySlideFriction();
                }
                else
                {
                    MoveVector.ApplyGroundFriction();
                    MoveVector.ApplyGroundMovement();
                }
            }
            else
            {
                MoveVector.ApplyAirMovement();

                //Only apply aerial friction forces when there is no springjoint, as to not mess with the forces involved
                if (!TryGetComponent(out SpringJoint _))
                {
                    MoveVector.ApplyAirFriction();
                } 
            }
        }


        /// <summary>
        /// Creates a spring joint for grappling
        /// </summary>
        /// <param name="_SpringJoint"> PlayerSpringJoint </param>
        /// <param name="_RayHit"> The ray that hit the target </param>
        public void CreateSpringJoint(out SpringJoint _SpringJoint, RaycastHit _RayHit)
        {
            _SpringJoint = gameObject.AddComponent<SpringJoint>();
            _SpringJoint.autoConfigureConnectedAnchor = false;
            _SpringJoint.spring = Spring;
            _SpringJoint.damper = Damper;
            _SpringJoint.tolerance = 0f;
            _SpringJoint.minDistance = 0f;
            //anchor on player (local)
            _SpringJoint.anchor = new Vector3(0f, 1f, 0f);

            if (_RayHit.rigidbody == null)
            {
                //anchor on rayhit (world)
                _SpringJoint.connectedAnchor = _RayHit.point;
            }
            else
            {
                //anchor on rayhit (local to connected rigidbody)
                _SpringJoint.connectedBody = _RayHit.rigidbody;
                _SpringJoint.connectedAnchor = _RayHit.rigidbody.gameObject.transform.InverseTransformPoint(_RayHit.point);
                _SpringJoint.enableCollision = true;
            }

            if (CameraControlScript.XRotation > 0f && !(_RayHit.rigidbody == null))
            {
                _SpringJoint.maxDistance = _RayHit.distance;
            }
            else
            {
                float DifferenceInY = Mathf.Abs(_RayHit.point.y - PlayerRigidBody.position.y) - 0.5f; // The extra distance is additional feet clearing distance
                _SpringJoint.maxDistance = DifferenceInY;
            }
        }

        /// <summary>
        /// Sends a spherecast downwards and returns true if it hits anything
        /// </summary>
        public static bool CheckGrounded()
        {
            Ray ray = new Ray(PlayerTransform.position, new Vector3(0f, -1f, 0f));

            if (Physics.SphereCast(ray, 0.4f, 1f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Makes a circle of rays downwards around the origin, starting at positive Z, going clockwise. Returns the average position of all the colliders below
        /// </summary>
        /// <param name="_Origin"> The origin that the circle of raycasts will start </param>
        /// <param name="_NumberofSamples"> The number of raycasts to be cast around the origin </param>
        /// <param name="_RaycastLength"> The length of the raycasts to be cast around the origin </param>
        /// <param name="_DistFromCentre"> The radius of the circle the raycasts will form around the centre </param>
        /// <param name="_CastFromOrigin"> Should an additional raycast be cast downward from the origin </param>
        public static Vector3 FindFloorAverageDistance(Vector3 _Origin, int _NumberofSamples, float _RaycastLength, float _DistFromCentre, bool _CastFromOrigin)
        {
            Vector3 CumulativeFloorPosition = Vector3.zero;

            int NumOfRayhits = 0;

            // Ring of raycasts
            for (int i = 1; i < _NumberofSamples + 1; i++)
            {
                Quaternion rotation = Quaternion.Euler(0f,(360f / _NumberofSamples) * i, 0f);
                Ray ray = new Ray(_Origin + (rotation * new Vector3(0f, 0f, _DistFromCentre)), Vector3.down);

                //Debug.DrawRay(PlayerTransform.position + (rotation * new Vector3(0f, 0f, _DistFromCentre)), Vector3.down * _RaycastLength, Color.magenta);

                if (Physics.Raycast(ray, out RaycastHit RayHit, _RaycastLength))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            // Additional centre raycast
            if (_CastFromOrigin)
            {
                if (Physics.Raycast(_Origin, Vector3.down, out RaycastHit RayHit, _RaycastLength))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            return CumulativeFloorPosition / NumOfRayhits;
        }
    }

    public static class MoveVector
    {
        public static Rigidbody PlayerRigidBody;
        public static Transform CameraFocusTransform;
        public static CapsuleCollider PlayerCapsuleCollider;

        public static Vector3 UnrotatedRaw;
        public static Vector3 RotatedRaw;
        public static Vector3 VelocityXZ;
        public static float MaxMoveMagnitude = 12f;

        public static float JumpForce = 10f;
        public static float RatioOfVerticalToNormal = 0.25f;

        public static float GroundFriction = 1f;
        public static float GroundMoveMultiplier = 2.0f;

        public static float AirFriction = 0.3f;
        public static float AirMoveMultiplier = 0.5f;

        public static float SlideFriction = 0.1f;
        public static float SlideBoost = 3f;
        public static float CrouchScaleDefault = 2f;
        public static float CrouchScaleMultiplier = 0.5f;

        /// <summary>
        /// Links the component references to the ones attached to the gameobject
        /// </summary>
        public static void Initialise(Rigidbody _PlayerRigidBody, Transform _CameraFocusTransform, CapsuleCollider _PlayerCapsuleCollider)
        {
            PlayerRigidBody = _PlayerRigidBody;
            CameraFocusTransform = _CameraFocusTransform;
            PlayerCapsuleCollider = _PlayerCapsuleCollider;
        }

        /// <summary>
        /// Calculates normalised input based on raw input axis, both rotated and unrotated. 
        /// Stores the velocity of movement ONLY on the XZ plane.
        /// </summary>
        public static void CalculateRaw()
        {
            float _horizontal = Input.GetAxisRaw("Horizontal");
            float _vertical = Input.GetAxisRaw("Vertical");
            Vector3 _input = new Vector3(_horizontal, 0f, _vertical).normalized;
            UnrotatedRaw = _input;
            RotatedRaw = Quaternion.Euler(0f, CameraFocusTransform.rotation.eulerAngles.y, 0f) * UnrotatedRaw;

            //Store the velocity along the XZ plane
            VelocityXZ = new Vector3(PlayerRigidBody.velocity.x, 0f, PlayerRigidBody.velocity.z);
        }

        /// <summary>
        /// Applies all input based AddForce calls for when on the ground
        /// </summary>
        public static void ApplyGroundMovement()
        {
            Vector3 RotatedMultipliedVector = RotatedRaw * GroundMoveMultiplier;

            //Add rotated input and make sure it doesn't exceed MaxMoveMagnityde
            if ((VelocityXZ + RotatedMultipliedVector).magnitude > MaxMoveMagnitude)
            {
                //VelocityChange so that VelocityMagnitude is equal to MaxMoveMagnitude
                PlayerRigidBody.AddForce(Vector3.ClampMagnitude(VelocityXZ + RotatedMultipliedVector, MaxMoveMagnitude) - VelocityXZ, ForceMode.VelocityChange);
            }
            else
            {
                PlayerRigidBody.AddForce(RotatedMultipliedVector, ForceMode.VelocityChange);
            }
        }

        /// <summary>
        /// Applies all input based AddForce calls for when in the air
        /// </summary>
        public static void ApplyAirMovement()
        {
            Vector3 RotatedMultipliedVector = RotatedRaw * AirMoveMultiplier;

            if ((VelocityXZ + RotatedMultipliedVector).magnitude > MaxMoveMagnitude)
            {
                PlayerRigidBody.AddForce(Vector3.ClampMagnitude(VelocityXZ + RotatedMultipliedVector, MaxMoveMagnitude) - VelocityXZ, ForceMode.VelocityChange);
            }
            else
            {
                PlayerRigidBody.AddForce(RotatedMultipliedVector, ForceMode.VelocityChange);
            }
        }

        /// <summary>
        /// Decrements velocity on the XZ plane by GroundFriction
        /// </summary>
        public static void ApplyGroundFriction()
        {
            PlayerRigidBody.AddForce(-Vector3.ClampMagnitude(VelocityXZ, GroundFriction), ForceMode.VelocityChange);
        }

        /// <summary>
        /// Decrements velocity on the XZ plane by 0.05*VelocityXZ
        /// </summary>
        public static void ApplyAirFriction()
        {
            //PlayerRigidBody.AddForce(-Vector3.ClampMagnitude(VelocityXZ, AirFriction), ForceMode.VelocityChange);
            PlayerRigidBody.AddForce(VelocityXZ * -0.05f, ForceMode.VelocityChange);
        }

        public static void StartSlide()
        {
            PlayerCapsuleCollider.height = CrouchScaleDefault * CrouchScaleMultiplier;

            // Boost on slide start
            if (VelocityXZ.magnitude > 0.5f /* && CheckGrounded()*/) //Slide boost threshold is arbitrarily assigned
            {
                PlayerRigidBody.AddForce(VelocityXZ.normalized * SlideBoost, ForceMode.VelocityChange);
            }
        }

        public static void EndSlide()
        {
            PlayerCapsuleCollider.height = CrouchScaleDefault;
        }

        /// <summary>
        /// Decrements velocity on the XZ plane by SlideFriction
        /// </summary>
        public static void ApplySlideFriction()
        {
            PlayerRigidBody.AddForce(-Vector3.ClampMagnitude(VelocityXZ, SlideFriction), ForceMode.VelocityChange);
            PlayerRigidBody.AddForce(Vector3.down * 5f, ForceMode.Force);
        }

        /// <summary>
        /// Applies jump force vertically and along the floor normal
        /// </summary>
        public static void ApplyJumpForce()
        {
            Ray ray = new Ray(PlayerRigidBody.position, new Vector3(0f, -1f, 0f));
            Physics.Raycast(ray, out RaycastHit hit, 10f);
            PlayerRigidBody.AddForce(Vector3.up * JumpForce * RatioOfVerticalToNormal, ForceMode.VelocityChange);
            PlayerRigidBody.AddForce(hit.normal.normalized * JumpForce * (1f - RatioOfVerticalToNormal), ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// An instantiable class that compares the times of the same button being pressed twice
    /// </summary>
    public class DoubleTapHandler
    {
        private float DoubleTapSpeed = 0.3f;
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

