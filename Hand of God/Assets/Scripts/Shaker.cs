namespace Assets.Scripts
{
    using System.Collections.Generic;

    using Assets.Scripts.MapGeneration;

    using UnityEngine;

    internal class IterateCube
    {
        public Cube Cube { get; set; }
        public int Distance { get; set; }
        
        public IterateCube(Cube pCube, int pDistance)
        {
            this.Cube = pCube;
            this.Distance = pDistance;
        }
    }

    internal class Shaker
    {
        public static Shaker Instance = new Shaker();

        private readonly HashSet<Cube> _iteratedCubes = new HashSet<Cube>();
        private readonly Queue<IterateCube> _toIterateCubes = new Queue<IterateCube>();

        public void StartShake(Cube cube)
        {
            this._iteratedCubes.Clear();
            this._toIterateCubes.Clear();
            this._toIterateCubes.Enqueue(new IterateCube(cube, 0));
            while (this._toIterateCubes.Count > 0)
            {
                IterateCube itCube = this._toIterateCubes.Dequeue();
                this.Shake(itCube.Cube, itCube.Distance);
            }
        }

        public void Shake(Cube cube, int distance)
        {
            if (this._iteratedCubes.Contains(cube)) return;
            this._iteratedCubes.Add(cube);
            if (distance > 99) distance = 99;
            RaycastHit hit;
            Ray ray = new Ray(cube.transform.position, new Vector3(0, 1, 0));
            Debug.DrawRay(cube.transform.position, ray.direction, Color.red, 3f);
            Physics.Raycast(ray, out hit);
            if (hit.collider != null && hit.transform.name == HandOfGod.Instance.transform.name)
                distance = 0;


            cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.green;
            int result = Random.Range(1, 101);
            if (result <= distance)
                return;
            cube.transform.GetComponent<Rigidbody>().isKinematic = false;
            cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.magenta;

            if (Random.Range(1, 4) > 1)
            {
                cube.transform.GetComponent<Rigidbody>().useGravity = true;
                cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            }
            foreach (Cube cubeChild in cube.ConnectedCubes.Values)
                this._toIterateCubes.Enqueue(new IterateCube(cubeChild, distance + 5));//  this.Shake(cubeChild, distance + 5);
        }
    }
}
