namespace Assets.Scripts.MapGeneration
{
    using System.Collections.Generic;

    internal class Cube : UnityEngine.MonoBehaviour, ICube
    {
        public Dictionary<Side, Cube> ConnectedCubes { get; set; }

        public Cube()
        {
            this.ConnectedCubes = new Dictionary<Side, Cube>();
        }

        public void SetConnectedCubes()
        {
            UnityEngine.Ray ray = new UnityEngine.Ray(UnityEngine.Vector3.zero, UnityEngine.Vector3.zero);
            Side side = Side.Front;
            for (int i = 0; i < 4; i++)
            {
                this.RayCheck(ref ray, ref side, i);
                UnityEngine.RaycastHit hit;
                UnityEngine.Physics.Raycast(ray, out hit);
                if (hit.transform.tag == C.FLOOR_TAG)
                    this.ConnectedCubes.Add(side, hit.transform.GetComponent<Cube>());
            }
        }

        private void RayCheck(ref UnityEngine.Ray ray, ref Side side, int i)
        {
            switch (i)
            {
                case 0:
                    ray = new UnityEngine.Ray(this.transform.position, this.transform.forward);
//#if UNITY_EDITOR
//                    UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward, UnityEngine.Color.cyan, 1f);
//#endif
//#if UNITY_EDITOR_64
//                    UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward, UnityEngine.Color.cyan, 1f);
//#endif
                    side = Side.Front;
                    break;
                case 1:
                    ray = new UnityEngine.Ray(this.transform.position, this.transform.right);
//#if UNITY_EDITOR
//                    UnityEngine.Debug.DrawRay(this.transform.position, this.transform.right, UnityEngine.Color.cyan, 1f);
//#endif
//#if UNITY_EDITOR_64
//                    UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward, UnityEngine.Color.cyan, 1f);
//#endif
                    side = Side.Right;
                    break;
                case 2:
                    ray = new UnityEngine.Ray(this.transform.position, -this.transform.forward);
//#if UNITY_EDITOR
//                    UnityEngine.Debug.DrawRay(this.transform.position, -this.transform.forward, UnityEngine.Color.cyan, 1f);
//#endif
//#if UNITY_EDITOR_64
//                    UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward, UnityEngine.Color.cyan, 1f);
//#endif
                    side = Side.Back;
                    break;
                case 3:
                    ray = new UnityEngine.Ray(this.transform.position, -this.transform.right);
//#if UNITY_EDITOR
//                    UnityEngine.Debug.DrawRay(this.transform.position, -this.transform.right, UnityEngine.Color.cyan, 1f);
//#endif
//#if UNITY_EDITOR_64
//                    UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward, UnityEngine.Color.cyan,1f);
//#endif
                    side = Side.Left;
                    break;
                default:
                    UnityEngine.Debug.LogError("Default switch case in RayCheck");
                    break;
            }
        }
    }
}
