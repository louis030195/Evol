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
        public GameObject CameraPrefab;
        public List<GameObject> SpawnablePrefabs;
        public List<Brain> Brains;
        public GameObject Ground;
        public GameObject Test;
        public Pool HerbivorousPool;
        public Pool CarnivorousPool;
        public Pool HerbPool;
    
        protected List<GameObject> players;

        /// <summary>
        /// The maximum number of players in game
        /// </summary>
        [Tooltip("The maximum number of players in game")]
        public byte MaxPlayersPerRoom = 4;


        protected virtual void Start()
        {
            if(!IsServer)
                Destroy(this); // Destroy the server script if not server
            players = new List<GameObject>();  
            
            
        }


        protected void Initialize()
        {
            //PlayerPool = new Pool(PlayerPrefab);
            HerbivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Herbivorous")));
            CarnivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Carnivorous")));
            HerbPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Herb")));
            
            // Find the brains in the list
            HerbivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
            CarnivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);

            // TODO: Fix this shit (animals not sync in network)
            StartCoroutine(SpawnAgents());

            //StartCoroutine(SpawnTree());
        }

        /// <summary>
        /// Remove the appropriate script following online / offline mode
        /// </summary>
        /// <param name="player">Instantiated player</param>
        protected void Mode(GameObject player)
        {
            if (PhotonNetwork.OfflineMode)
            {
                //player.GetComponent<PlayerManagerOffline>().enabled = true;
            }
            else if (!PhotonNetwork.OfflineMode)
            {
                //player.GetComponent<PlayerManagerOnline>().enabled = true;
                /*
                player.AddComponent<PhotonView>();
                player.AddComponent<PhotonTransformView>();
                player.GetComponent<PhotonTransformView>().m_SynchronizePosition = true;
                player.GetComponent<PhotonTransformView>().m_SynchronizeRotation = true;
                player.GetPhotonView().ObservedComponents.Add(player.GetComponent<PhotonTransformView>());
                */
            }
        }
        
        
        protected IEnumerator SpawnTree()
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

        protected virtual IEnumerator SpawnAgents()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0, 10));

                /*
                Vector2 direction = Random.insideUnitCircle;
                Vector3 position = Vector3.zero;

                Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);
                object[] instantiationData = { torque, true};

                //var herbivorousObject = HerbivorousPool.GetObject();
                var herbivorousObject = PhotonNetwork.InstantiateSceneObject(SpawnablePrefabs.Find(go => go.name.Equals("HerbivorousAgent")).name, Vector3.zero, Quaternion.identity);
                herbivorousObject.GetComponent<Agent>().GiveBrain(Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]));
                //herbivorousObject.GetComponent<Agent>().brain.InitializeBrain(FindObjectOfType<Academy>(), null);
                herbivorousObject.transform.parent = Ground.transform;
                herbivorousObject.SetActive(true);
                //herbivorousObject.GetPhotonView().TransferOwnership(0);
                //herbivorousObject.AddComponent<PhotonView>();
                //herbivorousObject.AddComponent<PhotonTransformView>();
                //herbivorousObject.GetComponent<LivingBeingAgent>().ResetPosition(Ground.transform);
                
                
                
                
                var carnivorousObject = CarnivorousPool.GetObject();
                carnivorousObject.transform.parent = Ground.transform;
                carnivorousObject.SetActive(true);
                carnivorousObject.GetComponent<LivingBeingAgent>().ResetPosition(Ground.transform);
                
                
                var herbObject = HerbPool.GetObject();
                herbObject.transform.parent = Ground.transform;
                herbObject.SetActive(true);
                //PhotonNetwork.InstantiateSceneObject("Agent", position, Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f), 0, instantiationData);
                */
            }
        }
        

    }
}
