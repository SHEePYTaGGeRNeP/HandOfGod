namespace Assets.Scripts
{
    using UnityEngine;
    
    internal class FloorBelow : MonoBehaviour
    {

        public static FloorBelow Instance;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.transform.tag)
            {
                case C.FLOOR_TAG:
                    collision.transform.GetComponent<Rigidbody>().isKinematic = true;
                    collision.transform.GetComponent<Rigidbody>().useGravity = false;
                    collision.transform.GetComponent<Rigidbody>().detectCollisions = false;
                    collision.transform.GetComponent<BoxCollider>().enabled = false;
                    collision.transform.GetComponent<MeshRenderer>().enabled = false;
                    break;
            }

        }

    }
}
