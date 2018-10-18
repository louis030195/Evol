using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Evol.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.SocketServer;

namespace Evol.Game.Networking
{
    public class Server : MonoBehaviourPunCallbacks
    {

        public bool IsServer = false;
        public Text Informations;
        public Canvas Ui;
        
        public GameObject PlayerPrefab;
        public List<GameObject> SpawnablePrefabs;
    
            
        public Pool PlayerPool;
        public Pool HerbivorousPool;
        public Pool CarnivorousPool;
    
        private GameObject evolAcademy;
        private List<GameObject> players;


        /// <summary>
        /// The maximum number of players in game
        /// </summary>
        [Tooltip("The maximum number of players in game")]
        public byte MaxPlayersPerRoom = 4;


        private void Start()
        {
            if(!IsServer)
                Destroy(this); // Destroy the server script if not server
            players = new List<GameObject>();
            PhotonNetwork.ConnectUsingSettings();
        }

        private void OnConnectedToServer()
        {
            Debug.Log($"OnConnectedToServer()");
        }


        public override void OnJoinedLobby()
        {
            Debug.Log($"OnJoinedLobby()"); 
        }

        public override void OnCreatedRoom()
        {
            Informations.text = $"Created room { PhotonNetwork.CurrentRoom }";
            Debug.Log($"OnCreatedRoom()"); 
            
            evolAcademy = Instantiate(SpawnablePrefabs.Find(prefab => prefab.name.Contains("Academy")));
            PlayerPool = new Pool(PlayerPrefab);
            HerbivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("carnivorous")));
            CarnivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("herbivorous")));
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRandomFailed() - {returnCode} - {message}"); 
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRoomFailed() - {returnCode} - {message}"); 
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"OnJoinedRoom()"); 
            Informations.text = $"Joined room { PhotonNetwork.CurrentRoom }";

            
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Informations.text = $"{ newPlayer.NickName } joined the game";
            Destroy(Ui.gameObject);
            //var player = PlayerPool.GetObject();
            //player.SetActive(true);
            
            var player = PhotonNetwork.InstantiateSceneObject("Player", Vector3.zero, Quaternion.identity);
            player.name = newPlayer.NickName;
            player.GetComponent<PhotonView>().TransferOwnership(player.GetComponent<PhotonView>().ViewID);
            players.Add(player);

            
            Debug.Log($"OnPlayerEnteredRoom() { newPlayer.NickName }");
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Informations.text = $"{ otherPlayer.NickName } left the game";
            Debug.Log($"OnPlayerLeftRoom() { otherPlayer.NickName }"); 
            
            PlayerPool.ReleaseObject(players.Find(p => p.name.Equals(otherPlayer.NickName)));
            players.Remove(players.Find(p => p.name.Equals(otherPlayer.NickName)));
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log($"OnConnectedToMaster()");
            
            // The server create the room
            var newRoomOptions = new RoomOptions();
            newRoomOptions.IsOpen = true;
            newRoomOptions.IsVisible = true;
            newRoomOptions.MaxPlayers = MaxPlayersPerRoom;
            
            PhotonNetwork.CreateRoom("Yolo", newRoomOptions);
            
        }
        
        private IEnumerator SpawnAgents()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0, 10));

                Vector2 direction = Random.insideUnitCircle;
                Vector3 position = Vector3.zero;

                Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);
                object[] instantiationData = { torque, true};

                PhotonNetwork.InstantiateSceneObject("Agent", position, Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f), 0, instantiationData);
            }
        }
    }
}
