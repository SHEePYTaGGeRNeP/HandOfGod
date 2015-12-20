using UnityEngine;

namespace Assets.Standard_Assets.Utility
{
    using Assets.Standard_Assets.CrossPlatformInput.Scripts;

    [RequireComponent(typeof (GUITexture))]
    public class ForcedReset : MonoBehaviour
    {
        private void Update()
        {
            // if we have forced a reset ...
            if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
            {
                //... reload the scene
                Application.LoadLevelAsync(Application.loadedLevelName);
            }
        }
    }
}
