namespace Assets.Standard_Assets.Characters.FirstPersonCharacter.Scripts
{
    using System;

    using Assets.Standard_Assets.CrossPlatformInput.Scripts;

    using UnityEngine;

    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
				    this.CurrentTargetSpeed = this.StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
				    this.CurrentTargetSpeed = this.BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
				    this.CurrentTargetSpeed = this.ForwardSpeed;
				}
#if !MOBILE_INPUT
	            if (Input.GetKey(this.RunKey))
	            {
	                this.CurrentTargetSpeed *= this.RunMultiplier;
	                this.m_Running = true;
	            }
	            else
	            {
	                this.m_Running = false;
	            }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return this.m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;


        public Vector3 Velocity
        {
            get { return this.m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return this.m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return this.m_Jumping; }
        }

        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return this.movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            this.m_RigidBody = this.GetComponent<Rigidbody>();
            this.m_Capsule = this.GetComponent<CapsuleCollider>();
            this.mouseLook.Init (this.transform, this.cam.transform);
        }


        private void Update()
        {
            this.RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !this.m_Jump)
            {
                this.m_Jump = true;
            }
        }


        private void FixedUpdate()
        {
            this.GroundCheck();
            Vector2 input = this.GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (this.advancedSettings.airControl || this.m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = this.cam.transform.forward*input.y + this.cam.transform.right*input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, this.m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x* this.movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z* this.movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y* this.movementSettings.CurrentTargetSpeed;
                if (this.m_RigidBody.velocity.sqrMagnitude <
                    (this.movementSettings.CurrentTargetSpeed* this.movementSettings.CurrentTargetSpeed))
                {
                    this.m_RigidBody.AddForce(desiredMove* this.SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (this.m_IsGrounded)
            {
                this.m_RigidBody.drag = 5f;

                if (this.m_Jump)
                {
                    this.m_RigidBody.drag = 0f;
                    this.m_RigidBody.velocity = new Vector3(this.m_RigidBody.velocity.x, 0f, this.m_RigidBody.velocity.z);
                    this.m_RigidBody.AddForce(new Vector3(0f, this.movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    this.m_Jumping = true;
                }

                if (!this.m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && this.m_RigidBody.velocity.magnitude < 1f)
                {
                    this.m_RigidBody.Sleep();
                }
            }
            else
            {
                this.m_RigidBody.drag = 0f;
                if (this.m_PreviouslyGrounded && !this.m_Jumping)
                {
                    this.StickToGroundHelper();
                }
            }
            this.m_Jump = false;
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(this.m_GroundContactNormal, Vector3.up);
            return this.movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(this.transform.position, this.m_Capsule.radius, Vector3.down, out hitInfo,
                                   ((this.m_Capsule.height/2f) - this.m_Capsule.radius) + this.advancedSettings.stickToGroundHelperDistance))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    this.m_RigidBody.velocity = Vector3.ProjectOnPlane(this.m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            
            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
            this.movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = this.transform.eulerAngles.y;

            this.mouseLook.LookRotation (this.transform, this.cam.transform);

            if (this.m_IsGrounded || this.advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(this.transform.eulerAngles.y - oldYRotation, Vector3.up);
                this.m_RigidBody.velocity = velRotation* this.m_RigidBody.velocity;
            }
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            this.m_PreviouslyGrounded = this.m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(this.transform.position, this.m_Capsule.radius, Vector3.down, out hitInfo,
                                   ((this.m_Capsule.height/2f) - this.m_Capsule.radius) + this.advancedSettings.groundCheckDistance))
            {
                this.m_IsGrounded = true;
                this.m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                this.m_IsGrounded = false;
                this.m_GroundContactNormal = Vector3.up;
            }
            if (!this.m_PreviouslyGrounded && this.m_IsGrounded && this.m_Jumping)
            {
                this.m_Jumping = false;
            }
        }
    }
}
