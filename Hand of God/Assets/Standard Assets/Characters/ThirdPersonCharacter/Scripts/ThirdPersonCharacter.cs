namespace Assets.Standard_Assets.Characters.ThirdPersonCharacter.Scripts
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;


		void Start()
		{
		    this.m_Animator = this.GetComponent<Animator>();
		    this.m_Rigidbody = this.GetComponent<Rigidbody>();
		    this.m_Capsule = this.GetComponent<CapsuleCollider>();
		    this.m_CapsuleHeight = this.m_Capsule.height;
		    this.m_CapsuleCenter = this.m_Capsule.center;

		    this.m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		    this.m_OrigGroundCheckDistance = this.m_GroundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = this.transform.InverseTransformDirection(move);
		    this.CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, this.m_GroundNormal);
		    this.m_TurnAmount = Mathf.Atan2(move.x, move.z);
		    this.m_ForwardAmount = move.z;

		    this.ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (this.m_IsGrounded)
			{
			    this.HandleGroundedMovement(crouch, jump);
			}
			else
			{
			    this.HandleAirborneMovement();
			}

		    this.ScaleCapsuleForCrouching(crouch);
		    this.PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
		    this.UpdateAnimator(move);
		}


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (this.m_IsGrounded && crouch)
			{
				if (this.m_Crouching) return;
			    this.m_Capsule.height = this.m_Capsule.height / 2f;
			    this.m_Capsule.center = this.m_Capsule.center / 2f;
			    this.m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(this.m_Rigidbody.position + Vector3.up * this.m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = this.m_CapsuleHeight - this.m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, this.m_Capsule.radius * k_Half, crouchRayLength))
				{
				    this.m_Crouching = true;
					return;
				}
			    this.m_Capsule.height = this.m_CapsuleHeight;
			    this.m_Capsule.center = this.m_CapsuleCenter;
			    this.m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!this.m_Crouching)
			{
				Ray crouchRay = new Ray(this.m_Rigidbody.position + Vector3.up * this.m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = this.m_CapsuleHeight - this.m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, this.m_Capsule.radius * k_Half, crouchRayLength))
				{
				    this.m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
		    this.m_Animator.SetFloat("Forward", this.m_ForwardAmount, 0.1f, Time.deltaTime);
		    this.m_Animator.SetFloat("Turn", this.m_TurnAmount, 0.1f, Time.deltaTime);
		    this.m_Animator.SetBool("Crouch", this.m_Crouching);
		    this.m_Animator.SetBool("OnGround", this.m_IsGrounded);
			if (!this.m_IsGrounded)
			{
			    this.m_Animator.SetFloat("Jump", this.m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(this.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + this.m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * this.m_ForwardAmount;
			if (this.m_IsGrounded)
			{
			    this.m_Animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (this.m_IsGrounded && move.magnitude > 0)
			{
			    this.m_Animator.speed = this.m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
			    this.m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * this.m_GravityMultiplier) - Physics.gravity;
		    this.m_Rigidbody.AddForce(extraGravityForce);

		    this.m_GroundCheckDistance = this.m_Rigidbody.velocity.y < 0 ? this.m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && this.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
			    this.m_Rigidbody.velocity = new Vector3(this.m_Rigidbody.velocity.x, this.m_JumpPower, this.m_Rigidbody.velocity.z);
			    this.m_IsGrounded = false;
			    this.m_Animator.applyRootMotion = false;
			    this.m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(this.m_StationaryTurnSpeed, this.m_MovingTurnSpeed, this.m_ForwardAmount);
		    this.transform.Rotate(0, this.m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (this.m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (this.m_Animator.deltaPosition * this.m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = this.m_Rigidbody.velocity.y;
			    this.m_Rigidbody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(this.transform.position + (Vector3.up * 0.1f), this.transform.position + (Vector3.up * 0.1f) + (Vector3.down * this.m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(this.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, this.m_GroundCheckDistance))
			{
			    this.m_GroundNormal = hitInfo.normal;
			    this.m_IsGrounded = true;
			    this.m_Animator.applyRootMotion = true;
			}
			else
			{
			    this.m_IsGrounded = false;
			    this.m_GroundNormal = Vector3.up;
			    this.m_Animator.applyRootMotion = false;
			}
		}
	}
}
