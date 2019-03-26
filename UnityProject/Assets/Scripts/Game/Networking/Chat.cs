using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.Networking
{
    public class Chat : MonoBehaviour, IChatClientListener
    {

        public PlayFabAuthenticationContext PlayFabAuthenticationContext;
        
        [Header("Chat layout")]
        public TMP_InputField ChatInput;
        public ScrollRect ChatContent;
        
        private ChatClient chatClient;
        
        // Start is called before the first frame update
        private void Start()
        {
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
            {
                PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
                AuthenticationContext = PlayFabAuthenticationContext
            }, OnPhotonChatSuccess, error => {
                Debug.Log($"Failed to connect to chat { error }");
            });
            // InitializeChat();
        }

        // Update is called once per frame
        private void Update()
        {
            if (chatClient != null)
                chatClient.Service();
            // If we have the chat focused and press enter, send the text in our input chat
            if (ChatInput.isFocused && Input.GetKeyDown(KeyCode.Return)) 
            {
                chatClient.PublishMessage( "channelA", $"{ ChatInput.text }" );
            }
        }
        
        private void OnPhotonChatSuccess(GetPhotonAuthenticationTokenResult result)
        {
            chatClient.SetOnlineStatus( ChatUserStatus.Online, "Mostly Harmless" );
            // In the C# SDKs, the callbacks are defined in the `IChatClientListener` interface.
            // In the demos, we instantiate and use the ChatClient class to implement the IChatClientListener interface.
            chatClient = new ChatClient(this);
            // Set your favourite region. "EU", "US", and "ASIA" are currently supported.
            var success = chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                PhotonNetwork.AppVersion,
                new AuthenticationValues(PlayFabAuthenticationContext.PlayFabId));
            Debug.Log($"Connected to chat { success }");
        }

        private void OnDisable()
        {
            chatClient.Unsubscribe(new[] {"channelA"});
            chatClient.Disconnect();
        }

        public void OnChat()
        {
            ChatContent.gameObject.SetActive(!ChatContent.gameObject.activeInHierarchy);
            Debug.Log($"OnChat");
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log($"DebugReturn - { PhotonNetwork.NickName } { level } { message }");
        }

        public void OnDisconnected()
        {
            Debug.Log($"OnDisconnected - { PhotonNetwork.NickName } disconnected from chat");
        }

        public void OnConnected()
        {
            chatClient.Subscribe( new[] { "channelA" } );
            Debug.Log($"OnConnected - { PhotonNetwork.NickName } connected to chat");
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log($"OnChatStateChange - { PhotonNetwork.NickName } new state { state }");
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            foreach (var s in senders)
            {
                // Empty the chat after a number of messages ? (should just remove the oldest messages)
                if (ChatContent.content.transform.childCount > 30)
                {
                    foreach (Transform child in ChatContent.content.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
                ChatContent.content.gameObject.AddComponent<TextMeshPro>().text = 
                    $"$Channel {channelName} - { s } said: { messages.Aggregate("", (current, m) => current + m) }";
            }
            // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`.
            // So you don't have to keep track of them.
            // The channel name is the key for `PublicChannels`.
            // In very long or active conversations, you might want to trim each channels history.
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            ChatInput.text = $"$Channel {channelName} - {sender} whispers: {message}";
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            foreach (var c in channels)
            {
                chatClient.PublishMessage( c, $"{ PhotonNetwork.LocalPlayer.NickName } joined the channel" );
            }
            Debug.Log($"OnSubscribed - { PhotonNetwork.NickName } - { channels.Aggregate("", (current, m) => current + m) }" +
                      $" - { results.Aggregate("", (current, m) => current + m) }");
        }

        public void OnUnsubscribed(string[] channels)
        {
            foreach (var c in channels)
            {
                chatClient.PublishMessage( c, $"{ PhotonNetwork.LocalPlayer.NickName } left the channel" );
            }
            Debug.Log($"OnUnsubscribed - { PhotonNetwork.NickName } - { channels.Aggregate("", (current, m) => current + m) }");
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log($"OnStatusUpdate - { user } - { status } - { gotMessage } - { message }");
        }
    }
}