using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PlayerControl
{
    public class CharacterControl : MonoBehaviour
    {
        //Grapple
        SpringJoint PlayerSpringJoint;
        private bool ConnectedToRigidBody = false;
        LineRenderer LineRender;
        private Vector3[] linepos = new Vector3[2];

        DoubleTapHandler DoubleTapW = new DoubleTapHandler(KeyCode.W);

        //ObjectReferences (Set in Start)
        private static CameraController CameraControlScript;
        private static Transform CameraFocusTransform;
        public static GameObject MenuController;
        public static Rigidbody PlayerRigidBody;
        public static Transform PlayerTransform;

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

            LineRender = GetComponent<LineRenderer>();
        }

        void Update()
        {
            /*
            //DEBUG
            if (DebugToggle.isOn) 
            {
                DebugMenu.SetActive(true);
                DebugTextMoveVector.text = MoveVector.UnrotatedRaw.ToString() + "\n" + PlayerRigidBody.velocity.ToString();
            }
            else
            {
                DebugMenu.SetActive(false);
            }
            */

            //Reload scene
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            //jump
            if (CheckGrounded() && Input.GetKeyDown(KeyCode.Space))
            {
                PlayerRigidBody.AddForce(new Vector3(0f, MoveVector.JumpForce, 0f), ForceMode.VelocityChange);
            }

            Ray PlayerDirection = new Ray(transform.position, CameraFocusTransform.rotation * Vector3.forward);
            if (Input.GetMouseButtonDown(0) && Physics.SphereCast(PlayerDirection, 0.3f, out RaycastHit RayHit, 40f))
            {
                if (RayHit.rigidbody == null)
                {
                    CreateStaticSpringJoint(out PlayerSpringJoint, RayHit);
                }
                else
                {
                    ConnectedToRigidBody = true;
                    CreateRigidBodySpringJoint(out PlayerSpringJoint, RayHit);
                }
            }
            if (Input.GetMouseButtonUp(0) || ((PlayerRigidBody.position - PlayerSpringJoint.connectedAnchor).magnitude <= 2f && ConnectedToRigidBody == true) )
            {
                ConnectedToRigidBody = false;
                Destroy(PlayerSpringJoint);
            }

            if (TryGetComponent<SpringJoint>(out SpringJoint _))
            {
                //Draw line with linerenderer
                linepos[0] = PlayerRigidBody.transform.position + new Vector3(0f, -0.2f, 0f) + (CameraFocusTransform.rotation * new Vector3(-0.5f, 0f, 0.5f));
                if (ConnectedToRigidBody)
                {
                    linepos[1] = PlayerSpringJoint.connectedBody.gameObject.transform.TransformPoint(PlayerSpringJoint.connectedAnchor);
                }
                else
                {
                    linepos[1] = PlayerSpringJoint.connectedAnchor;
                }
                LineRender.SetPositions(linepos);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerRigidBody.AddForce((CameraFocusTransform.rotation * Vector3.forward) * 45f, ForceMode.VelocityChange);
                }
            }
            else
            {
                linepos[0] = Vector3.zero;
                linepos[1] = Vector3.zero;
                LineRender.SetPositions(linepos);
            }
        }

        void FixedUpdate()
        {
            MoveVector.CalculateRaw();

            if (CheckGrounded())
            {
                MoveVector.AddGroundedForce();
            }
            else
            {
                MoveVector.AddAerialForce();

                if (!TryGetComponent(out SpringJoint _))
                {
                    //apply air friction
                    PlayerRigidBody.AddForce(-Vector3.ClampMagnitude(MoveVector.VelocityXZ, MoveVector.AirFriction), ForceMode.VelocityChange);
                }
            }

        }

        public static class MoveVector
        {
            public static Vector3 UnrotatedRaw;
            public static Vector3 RotatedRaw;
            public static Vector3 VelocityXZ;
            public static float MaxMoveMagnitude = 15f;
            public static float JumpForce = 13f;
            public static float GroundFriction = 1.7f;
            public static float GroundMoveMultiplier = 2.6f;
            public static float AirFriction = 0.25f;
            public static float AirMoveMultiplier = 0.6f;

            /// <summary>
            /// Calculates normalised and rotated inputs based on raw input axis
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

            public static void AddGroundedForce()
            {
                Vector3 RotatedMultipliedVector = RotatedRaw * GroundMoveMultiplier;

                //Decrement XZ velocity by grounded friction
                PlayerRigidBody.AddForce(-Vector3.ClampMagnitude(VelocityXZ, GroundFriction), ForceMode.VelocityChange);

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

            public static void AddAerialForce()
            {
                //Adds the force rotated by the camera
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
        }

        /// <summary>
        /// Creates a spring joint
        /// </summary>
        public void CreateStaticSpringJoint(out SpringJoint _SpringJoint, RaycastHit _RayHit)
        {
            _SpringJoint = gameObject.AddComponent<SpringJoint>();
            _SpringJoint.autoConfigureConnectedAnchor = false;
            _SpringJoint.spring = 10f;
            _SpringJoint.damper = 10f;
            //anchor on player (local)
            _SpringJoint.anchor = new Vector3(0f, 1f, 0f);
            //anchor on rayhit (world)
            _SpringJoint.connectedAnchor = _RayHit.point;
            _SpringJoint.tolerance = 0f;

            float dist = Mathf.Abs(_RayHit.point.y - PlayerRigidBody.position.y);
            _SpringJoint.minDistance = 0f;
            _SpringJoint.maxDistance = dist - 1.5f;
        }

        public void CreateRigidBodySpringJoint(out SpringJoint _SpringJoint, RaycastHit _RayHit)
        {
            _SpringJoint = gameObject.AddComponent<SpringJoint>();
            _SpringJoint.autoConfigureConnectedAnchor = false;
            _SpringJoint.spring = 5f;
            _SpringJoint.damper = 10f;
            _SpringJoint.enableCollision = true;
            //anchor on player (local to player rigidbody)
            _SpringJoint.anchor = new Vector3(0f, 1f, 0f);
            //anchor on rayhit (local to connected rigidbody)
            _SpringJoint.connectedBody = _RayHit.rigidbody;
            _SpringJoint.connectedAnchor = _RayHit.rigidbody.gameObject.transform.InverseTransformPoint(_RayHit.point);
            _SpringJoint.tolerance = 0.01f;
            _SpringJoint.minDistance = 0f;
            _SpringJoint.maxDistance = _RayHit.distance;
            _SpringJoint.breakForce = Mathf.Infinity;
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

