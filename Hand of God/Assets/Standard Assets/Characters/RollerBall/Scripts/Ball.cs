namespace Assets.Standard_Assets.Characters.RollerBall.Scripts
{
    using UnityEngine;

    public class Ball : MonoBehaviour
    {
        [SerializeField] private float m_MovePower = 5; // The force added to the ball to move it.
        [SerializeField] private bool m_UseTorque = true; // Whether or not to use torque to move the ball.
        [SerializeField] private float m_MaxAngularVelocity = 25; // The maximum velocity the ball can rotate at.
        [SerializeField] private float m_JumpPower = 2; // The force added to the ball when it jumps.

        private const float k_GroundRayLength = 1f; // The length of the ray to check if the ball is grounded.
        private Rigidbody m_Rigidbody;


        private void Start()
        {
            this.m_Rigidbody = this.GetComponent<Rigidbody>();
            // Set the maximum angular velocity.
            this.GetComponent<Rigidbody>().maxAngularVelocity = this.m_MaxAngularVelocity;
        }


        public void Move(Vector3 moveDirection, bool jump)
        {
            // If using torque to rotate the ball...
            if (this.m_UseTorque)
            {
                // ... add torque around the axis defined by the move direction.
                this.m_Rigidbody.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x)* this.m_MovePower);
            }
            else
            {
                // Otherwise add force in the move direction.
                this.m_Rigidbody.AddForce(moveDirection* this.m_MovePower);
            }

            // If on the ground and jump is pressed...
            if (Physics.Raycast(this.transform.position, -Vector3.up, k_GroundRayLength) && jump)
            {
                // ... add force in upwards.
                this.m_Rigidbody.AddForce(Vector3.up* this.m_JumpPower, ForceMode.Impulse);
            }
        }
    }
}
