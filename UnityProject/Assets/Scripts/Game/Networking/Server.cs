using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Evol.Agents;
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

        public bool IsServer;
        
        public GameObject PlayerPrefab;
        public List<GameObject> SpawnablePrefabs;
        public List<Brain> Brains;
        public GameObject Ground;

        public GameObject Test;
        
        
        public Pool HerbivorousPool;
        public Pool CarnivorousPool;
        public Pool HerbPool;
    
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
            HerbivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("herbivorous")));
            CarnivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("carnivorous")));
            HerbPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("food")));
            
            // Find the brains in the list
            HerbivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
            CarnivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);

            // TODO: Fix this shit (animals not sync in network)
            //StartCoroutine(SpawnAgents());

            StartCoroutine(SpawnTree());
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
            // PhotonNetwork.Instantiate("Actors/PlayerControlled/Player") 
            var player = PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.up, Quaternion.identity);
            //player.GetComponent<Agent>().brain.InitializeBrain(FindObjectOfType<Academy>(), null);
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

        private IEnumerator SpawnTree()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0, 10));

                var meshObject = Test.GetComponent<MeshFilter>().mesh;

                // Randomly change vertices
                var vertices = meshObject.vertices;
                var p = 0;
                while (p < vertices.Length)
                {
                    vertices[p] += new Vector3(0, Random.Range(-0.3F, 0.3F), 0);
                    p++;
                }

                meshObject.vertices = vertices;
                meshObject.RecalculateNormals();
            }
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

                //var herbivorousObject = HerbivorousPool.GetObject();
                var herbivorousObject = PhotonNetwork.InstantiateSceneObject(SpawnablePrefabs.Find(go => go.CompareTag("herbivorous")).name, Vector3.zero, Quaternion.identity);
                herbivorousObject.GetComponent<Agent>().GiveBrain(Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]));
                //herbivorousObject.GetComponent<Agent>().brain.InitializeBrain(FindObjectOfType<Academy>(), null);
                herbivorousObject.transform.parent = Ground.transform;
                herbivorousObject.SetActive(true);
                //herbivorousObject.GetPhotonView().TransferOwnership(0);
                //herbivorousObject.AddComponent<PhotonView>();
                //herbivorousObject.AddComponent<PhotonTransformView>();
                //herbivorousObject.GetComponent<LivingBeingAgent>().ResetPosition(Ground.transform);
                
                
                
                /*
                var carnivorousObject = CarnivorousPool.GetObject();
                carnivorousObject.transform.parent = Ground.transform;
                carnivorousObject.SetActive(true);
                carnivorousObject.GetComponent<LivingBeingAgent>().ResetPosition(Ground.transform);
                */
                
                var herbObject = HerbPool.GetObject();
                herbObject.transform.parent = Ground.transform;
                herbObject.SetActive(true);
                //PhotonNetwork.InstantiateSceneObject("Agent", position, Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f), 0, instantiationData);
            }
        }
        

    }
}
