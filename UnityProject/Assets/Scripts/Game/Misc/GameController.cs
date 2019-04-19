using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Networking;
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
    
    // IMPORTANT THING ABOUT PHOTON
    /*
     * By default, PUN instantiate uses the DefaultPool,
     * which loads prefabs from Resources folders and Destroys the GameObject later on.
     * A more sophisticated IPunPrefabPool implementation can return objects to a pool in Destroy and re-use them in Instantiate.
     * In that case, the GameObjects are not truly created in Instantiate, which means that Start() is not being called by Unity in such a case.
     * Due to this, scripts on networked game objects should just implement OnEnable and OnDisable
     */
    // SO BE CAREFUL WITH NETWORK STUFF AND START / ENABLE
    
    public class GameController : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("Game parameters")]
        [Tooltip("Pause time between rounds in seconds")] public int timeBetweenRound = 30;
        [Tooltip("Number of AIs spawned at every round")] public int aiPerRound = 30;
        [Tooltip("Delay between AIs spawns")] public float delayBetweenAiSpawn = 0.1f;
        [Tooltip("Number of guards to spawn")] public int numberOfGuards = 10;

        
        [Header("Spawnables")]
        [Tooltip("GameObject that contains all the map assets in the scene")] public GameObject map;
        [Tooltip("Prefabs of the characters")] public List<GameObject> characters;
        [Tooltip("Prefabs of the npcs")] public GameObject[] npcs;
        [Tooltip("Prefabs of the mobs")] public GameObject[] mobs;
        [Tooltip("Prefab of the guards")] public GameObject[] guards;
        [Tooltip("Prefab of the items")] public GameObject[] items;
        [Tooltip("Main text sync for all players")] public TextMeshProUGUI mainText;

        private enum GameState
        {
            Playing,
            Won,
            Lost
        }

        private GameState gameState = GameState.Playing;
        private int playersAlive, aisAlive, aiSpawned, roundNumber;
        private float difficulty;

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
                // Get the difficulty chosen
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("difficulty"))
                    difficulty = (int) PhotonNetwork.CurrentRoom.CustomProperties["difficulty"] / 3; // 1, 2, 3 => / 3
                else // Should only happen in debug mode (start from game scene)
                    difficulty = 1;
                
                // Spawn npc
                if (npcs.Length > 0)
                {
                    var npc = Instantiate(npcs[0], Position.AboveGround(
                            Position.RandomPositionAround(new Vector3(0, 0, 0), 5),
                            1),
                        Quaternion.identity);
                    npc.GetComponent<StateController>().SetupAi(true);

                    // Set as child of map object
                    npc.transform.parent = map.transform;
                }

                // Spawn mobs
                foreach (var i in Enumerable.Range(0, numberOfGuards))
                {
                    var randomGo = guards.Length > 0 ? guards[Random.Range(0, guards.Length)] : null;
                    if (randomGo)
                    {
                        var guard = PhotonNetwork.InstantiateSceneObject(randomGo.name,
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
                3;
            
            // Retrieve the prefab assiocated to this id
            var foundPrefab = characters.Find(c => c.GetComponent<PlayerManager>().characterData.id == characterId);
            
            // Instanciate the player
            PhotonNetwork.Instantiate(foundPrefab.name, new Vector3(0, 50, 0), Quaternion.identity);
        }
        

        public override void OnConnectedToMaster()
        {
            print("Connected to master, creating room");
            PhotonNetwork.CreateRoom("");
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
            // Set the total number of players alive
            playersAlive = PhotonNetwork.CurrentRoom.PlayerCount;
            // Wait other players and that everyone is ready, we also check if nobody has every joined
            /*
            while (nobodyJoinedYet && 
                   (PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("ready") && p.CustomProperties["ready"].Equals("true"))
                   < PhotonNetwork.CountOfPlayers))
            {
                yield return null;
            }*/
            
            gameState = GameState.Playing;
            StartCoroutine(UpdateTextAndDisappear($"Game starting !", 2f));
            /*
            foreach(var i in Enumerable.Range(0, 10)) {
                // Spawn random items
                var randomGo = items.Length > 0 ? items.PickRandom() : null;
                print(randomGo.name);
                var go = PhotonNetwork.Instantiate(randomGo.name,
                    Position.AboveGround(Position.RandomPositionAround(Vector3.zero, 10), 1),
                    Quaternion.identity);
            }*/

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(2);
        }


        private IEnumerator GamePlaying()
        {
            while (true)
            {
                // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
                yield return StartCoroutine(RoundStarting());

                // ... return on the next frame.
                yield return StartCoroutine(RoundPlaying());
                if (gameState != GameState.Playing)
                    yield break;

                yield return StartCoroutine(RoundEnding());
            }
        }

        private IEnumerator RoundStarting()
        {
            yield return null;
            aisAlive = 0;
            aiSpawned = 0;
            roundNumber++;
            StartCoroutine(UpdateTextAndDisappear("Round starting", 2f));
        }
        
        private IEnumerator RoundPlaying()
        {
            do
            {
                // ... return on the next frame.
                if (gameState != GameState.Playing)
                    yield break;
                yield return StartCoroutine(SpawnAllAi());

            } while (aisAlive > 0);
        }

        private IEnumerator SpawnAllAi()
        {
            // The number of AIs increase of aiPerRound + aiPerRound * 30% for example (1st round then 60%) in easy mode
            var aiPerRoundByDifficulty = aiPerRound + aiPerRound * (roundNumber * difficulty);
            if (aiSpawned < aiPerRoundByDifficulty)
            {
                // print($"AiPerRoundByDiffuculty {aiPerRoundByDifficulty}");
                // Randomgo is just useful to avoid exception when the array is empty
                var randomGo = mobs.Length > 0 ? mobs[Random.Range(0, mobs.Length)] : null;
                if (randomGo)
                {
                    // InstanciateSceneObject makes the photon view belongs to the scene
                    // It is useful to avoid the object being destroyed if the master client leave
                    // Because normal PhotonNetwork.Instanciate() makes the object belongs to the master
                    var mob = PhotonNetwork.InstantiateSceneObject(randomGo.name,
                        Position.AboveGround(
                            Position.RandomPositionAround(new Vector3(200, 0, 300), 200),
                            1),
                        Quaternion.identity);
                    mob.GetComponent<StateController>().SetupAi(true);

                    // TODO: implement an extension to generate non uniform random distribution (more chance to have 0, 1 items than more ...)
                    // Add some items to the loot of that monster
                    var loot = mob.GetComponent<Loot>();
                    if(loot) loot.items = new List<GameObject>(items.PickRandom(Random.Range(0, 10)));

                    // Set as child of map object
                    mob.transform.parent = map.transform;

                    // Increment the AI alive / spawned counters
                    aisAlive++;
                    aiSpawned++;
                }
            }

            yield return new WaitForSeconds(delayBetweenAiSpawn);
        }
        
        private IEnumerator RoundEnding()
        {
            StartCoroutine(UpdateTextAndDisappear("Round ending", 2f));
            yield return new WaitForSeconds(timeBetweenRound);
        }

        private IEnumerator GameEnding()
        {
            // Stop from moving.
            // DisableControl();
            
            if (gameState == GameState.Lost)
            {
                StartCoroutine(UpdateTextAndDisappear($"Game over !", 2f));
                print("lose");
            }
            
            if(gameState == GameState.Won)
            {
                StartCoroutine(UpdateTextAndDisappear($"Victory !", 2f));
                print("win");
            }
            
            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(2);
        }

        private IEnumerator UpdateTextAndDisappear(string text, float delay)
        {
            photonView.RPC(nameof(UpdateText), RpcTarget.All, text);
            yield return new WaitForSeconds(delay);
            photonView.RPC(nameof(UpdateText), RpcTarget.All, "");
        }

        [PunRPC]
        private void UpdateText(string text)
        {
            mainText.text = text;
        }

        // TODO: implement this to handle when master client leave game, take the game loop ...
        // https://doc.photonengine.com/en-us/pun/current/gameplay/hostmigration
        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
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
                // Assuming that round mobs are all tagged monster, decrement when one is dead
                if ((photonEvent.CustomData as object[])[0].Equals("Monster"))
                    aisAlive--;
                // Boss died ?
                if((photonEvent.CustomData as object[])[0].Equals("Boss"))
                    gameState = GameState.Won;
                // If all player are dead
                // checking that its a player that died just in case
                if ((photonEvent.CustomData as object[])[0].Equals("Player"))
                    playersAlive--;
                // If all players are dead or the nexus is dead
                if (playersAlive == 0 || (photonEvent.CustomData as object[])[0].Equals("Nexus"))
                    gameState = GameState.Lost;
            }
        }
    }
}