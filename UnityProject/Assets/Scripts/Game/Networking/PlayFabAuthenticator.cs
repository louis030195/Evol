using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using AuthenticationValues = Photon.Realtime.AuthenticationValues;

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
        [Tooltip("I don't know ?")] public GameObject Other;
        [Tooltip("Contains description / lore of the character")] public GameObject CharacterInformations;
        [Tooltip("Looking for a game status")] public TextMeshProUGUI QueueStatus;
        [Tooltip("Characters displayed in UI for selection")] public List<GameObject> Characters;
        private GameObject selectedCharacter;
        



        public void Login()
        {
            loginRequest = new LoginWithPlayFabRequest();
            loginRequest.Username = Username.text;
            loginRequest.Password = Password.text;
            PlayFabClientAPI.LoginWithPlayFab(loginRequest, result =>
            {
                // If the account is found
                PhotonNetwork.LocalPlayer.NickName = loginRequest.Username;
                Result.text = $"You're now logged in !";
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
        /// MainNav -> Play -> MainMode(1st box)
        /// </summary>
        public void OnMainMode()
        {
            // TODO: think about the character selection, best thing would be to store the last char played on the account
            // Then put the last char played
            OnCharacterSelection(Characters[0]);
            BackArrow.GetComponent<Button>().onClick.AddListener(() => // Like a Once event
            {
                var a = new UnityAction(() => Destroy(selectedCharacter));
                a.Invoke();
                BackArrow.GetComponent<Button>().onClick.RemoveListener(a);
            });}

        /// <summary>
        /// Click on my char in play layout -> characters list
        /// </summary>
        public void OnCharacterList()
        {
            Stats.SetActive(!Stats.activeInHierarchy);
            CharactersList.SetActive(!CharactersList.activeInHierarchy);
            Other.SetActive(!Other.activeInHierarchy);
            CharacterInformations.SetActive(!CharacterInformations.activeInHierarchy);
            BackArrow.GetComponent<Button>().onClick.AddListener(() => // Like a Once event
            {
                var a = new UnityAction(OnCharacterList);
                a.Invoke();
                BackArrow.GetComponent<Button>().onClick.RemoveListener(a);
            });
        }

        /// <summary>
        /// Called when clicking on character icon in characters list
        /// </summary>
        /// <param name="character"></param>
        public void OnCharacterSelection(GameObject character)
        {
            if(selectedCharacter != null)
                Destroy(selectedCharacter);
            var go = Instantiate(character, CharacterLayout.transform);
            go.transform.localScale *= 100;
            go.transform.rotation = new Quaternion(0, 90, 0, 90);
            selectedCharacter = go;
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
			// TODO: google photon lobby etc, next step join game whatever
			
        }
    }
}