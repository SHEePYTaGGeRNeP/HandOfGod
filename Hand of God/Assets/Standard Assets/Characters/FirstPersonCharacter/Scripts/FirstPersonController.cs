namespace Assets.Standard_Assets.Characters.FirstPersonCharacter.Scripts
{
    using Assets.Standard_Assets.CrossPlatformInput.Scripts;
    using Assets.Standard_Assets.Utility;

    using UnityEngine;

    using Random = UnityEngine.Random;

    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        // Use this for initialization
        private void Start()
        {
            this.m_CharacterController = this.GetComponent<CharacterController>();
            this.m_Camera = Camera.main;
            this.m_OriginalCameraPosition = this.m_Camera.transform.localPosition;
            this.m_FovKick.Setup(this.m_Camera);
            this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval);
            this.m_StepCycle = 0f;
            this.m_NextStep = this.m_StepCycle/2f;
            this.m_Jumping = false;
            this.m_AudioSource = this.GetComponent<AudioSource>();
            this.m_MouseLook.Init(this.transform , this.m_Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            this.RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!this.m_Jump)
            {
                this.m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!this.m_PreviouslyGrounded && this.m_CharacterController.isGrounded)
            {
                this.StartCoroutine(this.m_JumpBob.DoBobCycle());
                this.PlayLandingSound();
                this.m_MoveDir.y = 0f;
                this.m_Jumping = false;
            }
            if (!this.m_CharacterController.isGrounded && !this.m_Jumping && this.m_PreviouslyGrounded)
            {
                this.m_MoveDir.y = 0f;
            }

            this.m_PreviouslyGrounded = this.m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            this.m_AudioSource.clip = this.m_LandSound;
            this.m_AudioSource.Play();
            this.m_NextStep = this.m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            this.GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = this.transform.forward* this.m_Input.y + this.transform.right* this.m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(this.transform.position, this.m_CharacterController.radius, Vector3.down, out hitInfo, this.m_CharacterController.height/2f);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            this.m_MoveDir.x = desiredMove.x*speed;
            this.m_MoveDir.z = desiredMove.z*speed;


            if (this.m_CharacterController.isGrounded)
            {
                this.m_MoveDir.y = -this.m_StickToGroundForce;

                if (this.m_Jump)
                {
                    this.m_MoveDir.y = this.m_JumpSpeed;
                    this.PlayJumpSound();
                    this.m_Jump = false;
                    this.m_Jumping = true;
                }
            }
            else
            {
                this.m_MoveDir += Physics.gravity* this.m_GravityMultiplier*Time.fixedDeltaTime;
            }
            this.m_CollisionFlags = this.m_CharacterController.Move(this.m_MoveDir*Time.fixedDeltaTime);

            this.ProgressStepCycle(speed);
            this.UpdateCameraPosition(speed);
        }


        private void PlayJumpSound()
        {
            this.m_AudioSource.clip = this.m_JumpSound;
            this.m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (this.m_CharacterController.velocity.sqrMagnitude > 0 && (this.m_Input.x != 0 || this.m_Input.y != 0))
            {
                this.m_StepCycle += (this.m_CharacterController.velocity.magnitude + (speed*(this.m_IsWalking ? 1f : this.m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(this.m_StepCycle > this.m_NextStep))
            {
                return;
            }

            this.m_NextStep = this.m_StepCycle + this.m_StepInterval;

            this.PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!this.m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, this.m_FootstepSounds.Length);
            this.m_AudioSource.clip = this.m_FootstepSounds[n];
            this.m_AudioSource.PlayOneShot(this.m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            this.m_FootstepSounds[n] = this.m_FootstepSounds[0];
            this.m_FootstepSounds[0] = this.m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!this.m_UseHeadBob)
            {
                return;
            }
            if (this.m_CharacterController.velocity.magnitude > 0 && this.m_CharacterController.isGrounded)
            {
                this.m_Camera.transform.localPosition = this.m_HeadBob.DoHeadBob(this.m_CharacterController.velocity.magnitude +
                                      (speed*(this.m_IsWalking ? 1f : this.m_RunstepLenghten)));
                newCameraPosition = this.m_Camera.transform.localPosition;
                newCameraPosition.y = this.m_Camera.transform.localPosition.y - this.m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = this.m_Camera.transform.localPosition;
                newCameraPosition.y = this.m_OriginalCameraPosition.y - this.m_JumpBob.Offset();
            }
            this.m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = this.m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            this.m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = this.m_IsWalking ? this.m_WalkSpeed : this.m_RunSpeed;
            this.m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (this.m_Input.sqrMagnitude > 1)
            {
                this.m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (this.m_IsWalking != waswalking && this.m_UseFovKick && this.m_CharacterController.velocity.sqrMagnitude > 0)
            {
                this.StopAllCoroutines();
                this.StartCoroutine(!this.m_IsWalking ? this.m_FovKick.FOVKickUp() : this.m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            this.m_MouseLook.LookRotation (this.transform, this.m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (this.m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(this.m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
