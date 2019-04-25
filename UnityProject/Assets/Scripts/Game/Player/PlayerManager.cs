using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Ability;
using Evol.Game.Item;
using Evol.Game.Misc;
using Evol.Game.Networking;
using Evol.Game.UI;
using Evol.Utils;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Evol.Game.Player
{

    /// <summary>
    /// Player manager.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Tooltip("Frequency of sending player data to server in seconds")] public int playerDataUpdateFrequency = 60;
        [Tooltip("Contains the specific data about the character chosen")] public CharacterData characterData;
        public List<string> alliesTag = new List<string>();
        public List<string> enemiesTag = new List<string>();
        
        [HideInInspector] public EventListenedList<RuneData>[] abilitiesRunes;
        [HideInInspector] public EventListenedList<ItemData> inventoryNonEquipped = new EventListenedList<ItemData>();
        
        // The local player instance. Use this to know if the local player is represented in the Scene
        public static GameObject LocalPlayerInstance;

        private Animator animator; // TODO: should we use the behaviour stuff for this also ?
        private static readonly int Pickup = Animator.StringToHash("pickup");
        private Chat chat;

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                
                // #Critical
                // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
                DontDestroyOnLoad(gameObject);

                if(Chat.LocalChatInstance != null) // If it's null it means we are in debug mode (game scene directly)
                    chat = Chat.LocalChatInstance.GetComponent<Chat>();

                animator = GetComponent<Animator>();
                abilitiesRunes = new EventListenedList<RuneData>[characterData.abilities.Length];
                for (var i = 0; i < abilitiesRunes.Length; i++)
                {
                    abilitiesRunes[i] = new EventListenedList<RuneData>();
                    abilitiesRunes[i].OnAdd += Add;
                    abilitiesRunes[i].OnRemove += Remove;
                }


                foreach (var t in characterData.abilities)
                {
                    if (!t.prefab) continue; // In case our ability has no prefab (melee)
                    var ability = t.prefab.GetComponent<Ability.Ability>();
                    ability.alliesTag = alliesTag;
                    ability.enemiesTag = enemiesTag;
                    // Set the caster
                    ability.caster = gameObject;
                }
            }
        }

        private void Update()
        {
            // check if chat is set is for debug mode (not logged to playfab probably)
            if (chat && Time.time % playerDataUpdateFrequency < 1)
            {
                // I guess it could happen to be null if we are debugging and didn't pass by login scene ?
                if (PlayFabClientAPI.IsClientLoggedIn())
                {
                    // Persist player data
                    var playerData = new Dictionary<string, string>();
                    foreach (var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
                    {
                        if(!key.ToString().Equals("context")) // Here put keys to exclude from persisted on server data
                            playerData.Add(key.ToString(), PhotonNetwork.LocalPlayer.CustomProperties[key].ToString());
                    }

                    if (playerData.Count > 0) // We don't always add player data (just logging, disconecting ....)
                    {
                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                            {
                                AuthenticationContext = chat.PlayFabAuthenticationContext,
                                Data = playerData
                            }, result => { /*Debug.Log($"UpdateUserData succeed - {result}");*/ },
                            error => { /*Debug.Log($"UpdateUserData failed - {error}");*/ });
                    }
                }
            }
        }

        private void Add(object sender, EventArgs e)
        {
            print("rune added");
        }
        
        private void Remove(object sender, EventArgs e)
        {
            print("rune removed");
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
            }
            else
            {
            }
        }
    }
}