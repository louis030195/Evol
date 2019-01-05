using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DigitalRuby.LightningBolt;
using DigitalRuby.RainMaker;
using Evol.Agents;
using Evol.Utils;
using MLAgents;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.SocketServer;
using Random = UnityEngine.Random;

namespace Evol.Game.Networking
{
    public class Server : MonoBehaviourPunCallbacks
    {

        public bool IsServer;

        public Academy Academy;
        /// <summary>
        /// Prefabs for the different characters, set in the same order than mainmenu int value
        /// </summary>
        public GameObject[] PlayerPrefabs;
        public GameObject CameraPrefab;
        public List<GameObject> SpawnablePrefabs;
        
        /// <summary>
        /// This list of brains will be used when we want to switch for example from a trained model to an imitation learning
        /// or switching from trained model to training model (control ON)
        /// </summary>
        public List<Brain> Brains;

        public Light light;
        public GameObject Lightning;
        public RainScript rainScript;
        public GameObject Ground;
        public GameObject Test;
        public Pool HerbivorousPool;
        public Pool CarnivorousPool;
        public Pool HerbPool;
    
        protected List<GameObject> players;
        private bool rainFinished = true;

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


            // Basically turning off communication with python, we only want these brains to use the pre-trained model
            //Academy.broadcastHub.SetControlled(Academy.broadcastHub.broadcastingBrains.Find(brain => brain.name.Contains("Learning")), false);

            // TODO: this is for imitation learning, doesn't work, doesn't start the process ???
            //Arguments = "mlagents-learn /mnt/sdb/ML/ml-agents-master/config/trainer_config.yaml --train --slow"
            /*
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "/mnt/sdb/ML/ml-agents-master/evol/bin/python3",
                    WindowStyle = ProcessWindowStyle.Maximized,
                    UseShellExecute = false,
                    Arguments = "mlagents-learn /mnt/sdb/ML/ml-agents-master/config/trainer_config.yaml --train --slow"
                }
            };
            process.Start();*/


        }


        protected virtual void Update()
        {
            if(Time.frameCount % Random.Range(3, 6) == 0 && rainFinished)
                StartCoroutine(SlowlyStartRaining());
        }
        
        /// <summary>
        /// TODO: Larger rain zone ? laggy ?
        /// </summary>
        /// <returns></returns>
        private IEnumerator SlowlyStartRaining ()
        {
            rainFinished = false;
            var rainIntensity = Random.Range(0.1f, 1.0f);
            while(rainScript.RainIntensity < rainIntensity)
            {
                rainScript.RainIntensity += 0.01f;
                light.intensity -= 0.005f;
            
                yield return new WaitForSeconds(0.1f);
            }


            var duration = Random.Range(3f, 60f);
            
            if (rainIntensity > 0.5f)
            {
                StartCoroutine(ThrowLightning(duration));
            }
            
            print($"Reached the target : {rainIntensity}, raining for {duration}");
            
            yield return new WaitForSeconds(duration);
        
            while(rainScript.RainIntensity >= 0)
            {
                rainScript.RainIntensity -= 0.01f;
                light.intensity += 0.005f;
            
                yield return new WaitForSeconds(0.1f);
            }

            rainFinished = true;
            
            print($"Rain is now finished.");
        }

        private IEnumerator ThrowLightning(float duration)
        {
            while(duration > 0){
                var lightning = Instantiate(Lightning);
                var randomX = Random.Range(-100, 100);
                var randomZ = Random.Range(-100, 100);
                lightning.GetComponent<LightningBoltScript>().StartPosition = new Vector3(randomX, 100, randomZ);
                lightning.GetComponent<LightningBoltScript>().EndPosition = new Vector3(randomX, 0, randomZ);
                Destroy(lightning, 3f);
                duration--;
                yield return new WaitForSeconds(Random.Range(1f, 10f));
            }
        }

        



        protected void Initialize()
        {
            //PlayerPool = new Pool(PlayerPrefab);
            HerbivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Herbivorous")));
            CarnivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Carnivorous")));
            HerbPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Herb")));

            StartCoroutine(SpawnAgents());

            //StartCoroutine(SpawnTree());
        }
        
