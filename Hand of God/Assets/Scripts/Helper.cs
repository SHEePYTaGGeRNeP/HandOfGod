namespace Assets.Scripts
{
    using UnityEngine;

    internal class Helper
    {

        public static void Log(string classname, string message)
        {
            Debug.Log("<color=teal>" + classname + ":</color> " + message);
        }
    }
}
