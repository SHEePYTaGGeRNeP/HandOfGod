namespace Assets.Scripts
{
    using UnityEngine;

    class UIHandler : MonoBehaviour
    {

        public void ReloadLevel()
        {
            Application.LoadLevel(Application.loadedLevel);
        }

    }
}
