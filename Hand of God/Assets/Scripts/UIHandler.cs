namespace Assets.Scripts
{
    using UnityEngine;

    internal class UIHandler : MonoBehaviour
    {

        public void ReloadLevel()
        {
            Application.LoadLevel(Application.loadedLevel);
        }

    }
}
