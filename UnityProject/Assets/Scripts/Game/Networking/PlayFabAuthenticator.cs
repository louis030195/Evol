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
        [Header("Login / Register fields")] 
        public GameObject LoginRegisterCanvas;
        public InputField Username;
        public InputField Password;
        public InputField Email;
        public TextMeshProUGUI Result;
        private LoginWithPlayFabRequest loginRequest;

        [Header("Main menu fields")] 
        public GameObject MainMenuCanvas;
        [Tooltip("Main content at the centre of the screen")] public GameObject MainContent;
        [Tooltip("Nav at the top used to navigate in the main content")] public GameObject MainNav;
        [Tooltip("Bar at the bottom used for specific less used stuff")] public GameObject BottomBar;
        [Tooltip("Back arrow displayed in dead end screens used to go to previous screen")] public GameObject BackArrow;
        [Tooltip("Evol logo")] public GameObject Logo;
        
        [Header("Main mode layout")]
        [Tooltip("Client stats with this char")] public GameObject Stats;
        [Tooltip("Characters list")] public GameObject CharactersList;
        [Tooltip("The layout that contains the character to be selected to play")] public GameObject CharacterLayout;
        [Tooltip("Looking for a game status")] public TextMeshProUGUI QueueStatus;


        private float timeToWaitPlayers = 30; // Should be proportional to the total number of players currently playing


        public void Login()
        {
            loginRequest = new LoginWithPlayFabRequest {Username = Username.text, Password = Password.text};
            PlayFabClientAPI.LoginWithPlayFab(loginRequest, result =>
            {
                // If the account is found
                PhotonNetwork.LocalPlayer.NickName = loginRequest.Username;
                Result.text = $"You're now logged in !";
                PhotonNetwork.OfflineMode = false;
                PhotonNetwork.ConnectUsingSettings();
                gameObject.GetComponent<Chat>().PlayFabAuthenticationContext = result.AuthenticationContext;
                gameObject.GetComponent<Chat>().enabled = true;
                LoginRegisterCanvas.SetActive(false);
                MainMenuCanvas.SetActive(true);
            }, error =>
            {
                // If the account is not found
                Result.text = $"Incorrect username or password";
            }, null);
        }

        public void Register()
        {
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
            request.Email = Email.text;
            request.Username = Username.text;
            request.Password = Password.text;
            
            PlayFabClientAPI.RegisterPlayFabUser(request, result =>
                {
                    Result.text = $"Your account has been created !";
                }, error => { Result.text = $"Fill all field !"; });
        }

        /// <summary>
        /// Should be called when disconnected from account and going back to login / register screen
        /// BottomBar -> Last button
        /// </summary>
        public void OnDisconnect()
        {
            LoginRegisterCanvas.SetActive(true);
            MainMenuCanvas.SetActive(false);
            gameObject.GetComponent<Chat>().enabled = false;
            Result.text = $"Successfully disconnected";
        }

        /// <summary>
        /// Deactive all MainContent layouts and active the selected layout
        /// </summary>
        /// <param name="layout"></param>
        public void OnMainNav(GameObject layout)
        {
            foreach (Transform lay in MainContent.transform)
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
            MainNav.SetActive(!MainNav.activeInHierarchy);
            MainContent.SetActive(!MainContent.activeInHierarchy);
            BottomBar.SetActive(!BottomBar.activeInHierarchy);
            BackArrow.SetActive(!BackArrow.activeInHierarchy);
            Logo.SetActive(!Logo.activeInHierarchy);
            if (layout != null)
            {
                layout.SetActive(!layout.activeInHierarchy);
                BackArrow.GetComponent<Button>().onClick.AddListener(() => // Like a Once event
                {
                    var a = new UnityAction(() => OnDeadEnd(layout));
                    a.Invoke();
                    BackArrow.GetComponent<Button>().onClick.RemoveListener(a);
                });
            }
            else
                Debug.Log($"You forgot to add a layout to this dead end");
        }

        /// <summary>
        /// PlayLayout -> Ready
        /// Try to find a game
        /// </summary>
        public void OnReady()
        {
            // to join / create game (no friend)
            if(PhotonNetwork.JoinRandomRoom()) {
                // Wait a bit other people (proportional to the total number of player in the game)
                // Start game

            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRandomFailed { returnCode } - { message }");
            if (returnCode == 32760) // No match found
            {
                if (PhotonNetwork.JoinOrCreateRoom(new Random().Next(0, 100).ToString(), new RoomOptions(), TypedLobby.Default))
                {
                    // Wait a bit other players then start
                    StartCoroutine(WaitPlayersAndStart());
                    
                }
            }
        }

        IEnumerator WaitPlayersAndStart()
        {
            float timeToWait = timeToWaitPlayers;
            while (timeToWait > 0)
            {
                yield return new WaitForSeconds(1);
                
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    yield break;
                }

                QueueStatus.text = $"Estimated time to wait {timeToWait}";
                timeToWait--;
            }
            PhotonNetwork.LoadLevel("Game");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"OnJoinedRoom");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRoomFailed { returnCode } - { message }");
        }

        public override void OnConnectedToMaster()
        {
		    Debug.Log($"Connected to master cloud");	
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // Persist player data
            var playerData = new Dictionary<string, string>();
            foreach (var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
            {
                playerData.Add((string)key, (string)PhotonNetwork.LocalPlayer.CustomProperties[key]);
            }
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                AuthenticationContext = loginRequest.AuthenticationContext,
                Data = playerData
            }, result =>
            {
                Debug.Log($"UpdateUserData succeed - { result }");	
            }, error =>
            {
                Debug.Log($"UpdateUserData failed - { error }");	
            });
            Debug.Log($"Disconnected to master cloud { cause }");	
        }
    }
}