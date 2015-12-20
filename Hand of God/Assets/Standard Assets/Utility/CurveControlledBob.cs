namespace Assets.Standard_Assets.Utility
{
    using System;

    using UnityEngine;

    [Serializable]
    public class CurveControlledBob
    {
        public float HorizontalBobRange = 0.33f;
        public float VerticalBobRange = 0.33f;
        public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob
        public float VerticaltoHorizontalRatio = 1f;

        private float m_CyclePositionX;
        private float m_CyclePositionY;
        private float m_BobBaseInterval;
        private Vector3 m_OriginalCameraPosition;
        private float m_Time;


        public void Setup(Camera camera, float bobBaseInterval)
        {
            this.m_BobBaseInterval = bobBaseInterval;
            this.m_OriginalCameraPosition = camera.transform.localPosition;

            // get the length of the curve in time
            this.m_Time = this.Bobcurve[this.Bobcurve.length - 1].time;
        }


        public Vector3 DoHeadBob(float speed)
        {
            float xPos = this.m_OriginalCameraPosition.x + (this.Bobcurve.Evaluate(this.m_CyclePositionX)* this.HorizontalBobRange);
            float yPos = this.m_OriginalCameraPosition.y + (this.Bobcurve.Evaluate(this.m_CyclePositionY)* this.VerticalBobRange);

            this.m_CyclePositionX += (speed*Time.deltaTime)/ this.m_BobBaseInterval;
            this.m_CyclePositionY += ((speed*Time.deltaTime)/ this.m_BobBaseInterval)* this.VerticaltoHorizontalRatio;

            if (this.m_CyclePositionX > this.m_Time)
            {
                this.m_CyclePositionX = this.m_CyclePositionX - this.m_Time;
            }
            if (this.m_CyclePositionY > this.m_Time)
            {
                this.m_CyclePositionY = this.m_CyclePositionY - this.m_Time;
            }

            return new Vector3(xPos, yPos, 0f);
        }
    }
}
