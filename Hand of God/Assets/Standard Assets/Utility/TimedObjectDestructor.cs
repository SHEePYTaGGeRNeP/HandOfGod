namespace Assets.Standard_Assets.Utility
{
    using UnityEngine;

    public class TimedObjectDestructor : MonoBehaviour
    {
        [SerializeField] private float m_TimeOut = 1.0f;
        [SerializeField] private bool m_DetachChildren = false;


        private void Awake()
        {
            this.Invoke("DestroyNow", this.m_TimeOut);
        }


        private void DestroyNow()
        {
            if (this.m_DetachChildren)
            {
                this.transform.DetachChildren();
            }
            DestroyObject(this.gameObject);
        }
    }
}
