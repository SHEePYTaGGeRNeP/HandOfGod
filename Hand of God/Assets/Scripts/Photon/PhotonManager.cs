namespace Assets.Scripts.Photon
{
    using System;

    using global::Photon;

    using UnityEngine;

    internal class PhotonManager : PunBehaviour
    {
        public static PhotonManager Instance;       
        
        public bool Host { get; private set; }
        

        public event EventHandler OnJoinedRoomEvent;

        public event EventHandler OnReceivedRoomListUpdateEvent;



        private bool _aPlayerHasJoined;


        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            PhotonNetwork.logLevel = PhotonLogLevel.Informational;
            PhotonNetwork.ConnectUsingSettings("0.1");
        }

        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(new Vector2(10, 100), new Vector2(300, 200)));
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
            GUILayout.EndArea();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnReceivedRoomListUpdate()
        {            
            if (this.OnReceivedRoomListUpdateEvent != null)
                this.OnReceivedRoomListUpdateEvent.Invoke(null, null);
        }

        public void CreateRoom(string roomname, int maxPlayers)
        {
            Helper.Log("PhotonManager","Creating room " + roomname);
            this.Host = true;
            RoomOptions ro = new RoomOptions() { isVisible = true, maxPlayers = 5 };
            PhotonNetwork.CreateRoom(roomname, ro, TypedLobby.Default);
        }

        public override void OnJoinedLobby()
        {
            Helper.Log("PhotonManager" ,"OnJoinedLobby");// Joining random room!");
            //PhotonNetwork.JoinRandomRoom();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnPhotonRandomJoinFailed()
        {
            Helper.Log("PhotonManager","OnPhotonRandomJoinFailed Can't join random room - Creating room");
            this.Host = true;
            PhotonNetwork.CreateRoom(null);
        }

        public override void OnJoinedRoom()
        {
            Helper.Log("PhotonManager", "OnJoinedRoom : You have joined room : " + PhotonNetwork.room.name);
            if (PhotonNetwork.isMasterClient)
            {
                //PhotonNetwork.Instantiate("AI_Boat_Mobile_Roeien", this.transform.position - new Vector3(0, 0, 10f), Quaternion.identity, 0);
            }

            EventHandler handler = this.OnJoinedRoomEvent;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer player)
        {
            Helper.Log("PhotonManager", "Player connected");
            if (!PhotonNetwork.isMasterClient) return;
            
            

           // this.photonView.RPC("AssignPaddle", player, tempPlayerView.viewID, tempPaddleView.viewID, (int)tempPaddleView.GetComponent<Paddle>().RowSide);
        }
        
    }
}
