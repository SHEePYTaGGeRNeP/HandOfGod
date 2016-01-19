namespace Assets.Scripts.Photon
{
    using System;
    using System.Globalization;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.UI;

    public class LobbiesManager : MonoBehaviour
    {

        [SerializeField]
        private Dropdown _roomsDropdown;
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
                        //this.PopulateServerList();
                    };
        }

        public void Refresh()
        {
            LogHelper.Log(typeof(LobbiesManager), "Inside lobby: " + PhotonNetwork.insideLobby);
            this.PopulateServerList();
        }

        public void SliderPlayersChanged()
        {
            this._maxPlayersText.text = this._maxPlayersSlider.value.ToString(CultureInfo.InvariantCulture);
        }

        public void JoinGameButtonClicked()
        {
            bool join = true;
            RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            RoomInfo room = null;
            if (rooms == null || rooms.Length == 0)
                join = false;
            else
                foreach (RoomInfo ro in rooms.Where(ro => ro.name == this._roomsDropdown.captionText.text))
                {
                    room = ro;
                    break;
                }

            if (room == null)
            {
                Debug.LogError("LobbiesManager: Room does not exist.");
                join = false;
            }
            else if (room.open)
            {
                Debug.LogError("LobbiesManager: Room is not open.");
                join = false;
            }
            else if (room.playerCount >= room.maxPlayers)
            {
                Debug.LogError("LobbiesManager: Room is full.");
                join = false;
            }
            if (join)
                PhotonNetwork.JoinRoom(room.name);
            else
                this.PopulateServerList();
        }

        public void PopulateServerList()
        {
            this._roomsDropdown.options.Clear();
            this._roomsDropdown.options.Add(new Dropdown.OptionData(String.Empty));
            this._roomsDropdown.captionText.text = String.Empty;

            RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            LogHelper.Log(typeof(LobbiesManager), "Amount of rooms: " + rooms.Length);
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i].open)
                    continue;
                this._roomsDropdown.options.Add(new Dropdown.OptionData(rooms[i].ToStringInGame()));

            }
        }

        public void CreateRoomButtonClick()
        {
            if (String.IsNullOrEmpty(this._roomNameField.text.Trim()))
                return;

            PhotonManager.Instance.CreateRoom(this._roomNameField.text, (int)this._maxPlayersSlider.value);
            Application.LoadLevel("GameScene");
        }

    }
}
