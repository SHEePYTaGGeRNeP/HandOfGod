namespace Assets.Standard_Assets.Utility
{
    using UnityEngine;

    public class DynamicShadowSettings : MonoBehaviour
    {
        public Light sunLight;
        public float minHeight = 10;
        public float minShadowDistance = 80;
        public float minShadowBias = 1;
        public float maxHeight = 1000;
        public float maxShadowDistance = 10000;
        public float maxShadowBias = 0.1f;
        public float adaptTime = 1;

        private float m_SmoothHeight;
        private float m_ChangeSpeed;
        private float m_OriginalStrength = 1;


        private void Start()
        {
            this.m_OriginalStrength = this.sunLight.shadowStrength;
        }


        // Update is called once per frame
        private void Update()
        {
            Ray ray = new Ray(Camera.main.transform.position, -Vector3.up);
            RaycastHit hit;
            float height = this.transform.position.y;
            if (Physics.Raycast(ray, out hit))
            {
                height = hit.distance;
            }

            if (Mathf.Abs(height - this.m_SmoothHeight) > 1)
            {
                this.m_SmoothHeight = Mathf.SmoothDamp(this.m_SmoothHeight, height, ref this.m_ChangeSpeed, this.adaptTime);
            }

            float i = Mathf.InverseLerp(this.minHeight, this.maxHeight, this.m_SmoothHeight);

            QualitySettings.shadowDistance = Mathf.Lerp(this.minShadowDistance, this.maxShadowDistance, i);
            this.sunLight.shadowBias = Mathf.Lerp(this.minShadowBias, this.maxShadowBias, 1 - ((1 - i)*(1 - i)));
            this.sunLight.shadowStrength = Mathf.Lerp(this.m_OriginalStrength, 0, i);
        }
    }
}
