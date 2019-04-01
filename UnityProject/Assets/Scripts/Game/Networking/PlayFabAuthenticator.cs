using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using AuthenticationValues = Photon.Realtime.AuthenticationValues;
using Random = System.Random;

namespace Evol.Game.Networking
{
    // TODO: THINK ABOUT SPLITTING UI STUFF INTO MULTIPLE FILES
    public class PlayFabAuthenticator : MonoBehaviourPunCallbacks
    {
        [Header("Parameters")] 
        [Tooltip("When the game is started should we allow other players to join")] public bool lockRoomOnStart = true;
        
        [Header("Login / Register fields")] 
        public GameObject loginRegisterCanvas;
        public Button loginButton;
        public Button registerButton;
        public InputField username;
        public InputField password;
        public InputField email;
        public TextMeshProUGUI result;
        private LoginWithPlayFabRequest loginRequest;

        [Header("Main menu fields")] 
        public GameObject mainMenuCanvas;
        [Tooltip("Main content at the centre of the screen")] public GameObject mainContent;
        [Tooltip("Nav at the top used to navigate in the main content")] public GameObject mainNav;
        [Tooltip("Bar at the bottom used for specific less used stuff")] public GameObject bottomBar;
        [Tooltip("Back arrow displayed in dead end screens used to go to previous screen")] public GameObject backArrow;
        [Tooltip("Evol logo")] public GameObject logo;
        
        [Header("Main mode layout")]
        [Tooltip("Layout that contain everything to start finding a game")] public GameObject gameConfig;
        [Tooltip("Connection state to master server")] public TextMeshProUGUI gameConfigConnectionState;
        [Tooltip("Button to start finding a game")] public Button gameConfigFindGameButton;
        [Tooltip("Layout that contain everything to select a char after joining a game")] public GameObject characterSelection;
        [Tooltip("Button to say ready")] public Button characterSelectionReadyButton;
        [Tooltip("Client stats with this char")] public GameObject stats;
        [Tooltip("Characters list")] public GameObject charactersList;
        [Tooltip("The layout that contains the character to be selected to play")] public GameObject characterLayout;
        [Tooltip("Looking for a game status")] public TextMeshProUGUI queueStatus;


        private float timeToWaitPlayers = 3; // Should be proportional to the total number of players currently playing

        private void Awake()
        {
            if(!PhotonNetwork.ConnectUsingSettings())
                print("Failed to connect to master");
        }

        private void Start()
        {
            // Should set all listeners in code, it makes avoiding losing time on setting it using editor
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(OnLogin);
            
            registerButton.onClick.RemoveAllListeners();
            registerButton.onClick.AddListener(OnRegister);
            
            gameConfigFindGameButton.onClick.RemoveAllListeners();
            gameConfigFindGameButton.onClick.AddListener(OnReadyToFindAGame);
            
            characterSelectionReadyButton.onClick.RemoveAllListeners();
            characterSelectionReadyButton.onClick.AddListener(OnReadyToPlay);
            
            // We reach this condition if we leave a game a go back to main menu
            if(PlayFabClientAPI.IsClientLoggedIn())
                OnLoginSuccess();
        }

        private void Update()
        {
            // if (!gameObject.GetPhotonView().IsMine)
            //    enabled = false;
        }

        public void OnLogin()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                result.text = $"Check your internet connection";
                return;
            }

