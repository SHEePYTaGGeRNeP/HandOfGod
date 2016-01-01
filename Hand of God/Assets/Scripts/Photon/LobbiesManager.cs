namespace Assets.Scripts.Photon
{
    using System;
    using System.Globalization;

    using UnityEngine;
    using UnityEngine.UI;

    public class LobbiesManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject _roomElementPrefab;
        [SerializeField]
        private Transform _contentTransform;
        [SerializeField]
        private InputField _roomNameField;
        [SerializeField]
        private Slider _maxPlayersSlider;
        [SerializeField]
        private Text _maxPlayersText;

        internal void Start()
        {
            PhotonManager.Instance.OnReceivedRoomListUpdateEvent +=
                delegate
                    {
                        this.PopulateServerList();
                    };
        }

        public void Refresh()
        {
            Helper.Log("LobbiesManager", "Inside lobby: " + PhotonNetwork.insideLobby.ToString());
            //if (!PhotonNetwork.inRoom)
            //    PhotonNetwork.CreateRoom(null);
            this.PopulateServerList();
        }

        public void SliderPlayersChanged()
        {
            this._maxPlayersText.text = this._maxPlayersSlider.value.ToString(CultureInfo.InvariantCulture);
        }

        public void PopulateServerList()
        {
            RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            Helper.Log("LobbiesManager", rooms.Length.ToString());
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i].open)
                    continue;

                GameObject roomElement = Instantiate(this._roomElementPrefab);
                roomElement.transform.SetParent(this._contentTransform);
                roomElement.transform.position = new Vector3(4, i * 61 + 1, 0);
                roomElement.transform.FindChild("RoomTitleText").GetComponent<Text>().text = rooms[i].name;
                roomElement.transform.FindChild("AmountOfPlayersText").GetComponent<Text>().text = rooms[i].playerCount + "/" + rooms[i].maxPlayers;
                
            }
        }

        public void CreateRoomButtonClick()
        {
           //PhotonManager.Instance.CreateRoom(
        }

    }
}
