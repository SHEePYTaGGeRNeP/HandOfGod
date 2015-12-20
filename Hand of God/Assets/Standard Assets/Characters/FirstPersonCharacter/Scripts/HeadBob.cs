namespace Assets.Standard_Assets.Characters.FirstPersonCharacter.Scripts
{
    using Assets.Standard_Assets.Utility;

    using UnityEngine;

    public class HeadBob : MonoBehaviour
    {
        public Camera Camera;
        public CurveControlledBob motionBob = new CurveControlledBob();
        public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
        public RigidbodyFirstPersonController rigidbodyFirstPersonController;
        public float StrideInterval;
        [Range(0f, 1f)] public float RunningStrideLengthen;

       // private CameraRefocus m_CameraRefocus;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;


        private void Start()
        {
            this.motionBob.Setup(this.Camera, this.StrideInterval);
            this.m_OriginalCameraPosition = this.Camera.transform.localPosition;
       //     m_CameraRefocus = new CameraRefocus(Camera, transform.root.transform, Camera.transform.localPosition);
        }


        private void Update()
        {
          //  m_CameraRefocus.GetFocusPoint();
            Vector3 newCameraPosition;
            if (this.rigidbodyFirstPersonController.Velocity.magnitude > 0 && this.rigidbodyFirstPersonController.Grounded)
            {
                this.Camera.transform.localPosition = this.motionBob.DoHeadBob(this.rigidbodyFirstPersonController.Velocity.magnitude*(this.rigidbodyFirstPersonController.Running ? this.RunningStrideLengthen : 1f));
                newCameraPosition = this.Camera.transform.localPosition;
                newCameraPosition.y = this.Camera.transform.localPosition.y - this.jumpAndLandingBob.Offset();
            }
            else
            {
                newCameraPosition = this.Camera.transform.localPosition;
                newCameraPosition.y = this.m_OriginalCameraPosition.y - this.jumpAndLandingBob.Offset();
            }
            this.Camera.transform.localPosition = newCameraPosition;

            if (!this.m_PreviouslyGrounded && this.rigidbodyFirstPersonController.Grounded)
            {
                this.StartCoroutine(this.jumpAndLandingBob.DoBobCycle());
            }

            this.m_PreviouslyGrounded = this.rigidbodyFirstPersonController.Grounded;
          //  m_CameraRefocus.SetFocusPoint();
        }
    }
}
