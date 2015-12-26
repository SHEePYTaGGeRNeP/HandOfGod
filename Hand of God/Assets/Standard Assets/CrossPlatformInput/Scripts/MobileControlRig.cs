#if UNITY_EDITOR
#endif

namespace Assets.Standard_Assets.CrossPlatformInput.Scripts
{

#if UNITY_EDITOR
    using UnityEditor;
#endif
#if UNITY_EDITOR_64
    using UnityEditor;
#endif

    using UnityEngine;

    [ExecuteInEditMode]
    public class MobileControlRig : MonoBehaviour
    {
        // this script enables or disables the child objects of a control rig
        // depending on whether the USE_MOBILE_INPUT define is declared.

        // This define is set or unset by a menu item that is included with
        // the Cross Platform Input package.

#if !UNITY_EDITOR
	void OnEnable()
	{
		CheckEnableControlRig();
	}
	#endif

        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) //if in the editor, need to check if we are playing, as start is also called just after exiting play
#endif
            {
                UnityEngine.EventSystems.EventSystem system = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

                if (system == null)
                {//the scene have no event system, spawn one
                    GameObject o = new GameObject("EventSystem");

                    o.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    o.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    o.AddComponent<UnityEngine.EventSystems.TouchInputModule>();
                }
            }
        }

#if UNITY_EDITOR

        private void OnEnable()
        {
            EditorUserBuildSettings.activeBuildTargetChanged += this.Update;
            EditorApplication.update += this.Update;
        }


        private void OnDisable()
        {
            EditorUserBuildSettings.activeBuildTargetChanged -= this.Update;
            EditorApplication.update -= this.Update;
        }


        private void Update()
        {
            this.CheckEnableControlRig();
        }
#endif


        private void CheckEnableControlRig()
        {
#if MOBILE_INPUT
		EnableControlRig(true);
		#else
            this.EnableControlRig(false);
#endif
        }


        private void EnableControlRig(bool enabled)
        {
            foreach (Transform t in this.transform)
            {
                t.gameObject.SetActive(enabled);
            }
        }
    }
}
