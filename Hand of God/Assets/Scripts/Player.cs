namespace Assets.Scripts
{
    using System.Globalization;

    using Assets.Scripts.MapGeneration;
    using Assets.Standard_Assets.Characters.FirstPersonCharacter.Scripts;

    using UnityEngine;
    using UnityEngine.UI;

    class Player : MonoBehaviour
    {
        [SerializeField]
        private Text _pointsText;

        [SerializeField]
        private Text _RIPText;
        public bool IsDead = false;


        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if (this.IsDead) return;
            if (this.transform.position.y < MapGenerator.Instance.transform.position.y - 20f)
                this.Kill();
            this._pointsText.text = Time.timeSinceLevelLoad.ToString(CultureInfo.InvariantCulture);
        }

        public void Kill()
        {
            this.GetComponent<FirstPersonController>().m_WalkSpeed = 0f;
            this.GetComponent<FirstPersonController>().m_RunSpeed = 0f;
            this._RIPText.text = "R.I.P.";
            this.IsDead = true;
        }
    }
}
