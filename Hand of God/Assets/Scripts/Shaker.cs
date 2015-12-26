﻿namespace Assets.Scripts
{
    using System.Collections.Generic;

    using Assets.Scripts.MapGeneration;

    using UnityEngine;

    internal class Shaker
    {
        public static Shaker Instance = new Shaker();

        private readonly HashSet<Cube> _iteratedCubes = new HashSet<Cube>();

        private readonly HashSet<Cube> _toIterateCubes = new HashSet<Cube>();
        //    this._toIterateCubes.Clear();
        //    this._toIterateCubes.Add(cube);
        //    int distance = 0;
        //    while (this._toIterateCubes.Count > 0)
        //    {
        //        this.Shake(cube, distance);
        //        distance += 5;
        //    }
        //        this._toIterateCubes.Add(cubeChild);

        public void StartShake(Cube cube)
        {
            this._iteratedCubes.Clear();
            this.Shake(cube, 0);
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


            cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.magenta;
            int result = Random.Range(1, 101);
            if (result <= distance)
                return;
            cube.transform.GetComponent<Rigidbody>().isKinematic = false;
            cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.green;

            if (Random.Range(1, 4) > 1)
            {
                cube.transform.GetComponent<Rigidbody>().useGravity = true;
                cube.transform.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            }
            foreach (Cube cubeChild in cube.ConnectedCubes.Values)
                this.Shake(cubeChild, distance += 5);
        }
    }
}
