using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Evol.Utils;
using MLAgents;
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
        
        public GameObject PlayerPrefab;
        public List<GameObject> SpawnablePrefabs;
        public List<Brain> Brains;
    
            
        public Pool HerbivorousPool;
        public Pool CarnivorousPool;
    
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
            Debug.Log($"OnCreatedRoom()"); 
            
            //PlayerPool = new Pool(PlayerPrefab);
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
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            //var player = PlayerPool.GetObject();
            //player.SetActive(true);
            var player = PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.up, Quaternion.identity);
            //player.GetComponent<Agent>().GiveBrain(Brains[0]);
            //player.SetActive(true); // Required to give brain on disabled GO, then active it
            player.name = newPlayer.NickName;
            
            player.GetPhotonView().TransferOwnership(newPlayer);
            
            
            players.Add(player);

            
            Debug.Log($"OnPlayerEnteredRoom() { newPlayer.NickName }");
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log($"OnPlayerLeftRoom() { otherPlayer.NickName }"); 
            
            // PlayerPool.ReleaseObject(players.Find(p => p.name.Equals(otherPlayer.NickName)));
            Destroy(players.Find(p => p.name.Equals(otherPlayer.NickName)), 1f);
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
