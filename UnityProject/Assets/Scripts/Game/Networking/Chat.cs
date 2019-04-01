using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

namespace Evol.Game.Networking
{
    public class Chat : MonoBehaviour, IChatClientListener
    {

        public PlayFabAuthenticationContext PlayFabAuthenticationContext;
        public int messageLimit = 100;
        [Header("Chat layout")]
        public TMP_InputField ChatInput;
        public GameObject ChatContent;
        public TextMeshProUGUI ChatText;

        private ScrollRect chatScroll;
        private ChatClient chatClient;

        private Queue<string> chatMessages = new Queue<string>();
        
        
        // Start is called before the first frame update
        private void Start()
        {
            chatScroll = ChatContent.GetComponent<ScrollRect>();
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
            {
                PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
                AuthenticationContext = PlayFabAuthenticationContext
            }, OnPhotonChatSuccess, error => {
                Debug.Log($"Failed to connect to chat { error }");
            });
        }

        // Update is called once per frame
        private void Update()
        {
            if (chatClient == null)
                return;
            chatClient.Service();
            
            // If i press enter
            if (Input.GetKeyDown(KeyCode.Return)) 
            {
                // If the chat input is focused
                if (ChatContent.activeInHierarchy)
                {
                    // Send message
                    chatClient.PublishMessage("channelA", $"{ChatInput.text}");

                    // Empty the input
                    ChatInput.text = "";
                    Debug.Log($"Published {ChatInput.text}");
                }
                else // If the chat input isn't focused, focus it !
                {
                    OnChat();
                    ChatInput.ActivateInputField();
                }
            }
        }
        
        private void OnPhotonChatSuccess(GetPhotonAuthenticationTokenResult result)
        {
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
            chatClient.SetOnlineStatus(ChatUserStatus.Offline, $"disconnected");
            chatClient.Disconnect();
        }

        public void OnChat()
        {
            ChatContent.SetActive(!ChatContent.activeInHierarchy);
            Debug.Log($"Chat { ChatContent.activeInHierarchy }");
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
                chatMessages.Enqueue($"Channel {channelName} - { s } said: { messages.Aggregate("", (current, m) => current + m) }");
                if (messages.Length > messageLimit)
                    chatMessages.Dequeue();
            } 
            
            ChatText.text = "";
            foreach(string m in messages)
                ChatText.text += m + "\n";
            // Scroll to bottom
            StartCoroutine(ScrollToBottom());
        }
        
        private IEnumerator ScrollToBottom() {
            yield return new WaitForSeconds(.1f);
            chatScroll.normalizedPosition = new Vector2(0, 0);
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
            chatClient.SetOnlineStatus( ChatUserStatus.Online, "Mostly Harmless" );
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

        public void OnUserSubscribed(string channel, string user)
        {
            //
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            //
        }
    }
}