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
using Evol.Heuristic.StateMachine;
using Evol.Utils;
using ExitGames.Client.Photon;
using MLAgents;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.SocketServer;
using TMPro;
using UnityEngine.Experimental.UIElements;
using Random = UnityEngine.Random;

namespace Evol.Game.Networking
{
    public enum GameState
    {
        Playing,
        Won,
        Lost
    }
    
    public class Server : MonoBehaviourPunCallbacks, IOnEventCallback
    {

        public bool IsServer;

        /// <summary>
        /// Prefabs for the different characters, set in the same order than mainmenu int value
        /// </summary>
        public GameObject[] PlayerPrefabs;
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
        public Pool HerbivorousPool;
        public Pool CarnivorousPool;
        public Pool HerbPool;
        /// <summary>
        /// The maximum number of players in game
        /// </summary>
        [Tooltip("The maximum number of players in game")]
        public byte MaxPlayersPerRoom = 4;
        public GameObject mainCanvas;
        [Header("Game loop parameters")]
        public float startWait = 2;
        public float endWait = 2;
    
        private bool rainFinished = true;
        private GameState gameState = GameState.Playing;
        private Text mainText;
        /// <summary>
        /// Just a variable to check if anybody joined the game yet
        /// </summary>
        private bool nobodyJoinedYet = true;
        
        protected virtual void Start()
        {
            
            if(!IsServer)
               Destroy(this); // Destroy the server script if not server
            
            mainText = mainCanvas.GetComponentInChildren<Text>();
            // PhotonNetwork.SendRate = 60;
            // PhotonNetwork.SerializationRate = 60;
            
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

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            nobodyJoinedYet = false;
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
            
            yield return new WaitForSeconds(duration);
        
            while(rainScript.RainIntensity >= 0)
            {
                rainScript.RainIntensity -= 0.01f;
                light.intensity += 0.005f;
            
                yield return new WaitForSeconds(0.1f);
            }

            rainFinished = true;
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

        



        public void Initialize(bool network = false)
        {
            // HerbivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Herbivorous")), network);
            // CarnivorousPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Carnivorous")), network);
            // HerbPool = new Pool(SpawnablePrefabs.Find(prefab => prefab.CompareTag("Herb")), network);
            // StartCoroutine(SpawnAgents());
            foreach(var i in Enumerable.Range(0, 10))
            {
                var go = Instantiate(SpawnablePrefabs.Find(s => s.name.Equals("WolfSM")),
                    Position.AboveGround(Position.RandomPositionAround(Vector3.zero, 200),
                        SpawnablePrefabs.Find(s => s.name.Equals("WolfSM")).GetComponent<Collider>().bounds.size.y),
                    Quaternion.identity);
                go.GetComponent<StateController>().SetupAi(true, new List<Transform>());
            }

            StartCoroutine(GameLoop());
        }
        
        protected GameObject SpawnHerbivorous()
        {
            var herbivorousObject = HerbivorousPool.GetObject();
            herbivorousObject.transform.position = Position.RandomPositionAround(Vector3.zero, Ground.GetComponent<Terrain>().terrainData.size.x / 4);
            //var herbivorousObject = PhotonNetwork.InstantiateSceneObject(SpawnablePrefabs.Find(go => go.CompareTag("Herbivorous")).name, Vector3.zero, Quaternion.identity);
            herbivorousObject.transform.parent = Ground.transform;
            
            herbivorousObject.SetActive(true);

            return herbivorousObject;
        }
        
        protected GameObject SpawnCarnivorous()
        {
            var carnivorousObject = CarnivorousPool.GetObject();
            carnivorousObject.transform.position = Position.RandomPositionAround(Vector3.zero, Ground.GetComponent<Terrain>().terrainData.size.x / 4);
            carnivorousObject.transform.parent = Ground.transform;
            
            carnivorousObject.SetActive(true);

            return carnivorousObject;
        }
        
        protected GameObject SpawnHerb()
        {
            var herbObject = HerbPool.GetObject();
            herbObject.transform.position = Position.RandomPositionAround(Vector3.zero, Ground.GetComponent<Terrain>().terrainData.size.x / 4);
            herbObject.transform.parent = Ground.transform;
            
            herbObject.SetActive(true);

            return herbObject;
        }
        

        protected virtual IEnumerator SpawnAgents()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0, 10));
                // SpawnCarnivorous();
                // SpawnHerbivorous();
                // SpawnHerb();
                

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
           
        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {

            // Start off by running the 'GameStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(GameStarting());
            
            // Once the 'GameStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(GamePlaying());

            // Once execution has returned here, run the 'GameEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine(GameEnding());
            /*
            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            switch (gameState)
            {
                case GameState.Won:
                    // If there is a game winner, what are we doing ?

                    break;
                case GameState.Lost:
                    // If game is lost, what are we doing ?

                    break;
                case GameState.Playing:
                    // If there isn't a winner yet, restart this coroutine so the loop continues.
                    // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                    StartCoroutine(GameLoop());
                    break;
            }*/
        }