        private Vector3 RandomCircle (Vector3 center , float radius){
            var ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            
            // Check if the object is above ground, if not, we lift it to the ground
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.up, out hit, Mathf.Infinity))
            {
                transform.position = new Vector3(pos.x, hit.distance + 0.1f, pos.z);
            }
            
            return pos;
        }

        private Vector3 RandomPositionAround()
        {
            var pos = Random.insideUnitCircle * Ground.GetComponent<Terrain>().terrainData.size.x / 4;
            
            // Check if the object is above ground, if not, we lift it to the ground
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(pos.x, 0, pos.y), Vector3.up, out hit, Mathf.Infinity))
            {
                transform.position = new Vector3(pos.x, hit.distance + 0.1f, pos.y);
            }

            return pos;
        }
        
        protected GameObject SpawnHerbivorous()
        {
            var herbivorousObject = HerbivorousPool.GetObject();
            herbivorousObject.transform.position = RandomCircle(Vector3.zero, Ground.GetComponent<Terrain>().terrainData.size.x / 4);
            //var herbivorousObject = PhotonNetwork.InstantiateSceneObject(SpawnablePrefabs.Find(go => go.CompareTag("Herbivorous")).name, Vector3.zero, Quaternion.identity);
            herbivorousObject.transform.parent = Ground.transform;
            
            herbivorousObject.SetActive(true);

            return herbivorousObject;
        }
        
        protected GameObject SpawnCarnivorous()
        {
            var carnivorousObject = CarnivorousPool.GetObject();
            carnivorousObject.transform.position = RandomCircle(Vector3.zero, Ground.GetComponent<Terrain>().terrainData.size.x / 4);
            carnivorousObject.transform.parent = Ground.transform;
            
            carnivorousObject.SetActive(true);

            return carnivorousObject;
        }
        
        protected GameObject SpawnHerb()
        {
            var herbObject = HerbPool.GetObject();
            herbObject.transform.position = RandomCircle(Vector3.zero, Ground.GetComponent<Terrain>().terrainData.size.x / 4);
            herbObject.transform.parent = Ground.transform;
            
            herbObject.SetActive(true);

            return herbObject;
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
        
        /*
        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {

            // Start off by running the 'GameStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(GameStarting());

            // Once the 'GameStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(GamePlaying());

            // Once execution has returned here, run the 'GameEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine(GameEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            switch (gameState)
            {
                case GameState.Won:
                    // If there is a game winner, restart the level.

                    // Forces the server to shutdown.
                    NetworkManager.singleton.StopHost();
                    NetworkManager.singleton.StopClient();
                    Shutdown();

                    // Reset internal state of the server and start the server again.
                    Start();
                    ServerChangeScene("Main");

                    break;
                case GameState.Lost:
                    // If game is lost, restart the level.


                    NetworkManager.singleton.StopHost();
                    NetworkManager.singleton.StopClient();
                    Shutdown();

                    // Reset internal state of the server and start the server again.
                    Start();
                    ServerChangeScene("Main");

                    //NetworkManager.singleton.StopHost();
                    //NetworkManager.singleton.StopClient();
                    break;
                case GameState.Playing:
                    // If there isn't a winner yet, restart this coroutine so the loop continues.
                    // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                    StartCoroutine(GameLoop());
                    break;
            }
        }

        private IEnumerator GameStarting()
        {

            countAi = 0;
            roundNumber = 0;

            messageText.text = "Waiting more players or press Space to play solo";


            // Wait other players
            
            while (NetworkServer.connections.Count < 2)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Space))
                    break;
            }
            
            gameState = GameState.Playing;
            messageText.text = "Kill them all";


            // Wait for the specified length of time until yielding control back to the game loop.
            yield return startWait;
        }


        private IEnumerator GamePlaying()
        {

            nexus = (GameObject)Instantiate(spawnPrefabs[9], new Vector3(0, 0.5f, 0), Quaternion.identity);
            NetworkServer.Spawn(nexus);

            while (true)
            {
                // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
                yield return StartCoroutine(RoundStarting());

                // ... return on the next frame.
                yield return StartCoroutine(RoundPlaying());
                if (GameFinished())
                    yield break;

                yield return StartCoroutine(RoundEnding());
            }
        }

        private IEnumerator GameEnding()
        {
            // Stop from moving.
            // DisableControl();
            if (GameFinished() && !victory)
            {
                messageText.text = "GAME OVER";
                gameState = GameState.Lost;
            }
            if(victory)
            {
                messageText.text = "Win";
                gameState = GameState.Won;
            }

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return endWait;
        }
        */

    }
}
