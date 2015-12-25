namespace Assets.Scripts.MapGeneration
{
    using System;

    using UnityEngine;

    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cube1x1Prefab;

        public int X, Z;

        [SerializeField]
        private int _wallHeight = 20;

        void Awake()
        {
            this.StartGenerating(1);
        }

        public void StartGenerating(int seed)
        {
            for (int x = -this.X; x < this.X; x++)
                for (int z = -this.Z; z < this.Z; z++)
                {
                    // *2f because scale = 0.5 because cube was 2x2
                    float posX = this.transform.position.x + (this._cube1x1Prefab.transform.localScale.x * 2f * x);
                    float posZ = this.transform.position.z + (this._cube1x1Prefab.transform.localScale.z * 2f * z);
                    if (this.GenerateWall(posX, posZ)) continue;
                    GameObject cube = (GameObject)GameObject.Instantiate(this._cube1x1Prefab, new Vector3(posX, this.transform.position.y, posZ), Quaternion.identity);
                    cube.transform.SetParent(this.transform);
                }

        }

        private bool GenerateWall(float posX, float posZ)
        {
            if (posX == this.transform.position.x - this.X || posX == this.transform.position.x + (this.X -1 )
                || posZ == this.transform.position.z - this.Z || posZ == this.transform.position.z + (this.Z - 1))
            {
                for (int i = 0; i < this._wallHeight; i++)
                {
                    GameObject cube = (GameObject)GameObject.Instantiate(this._cube1x1Prefab,
                        new Vector3(posX, this.transform.position.y + (i * this._cube1x1Prefab.transform.localScale.y * 2f), posZ), Quaternion.identity);
                    cube.transform.SetParent(this.transform);
                    cube.transform.tag = C.WALL_TAG;
                }
                return true;
            }
            return false;
        }
    }
}