            loginRequest = new LoginWithPlayFabRequest {Username = username.text, Password = password.text};
            PlayFabClientAPI.LoginWithPlayFab(loginRequest, result =>
            {
                // If the account is found
                PhotonNetwork.LocalPlayer.NickName = loginRequest.Username;
                gameObject.GetComponent<Chat>().PlayFabAuthenticationContext = result.AuthenticationContext;
                gameObject.GetComponent<Chat>().enabled = true;
                this.result.text = $"You're now logged in !";
                OnLoginSuccess();
            }, error =>
            {
                // If the account is not found
                result.text = $"Incorrect username or password";
            }, null);

        }
        
        public override void OnConnectedToMaster()
        {
            gameConfigConnectionState.text = $"Connected to server !";
            gameConfigFindGameButton.interactable = true;
            PhotonNetwork.AutomaticallySyncScene = false;
            Debug.Log($"Connected to master cloud");
        }

        private void OnLoginSuccess()
        {
            loginRegisterCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);

            // TODO: Maybe should start when game config is shown
            StartCoroutine(nameof(UpdateConnectionState));
            
            // I think here we will initialize all the custom properties of the current player used through the game
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ready"))
            {
                PhotonNetwork.LocalPlayer.CustomProperties["ready"] = "0";
            }
            else
            {
                PhotonNetwork.LocalPlayer.CustomProperties.Add("ready", "0");
            }
        }

        private IEnumerator UpdateConnectionState()
        {
            // Don't want to do useless calculation
            // TODO: DOES IT WORK ?
            var odd = true;
            yield return new WaitUntil(() => gameConfig.activeInHierarchy);
            while (!PhotonNetwork.IsConnectedAndReady)
            {
                yield return new WaitForSeconds(2f);
                gameConfigConnectionState.text =  odd ? "Trying to connect to server ..." :  
                    "Trying to connect to server ..";
                odd = !odd;
            }
        }

        public void OnRegister()
        {
            var request = new RegisterPlayFabUserRequest
            {
                Email = email.text, Username = username.text, Password = password.text
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, result =>
                {
                    this.result.text = $"Your account has been created !";
                }, error => { result.text = $"Fill all field !"; });
        }

        /// <summary>
        /// Should be called when disconnected from account and going back to login / register screen
        /// BottomBar -> Last button
        /// </summary>
        public void OnDisconnect()
        {
            loginRegisterCanvas.SetActive(true);
            mainMenuCanvas.SetActive(false);
            gameObject.GetComponent<Chat>().enabled = false;
            PhotonNetwork.Disconnect(); // There isn't a Disconnect for playfab
            result.text = $"Successfully disconnected";
        }

        /// <summary>
        /// Deactive all MainContent layouts and active the selected layout
        /// </summary>
        /// <param name="layout"></param>
        public void OnMainNav(GameObject layout)
        {
            foreach (Transform lay in mainContent.transform)
            {
                lay.gameObject.SetActive(false);
            }
            
            layout.SetActive(true);
        }

        /// <summary>
        /// This function is used for dead end screen: always same behaviour, hide logo, show back arrow, hide MainNav and BottomBar
        /// Show the wanted layout and everything call be called in reverse
        /// <param name="layout"></param>
        /// </summary>
        public void OnDeadEnd(GameObject layout)
        {
            mainNav.SetActive(!mainNav.activeInHierarchy);
            mainContent.SetActive(!mainContent.activeInHierarchy);
            bottomBar.SetActive(!bottomBar.activeInHierarchy);
            backArrow.SetActive(!backArrow.activeInHierarchy);
            logo.SetActive(!logo.activeInHierarchy);
            if (layout != null)
            {
                layout.SetActive(!layout.activeInHierarchy);
                backArrow.GetComponent<Button>().onClick.AddListener(() => // Like a Once event
                {
                    var a = new UnityAction(() => OnDeadEnd(layout));
                    a.Invoke();
                    backArrow.GetComponent<Button>().onClick.RemoveListener(a);
                });
            }
            else
                Debug.Log($"You forgot to add a layout to this dead end");
        }

        /// <summary>
        /// GameConfig -> Ready
        /// Try to find a game
        /// </summary>
        public void OnReadyToFindAGame()
        {
            // to join / create game (no friend)
            if(PhotonNetwork.JoinRandomRoom()) {
                Debug.Log("JoinRandomRoom success");
            }
        }
        
        /// <summary>
        /// GameConfig -> PlayLayout -> Ready
        /// Ready to play
        /// </summary>
        public void OnReadyToPlay()
        {
            // Means that we can just start whenever we want and join others room
            if(!lockRoomOnStart)
                PhotonNetwork.LoadLevel("Game");
            
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ready"))
            {
                PhotonNetwork.LocalPlayer.CustomProperties["ready"] =
                    PhotonNetwork.LocalPlayer.CustomProperties["ready"].Equals("1") ? "0" : "1";
            }
            else
            {
                PhotonNetwork.LocalPlayer.CustomProperties.Add("ready", "1");
            }
        }




        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRandomFailed { returnCode } - { message }");
            if (returnCode == 32760) // No match found
            {
                queueStatus.text = $"No games available, creating one";
                if (PhotonNetwork.CreateRoom(new Random().Next(0, 100).ToString(), new RoomOptions(), TypedLobby.Default))
                {

                    
                }
            }
        }

        public override void OnCreatedRoom()
        {
            LoadPlayLayout();
            // Only the master should wait & start everyone game
            StartCoroutine(WaitPlayersAndStart());
        }

        private IEnumerator WaitPlayersAndStart()
        {
            queueStatus.text = $"Press ready to start";
            
            if(!lockRoomOnStart)
                yield break;
                        
            // Wait until all players are ready
            yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.Players.All(p => p.Value.CustomProperties["ready"].Equals("1")));

            // Lock the room on start
            PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            gameObject.GetPhotonView().RPC(nameof(LoadLevelForEveryone), RpcTarget.All, "Game");
        }

        private void LoadPlayLayout()
        {
            gameConfig.SetActive(false);
            characterSelection.SetActive(true);
        }
        
        public override void OnJoinedRoom()
        {
            queueStatus.text = $"Joined a game";
            LoadPlayLayout();
            // Wait a bit other players then start
            Debug.Log($"OnJoinedRoom");
        }

        [PunRPC]
        public void LoadLevelForEveryone(string scene)
        {
            PhotonNetwork.LoadLevel(scene);
        }
        

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRoomFailed { returnCode } - { message }");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // I guess it could happen to be null if we are debugging and didn't pass by login scene ?
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                // Persist player data
                var playerData = new Dictionary<string, string>();
                foreach (var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
                {
                    playerData.Add((string) key, (string) PhotonNetwork.LocalPlayer.CustomProperties[key]);
                }
                // TODO: Think to not push all custom properties, there is some properties that shouldn't be persisted (ready for example)
                if (playerData.Count > 0) // We don't always add player data (just logging, disconecting ....)
                {
                    PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                        {
                            AuthenticationContext = loginRequest.AuthenticationContext,
                            Data = playerData
                        }, result => { Debug.Log($"UpdateUserData succeed - {result}"); },
                        error => { Debug.Log($"UpdateUserData failed - {error}"); });
                }
            }

            Debug.Log($"Disconnected from photon cloud { cause }");	
            queueStatus.text = $"Failed to connect to the server { cause }";
        }
    }
}