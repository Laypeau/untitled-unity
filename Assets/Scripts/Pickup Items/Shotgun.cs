using UnityEngine;

namespace PickupItem
{
    class Shotgun : UseItem
    {
        //temp
        private Material defaultMaterial;
        public Material reloadMaterial;

        public int clipSize = 1;
        public int clip = 1;
        public float reloadTime = 3f;
        [HideInInspector] public bool reloading = false;
        public float useTime = 0f;
        [HideInInspector] private float lastUsed = 0f;

        /// <summary>
        /// Initial velocity of the bullet's rigidbody
        /// </summary>
        public float bulletSpeed = 30f;
        /// <summary>
        /// The offset from the game object's transform, in local space
        /// </summary>
        public Vector3 bulletSpawnOffset = new Vector3(0f, 0f, 0.4f);
        /// <summary>
        /// How many bulets
        /// </summary>
        public int numberOfBullets = 8;
        /// <summary>
        /// How many degrees will the spread 
        /// </summary>
        /// <summary>
        /// Knockback to apply to the player rigidbody, in this gameobject's local space
        /// </summary>
        public float spread = 3f;
        public Vector3 recoil = new Vector3(0f, 0f, -20f);

        private Rigidbody playerRigidbody;

        void Start()
        {
            defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;
            playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        }

        void Update()
        {
            if ((clip <= 0 || Input.GetKeyDown(KeyCode.R)) && GetComponent<PickupItem>().PickedUp && !reloading)
            {
                gameObject.GetComponent<MeshRenderer>().material = reloadMaterial;
                reloading = true;
                Invoke("Reload", reloadTime);
            }
        }

        public override void Use()
        {
            if (clip > 0 && !reloading)
            {
                if (Time.time >= lastUsed + useTime)
                {
                    lastUsed = Time.time;
                    for (int i = 0; i < numberOfBullets; i++)
                    {
                        GameObject bullet = ObjectPool.Instance.GetBullet();
                        bullet.GetComponent<Rigidbody>().position = transform.position + (CameraFocusTransform.rotation * bulletSpawnOffset);
                        
                        Ray ray = new Ray(CameraFocusTransform.position, CameraFocusTransform.rotation * Vector3.forward);
                        Quaternion _bulletRot;
                        if (Physics.Raycast(ray, out RaycastHit rayhit))
                        {
                            _bulletRot = Quaternion.LookRotation(rayhit.point - (transform.position + (transform.rotation * bulletSpawnOffset)));
                            Debug.DrawRay(transform.position + (transform.rotation * bulletSpawnOffset), _bulletRot * Vector3.forward * 100f, Color.yellow, 3f);
                        }
                        else
                        {
                            _bulletRot = transform.rotation;
                        }
                        bullet.GetComponent<Rigidbody>().velocity = _bulletRot * Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * Vector3.forward * bulletSpeed;
                    }
                    clip -= 1;

                    CameraFocusTransform.GetComponent<CameraController>().AddShakeTrauma(4f);
                    playerRigidbody.AddForce(transform.rotation * recoil, ForceMode.VelocityChange);
                }
            }
        }

        public void Reload()
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            reloading = false;
            clip = clipSize;
        }
    }
}
