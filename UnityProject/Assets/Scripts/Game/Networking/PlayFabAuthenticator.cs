using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.Networking
{
    public class PlayFabAuthenticator : MonoBehaviourPunCallbacks
    {
        [Header("Login / Register fields")] 
        public GameObject LoginRegisterCanvas;
        public InputField Username;
        public InputField Password;
        public InputField Email;
        public TextMeshProUGUI Result;

        [Header("Main menu fields")] 
        public GameObject MainMenuCanvas;
        [Tooltip("Main content at the centre of the screen")] public GameObject MainContent;
        [Tooltip("Nav at the top used to navigate in the main content")] public GameObject MainNav;
        [Tooltip("Bar at the bottom used for specific less used stuff")] public GameObject BottomBar;
        [Tooltip("Back arrow displayed in dead end screens used to go to previous screen")] public GameObject BackArrow;
        [Tooltip("Evol logo")] public GameObject Logo;
        
        [Header("Play layout")]
        public GameObject PlayLayout;
        [Tooltip("The layout that contains the character to be selected to play")] public GameObject CharacterLayout;
        [Tooltip("Characters displayed in UI for selection")] public GameObject[] Characters;
        [Tooltip("Looking for a game status")] public TextMeshProUGUI QueueStatus;

        private string playFabPlayerIdCache;

        public bool IsAuthenticated = false;
        
        private LoginWithPlayFabRequest loginRequest;

        public void Login()
        {
            loginRequest = new LoginWithPlayFabRequest();
            loginRequest.Username = Username.text;
            loginRequest.Password = Password.text;
            PlayFabClientAPI.LoginWithPlayFab(loginRequest, result =>
            {
                // If the account is found
                IsAuthenticated = true;
                PhotonNetwork.LocalPlayer.NickName = loginRequest.Username;
                Result.text = $"You're now logged in !";
                LoginRegisterCanvas.gameObject.SetActive(false);
                MainMenuCanvas.gameObject.SetActive(true);
            }, error =>
            {
                // If the account is not found
                IsAuthenticated = false;
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
            LoginRegisterCanvas.gameObject.SetActive(true);
            MainMenuCanvas.gameObject.SetActive(false);
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
        /// </summary>
        public void OnDeadEnd()
        {
            MainNav.SetActive(!MainNav.activeInHierarchy);
            MainContent.SetActive(!MainContent.activeInHierarchy);
            BottomBar.SetActive(!BottomBar.activeInHierarchy);
            BackArrow.SetActive(!BackArrow.activeInHierarchy);
            Logo.SetActive(!Logo.activeInHierarchy);
        }

        /// <summary>
        /// MainNav -> Play -> Play(1st box)
        /// </summary>
        public void OnPlayLayout()
        {
            // TODO: think about the character selection, best thing would be to store the last char played on the account
            // Then put the last char played
            PlayLayout.SetActive(true);
            var go = Instantiate(Characters[0], CharacterLayout.transform);
            go.transform.localScale *= 100;
            go.transform.rotation = new Quaternion(0, 90, 0, 90);
        }

        /// <summary>
        /// PlayLayout -> Ready
        /// Try to find a game
        /// </summary>
        public void OnReady()
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.ConnectUsingSettings();
        }
        
        public override void OnConnectedToMaster()
        {
            QueueStatus.text = "Looking for a game ...";
			// TODO: google photon lobbys etc, next step join game whatever
        }
    }
}