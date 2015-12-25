namespace Assets.Scripts.Photon
{
    using System;

    using Assets.Scripts.PhotonNetworking;

    using global::Photon;

    using UnityEngine;

    class PhotonManager : PunBehaviour
    {
        public static PhotonManager Instance;       
        
        public bool Host;
        

        public event EventHandler OnJoinedRoomEvent;

        private LobbiesManager _lobbiesManager;

        private bool _aPlayerHasJoined;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            PhotonNetwork.logLevel = PhotonLogLevel.Informational;
            PhotonNetwork.ConnectUsingSettings("0.1");
        }

        private void OnGUI()
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }

        public override void OnConnectedToMaster()
        {
            // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
            if (this._lobbiesManager == null)
            {
                Debug.Log("Joining random room!");
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                this._lobbiesManager.StartUpdating();
            }
        }

        public void CreateRoom(string roomname)
        {
            Debug.Log("Creating room " + roomname);
            this.Host = true;
            RoomOptions ro = new RoomOptions() { isVisible = true, maxPlayers = 5 };
            PhotonNetwork.CreateRoom(roomname, ro, TypedLobby.Default);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joining random room!");
            PhotonNetwork.JoinRandomRoom();
        }
        void OnPhotonRandomJoinFailed()
        {
            Debug.Log("Can't join random room - Creating room");
            this.Host = true;
            PhotonNetwork.CreateRoom(null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() : You Have Joined room : " + PhotonNetwork.room.name);
            if (PhotonNetwork.isMasterClient)
            {
                //PhotonNetwork.Instantiate("AI_Boat_Mobile_Roeien", this.transform.position - new Vector3(0, 0, 10f), Quaternion.identity, 0);
            }
            this.OnJoinedRoomReached(EventArgs.Empty);
        }
        protected virtual void OnJoinedRoomReached(EventArgs e)
        {
            EventHandler handler = this.OnJoinedRoomEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer player)
        {
            Debug.Log("Player connected");
            if (!PhotonNetwork.isMasterClient) return;
            
            

           // this.photonView.RPC("AssignPaddle", player, tempPlayerView.viewID, tempPaddleView.viewID, (int)tempPaddleView.GetComponent<Paddle>().RowSide);
        }
        
    }
}