        private IEnumerator GameStarting()
        {
            // photonView.RPC("UpdateText", RpcTarget.All, "Waiting more players or press Space to play solo");
            mainText.text = "Press space to start";
            // Wait other players and that everyone is ready, we also check if nobody has every joined
            while (nobodyJoinedYet && 
                   (PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("ready") && p.CustomProperties["ready"].Equals("true"))
                   < PhotonNetwork.CountOfPlayers))
            {
                yield return null;
            }
            
            gameState = GameState.Playing;
            // photonView.RPC("UpdateText", RpcTarget.All, "Kill them all");
            mainText.text = "Game starting";

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(startWait);
        }


        private IEnumerator GamePlaying()
        {
            while (gameState == GameState.Playing)
            {
                yield return null;
                /*
                // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
                yield return StartCoroutine(RoundStarting());

                // ... return on the next frame.
                yield return StartCoroutine(RoundPlaying());
                if (GameFinished())
                    yield break;

                yield return StartCoroutine(RoundEnding());
                */
            }
        }

        private IEnumerator GameEnding()
        {
            // Stop from moving.
            // DisableControl();
            
            if (gameState == GameState.Lost)
            {
                mainText.text = "GAME OVER";
            }
            
            if(gameState == GameState.Won)
            {
                mainText.text = "VICTORY";
            }
            
            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(endWait);
        }

        public void OnEvent(EventData photonEvent)
        {
            // Ready event
            if (photonEvent.Code == 0)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    print($"{player.ActorNumber} : {photonEvent.Sender}");
                }
                // If the player is not ready
                if (!PhotonNetwork.PlayerList.First(p => p.ActorNumber == photonEvent.Sender).CustomProperties.ContainsKey("ready") ||
                    PhotonNetwork.PlayerList.First(p => p.ActorNumber == photonEvent.Sender).CustomProperties["ready"].Equals("false"))
                {
                    PhotonNetwork.PlayerList.First(p => p.ActorNumber == photonEvent.Sender).CustomProperties["ready"] =
                        "true"; // We add him to ready players
                    print($"Player ${ photonEvent.Sender } is ready");
                }
                else
                {
                    PhotonNetwork.PlayerList.First(p => p.ActorNumber == photonEvent.Sender).CustomProperties["ready"] =
                        "false"; // Else we remove him from ready players
                    print($"Player ${ photonEvent.Sender } is not ready anymore");
                }
            }

            if (photonEvent.Code == 1)
            {
                // Boss died ?
                if((photonEvent.CustomData as object[])[0].Equals("Boss"))
                    gameState = GameState.Won;
                // If all player are dead (CountOfPlayers = 0 always in single player ? to investigate that)
                // checking that its a player that died just in case (and required for single player)
                if (PhotonNetwork.CountOfPlayers == 0 && (photonEvent.CustomData as object[])[0].Equals("Player"))
                    gameState = GameState.Lost;
                print($"Player ${ photonEvent.Sender } : { (photonEvent.CustomData as object[])[0] } died");
            }
        }

        [PunRPC]
        public void UpdateText(string text)
        {
            mainText.text = text;
        }
    }
}
