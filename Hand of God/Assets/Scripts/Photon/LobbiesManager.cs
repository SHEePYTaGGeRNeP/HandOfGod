namespace Assets.Scripts.Photon
{
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
            RoomOptions ro = new RoomOptions() { isVisible = true, maxPlayers = 5, isOpen = true };
            PhotonNetwork.CreateRoom("Test1", ro, TypedLobby.Default);
            PhotonNetwork.CreateRoom("Test2", ro, TypedLobby.Default);
            PhotonNetwork.CreateRoom("Test3", ro, TypedLobby.Default);
            PhotonNetwork.CreateRoom("Test4", ro, TypedLobby.Default);
            PhotonNetwork.CreateRoom("Test5", ro, TypedLobby.Default);
            PhotonNetwork.JoinOrCreateRoom("Test31", ro, TypedLobby.Default);
            this.PopulateServerList();
        }

        public void SliderPlayersChanged()
        {
            this._maxPlayersText.text = this._maxPlayersSlider.value.ToString(CultureInfo.InvariantCulture);
        }

        public void PopulateServerList()
        {
            RoomInfo[] rooms = PhotonNetwork.GetRoomList();

            if (null == rooms) return;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i].open)
                    continue;

                GameObject roomElement = Instantiate(this._roomElementPrefab);
                roomElement.transform.SetParent(this._contentTransform);
                roomElement.transform.position = new Vector3(4, i * 61 + 1, 0);
                roomElement.transform.FindChild("RoomTitleText").GetComponent<Text>().text = rooms[i].name;
                roomElement.transform.FindChild("AmountOfPlayersText").GetComponent<Text>().text = rooms[i].playerCount + "/" + rooms[i].maxPlayers;
                roomElement.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (i * -25), 0);
            }
        }

        public void CreateRoomButtonClick()
        {
           //PhotonManager.Instance.CreateRoom(
        }

    }
}
