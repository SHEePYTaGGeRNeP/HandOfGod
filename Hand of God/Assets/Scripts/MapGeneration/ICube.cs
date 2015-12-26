namespace Assets.Scripts.MapGeneration
{
    using System.Collections.Generic;

    public enum Side { Front, Right, Back, Left };

    internal interface ICube
    {
        Dictionary<Side, Cube> ConnectedCubes { get; set; }

        void SetConnectedCubes();

    }
}
