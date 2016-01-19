namespace Assets.Scripts
{
    using Assets.Scripts.MapGeneration;

    using UnityEngine;

    internal class HandOfGod : MonoBehaviour
    {
        [SerializeField]
        private bool _checkForCollision = true;
        public float _RADIUS = 30F;
        public float _POWER = 5f;


        public static HandOfGod Instance;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform == FloorBelow.Instance.transform || collision.transform.tag == C.PLAYER_TAG || collision.transform.tag == C.WALL_TAG)
            {
                this.Respawn();
                if (collision.transform.tag == C.PLAYER_TAG)
                    collision.transform.GetComponent<Player>().Kill();
                return;
            }

            if (collision.transform.tag != C.FLOOR_TAG || !this._checkForCollision) return;
            this._checkForCollision = false;
            this.Invoke("SetCheckForCollisionTrue", 2f);

            RaycastHit hit;
            Ray ray = new Ray(this.transform.position, new Vector3(0, - 1, 0));
            Physics.Raycast(ray, out hit);
            Debug.DrawRay(this.transform.position, ray.direction, Color.red, 3f);
            Cube cube = hit.collider.GetComponent<Cube>();
            if (cube == null)
                cube = collision.collider.GetComponent<Cube>();
            cube.GetComponent<Rigidbody>().isKinematic = false;
            cube.GetComponent<Rigidbody>().useGravity = true;
            cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.yellow;
            Debug.DrawRay(this.transform.position, ray.direction * 5, Color.yellow, 20f);


            LogHelper.Log(typeof(HandOfGod), "Shaking...");
            Shaker.Instance.StartShake(cube);
            this.transform.GetComponent<Rigidbody>().AddExplosionForce(this._POWER, this.transform.position, this._RADIUS);
            LogHelper.Log(typeof(HandOfGod), "Done shaking");
            this.Invoke("Respawn", 2f);
        }

        private void Respawn()
        {
            this.transform.position = new Vector3(Random.Range(MapGenerator.Instance.transform.position.x + (this.transform.localScale.x) - MapGenerator.Instance.X, MapGenerator.Instance.transform.position.x - (this.transform.localScale.x) + MapGenerator.Instance.X),
                80f, Random.Range(MapGenerator.Instance.transform.position.z + (this.transform.localScale.z) - MapGenerator.Instance.Z, MapGenerator.Instance.transform.position.z - (this.transform.localScale.z) + MapGenerator.Instance.Z));
        }

        // ReSharper disable once UnusedMember.Local
        private void SetCheckForCollisionTrue()
        {
            this._checkForCollision = true;
        }
    }
}
