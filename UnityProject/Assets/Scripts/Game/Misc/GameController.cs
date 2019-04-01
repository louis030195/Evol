using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Heuristic.StateMachine;
using Evol.Utils;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

namespace Evol.Game.Misc
{
    public class GameController : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Tooltip("GameObject that contains all the map assets in the scene")] public GameObject map;
        
        [Header("Spawnables")]
        [Tooltip("Prefabs of the characters")] public List<GameObject> characters;
        [Tooltip("Prefabs of the npcs")] public GameObject[] npcs;
        [Tooltip("Prefabs of the mobs")] public GameObject[] mobs;
        [Tooltip("Prefab of the guards")] public GameObject[] guards;
        
        private enum GameState
        {
            Playing,
            Won,
            Lost
        }

        private GameState gameState = GameState.Playing;

        private void Awake()
        {
            // Makes the scene independent for debugging (not having to start all scenes everytime)
            if (!PhotonNetwork.InRoom)
                PhotonNetwork.ConnectUsingSettings();
        }

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(WaitBeingInARoom());      
        }
        
        void OnEnable()
        {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
         
        void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            print("Level Loaded");
            print(scene.name);
            print(mode);
        }


        private IEnumerator WaitBeingInARoom()
        {
            yield return new WaitUntil(() => PhotonNetwork.InRoom); // For debug + prod
            
            if (PlayerManager.LocalPlayerInstance == null)
            {
                print($"We are Instantiating LocalPlayer from {SceneManagerHelper.ActiveSceneName}");
                // We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                SpawnPlayer(PhotonNetwork.LocalPlayer);
            }
            else
            {
                print($"Ignoring scene load for {SceneManagerHelper.ActiveSceneName}");
            }

            if (PhotonNetwork.IsMasterClient)
            {
                // Spawn npc
                var npc = Instantiate(npcs[0], Position.AboveGround(
                        Position.RandomPositionAround(new Vector3(0, 0, 0), 5),
                        1),
                    Quaternion.identity);
                npc.GetComponent<StateController>().SetupAi(true);

                // Set as child of map object
                npc.transform.parent = map.transform;
                
                // Spawn mobs
                foreach (var i in Enumerable.Range(0, 100))
                {
                    // Randomgo is just useful to avoid exception when the array is empty
                    var randomGo = mobs.Length > 0 ? mobs[Random.Range(0, mobs.Length)] : null;
                    if (randomGo)
                    {
                        var mob = Instantiate(randomGo,
                            Position.AboveGround(
                                Position.RandomPositionAround(new Vector3(400, 0, 700), 200), // Hardcoded Boss position
                                1),
                            Quaternion.identity);
                        mob.GetComponent<StateController>().SetupAi(true);

                        // Set as child of map object
                        mob.transform.parent = map.transform;
                    }

                    randomGo = guards.Length > 0 ? guards[Random.Range(0, guards.Length)] : null;
                    if (randomGo)
                    {
                        var guard = Instantiate(guards[Random.Range(0, guards.Length)],
                            Position.AboveGround(Position.RandomPositionAround(Vector3.zero, 10),
                                1),
                            Quaternion.identity);
                        guard.GetComponent<StateController>().SetupAi(true);

                        // Set as child of map object
                        guard.transform.parent = map.transform;
                    }
                }

                StartCoroutine(GameLoop());
            }
        }

        private void SpawnPlayer(Photon.Realtime.Player player)
        {
            // Retrieve the chosen character id
            var characterId = player.CustomProperties.ContainsKey("character") ?
                Convert.ToInt32(player.CustomProperties["character"]) :
                2;
            
            // Retrieve the prefab assiocated to this id
            var foundPrefab = characters.Find(c => c.GetComponent<CastBehaviour>().characterData.Id == characterId);
            
            // Instanciate the player
            PhotonNetwork.Instantiate(foundPrefab.name, new Vector3(0, 50, 0), Quaternion.identity);
        }
        

        public override void OnConnectedToMaster()
        {
            print("Connected to master, creating room");
            PhotonNetwork.CreateRoom("");
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            // He is in character selection
            // Once level loaded, spawn him
            StartCoroutine(WaitPlayerEnterLevel(newPlayer));
        }

        private IEnumerator WaitPlayerEnterLevel(Photon.Realtime.Player player)
        {
            yield return new WaitUntil(() => true);
        }
        
        

        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {
            /*
              BTW
                IEnumerator RunInSequence()
                {
                   yield return StartCoroutine(Coroutine1());
                   yield return StartCoroutine(Coroutine2());
                }
                
                public void RunInParallel()
                {
                   StartCoroutine(Coroutine1());
                   StartCoroutine(Coroutine1());
                }
             */

            // Start off by running the 'GameStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(GameStarting());
            
            // Once the 'GameStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(GamePlaying());

            // Once execution has returned here, run the 'GameEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine(GameEnding());
            
            // Should run this in a loop ?
            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            switch (gameState)
            {
                case GameState.Won:
                    // If there is a game winner, what are we doing ?
                    // Show stats idk
                    // Send RPC asking to go back to main menu to everyone
                    break;
                case GameState.Lost:
                    // If game is lost, what are we doing ?
                    // Show stats idk
                    // Send RPC asking to go back to main menu to everyone
                    break;
                case GameState.Playing:
                    // If there isn't a winner yet, restart this coroutine so the loop continues.
                    // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                    // StartCoroutine(GameLoop());
                    break;
            }
        }

        private IEnumerator GameStarting()
        {
            // photonView.RPC("UpdateText", RpcTarget.All, "Waiting more players or press Space to play solo");
            // mainText.text = "Press space to start";
            // Wait other players and that everyone is ready, we also check if nobody has every joined
            /*
            while (nobodyJoinedYet && 
                   (PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("ready") && p.CustomProperties["ready"].Equals("true"))
                   < PhotonNetwork.CountOfPlayers))
            {
                yield return null;
            }*/
            
            gameState = GameState.Playing;
            // photonView.RPC("UpdateText", RpcTarget.All, "Kill them all");
            // mainText.text = "Game starting";
            print("Game starting");

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(2);
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
                // mainText.text = "GAME OVER";
                print("Game over");
            }
            
            if(gameState == GameState.Won)
            {
                // mainText.text = "VICTORY";
                print("Victory");
            }
            
            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(2);
        }

        /// <summary>
        /// Photon event callback
        /// Should be used for rare event, for frequent event, prefer PunRPC
        /// </summary>
        /// <param name="photonEvent"></param>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == 0)
            {
                // Boss died ?
                if((photonEvent.CustomData as object[])[0].Equals("Boss"))
                    gameState = GameState.Won;
                // If all player are dead (CountOfPlayers = 0 always in single player ? to investigate that)
                // checking that its a player that died just in case (and required for single player)
                if (PhotonNetwork.CountOfPlayers == 0 && (photonEvent.CustomData as object[])[0].Equals("Player"))
                    gameState = GameState.Lost;
                print($" { (photonEvent.CustomData as object[])[0] } died");
            }
        }
    }
}