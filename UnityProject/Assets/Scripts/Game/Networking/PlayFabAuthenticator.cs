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
        public InputField Username;
        public InputField Password;
        public InputField Email;
        public TextMeshProUGUI Result;

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
                Result.text = $"You're now logged in !";
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


    }
}