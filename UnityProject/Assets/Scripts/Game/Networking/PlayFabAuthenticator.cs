using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
        public TMP_InputField username;
        public TMP_InputField password;
        public TMP_InputField email;
        public TextMeshProUGUI result;
        public GameObject chat;
        private LoginWithPlayFabRequest loginRequest;

        [Header("Main menu fields")] 
        public GameObject mainMenuCanvas;
        [Tooltip("Main content at the centre of the screen")] public GameObject mainContent;
        [Tooltip("Nav at the top used to navigate in the main content")] public GameObject mainNav;
        //[Tooltip("Bar at the bottom used for specific less used stuff")] public GameObject bottomBar;
        [Tooltip("Back arrow displayed in dead end screens used to go to previous screen")] public GameObject backArrow;
        [Tooltip("Evol logo")] public GameObject logo;
        
        [Header("Main mode layout")]
        [Tooltip("Layout that contain everything to start finding a game")] public GameObject gameConfig;
        [Tooltip("Connection state to master server")] public TextMeshProUGUI gameConfigConnectionState;
        [Tooltip("Button to start finding a game")] public Button gameConfigFindGameButton;
        [Tooltip("Layout that contain everything to select a char after joining a game")] public GameObject characterSelection;
        [Tooltip("Button to say ready")] public Button characterSelectionReadyButton;
        //[Tooltip("Client stats with this char")] public GameObject stats;
        //[Tooltip("Characters list")] public GameObject charactersList;
        //[Tooltip("The layout that contains the character to be selected to play")] public GameObject characterLayout;
        [Tooltip("Difficulty dropdown")] public TMP_Dropdown difficultyDropdown;


        private float timeToWaitPlayers = 3; // Should be proportional to the total number of players currently playing
        private bool ready;

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
            
            // Set the difficulty settings as room property
            difficultyDropdown.onValueChanged.AddListener(value =>
            {
                if (PhotonNetwork.InRoom)
                {
                    if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("difficulty"))
                        PhotonNetwork.CurrentRoom.CustomProperties.Add("difficulty", difficultyDropdown.value);
                    else
                        PhotonNetwork.CurrentRoom.CustomProperties["difficulty"] = difficultyDropdown.value;
                }
            });
            
            // We reach this condition if we leave a game a go back to main menu
            if(PlayFabClientAPI.IsClientLoggedIn())
                OnLoginSuccess();
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
                
                chat.GetComponent<Chat>().PlayFabAuthenticationContext = result.AuthenticationContext;
                chat.SetActive(true);
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
            chat.GetComponent<Chat>().enabled = false;
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
            // bottomBar.SetActive(!bottomBar.activeInHierarchy);
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
            if (PhotonNetwork.IsMasterClient)
                ready = true;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRandomFailed { returnCode } - { message }");
            if (returnCode == 32760) // No match found
            {
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
            if(!lockRoomOnStart)
                yield break;
                        
            // Wait until all players are ready
            yield return new WaitUntil(() => ready);

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
            // Only the master client can start the game (or if it can if its not locked)
            characterSelectionReadyButton.interactable = PhotonNetwork.IsMasterClient || !lockRoomOnStart;
            if (!characterSelectionReadyButton.interactable)
            {
                difficultyDropdown.interactable = false;
                characterSelectionReadyButton.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Waiting for host to start ...";
            }

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
            Debug.Log($"Disconnected from photon cloud { cause }");	
        }
    }
}