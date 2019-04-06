using System;
using System.Collections;
using System.Collections.Generic;
using Evol.Game.Item;
using Evol.Game.Misc;
using Evol.Game.UI;
using Evol.Utils;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

namespace Evol.Game.Player
{

    /// <summary>
    /// Player manager.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Tooltip("Contains the specific data about the character chosen")] public CharacterData characterData;
        
        [HideInInspector] public EventListenedList<Item.Item> inventoryEquipped = new EventListenedList<Item.Item>();
        [HideInInspector] public EventListenedList<Item.Item> inventoryNonEquipped = new EventListenedList<Item.Item>();
        
        // The local player instance. Use this to know if the local player is represented in the Scene
        public static GameObject LocalPlayerInstance;

        private Animator animator; // TODO: should we use the behaviour stuff for this also ?
        private static readonly int Pickup = Animator.StringToHash("Pickup");

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
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
            
            inventoryEquipped.OnAdd += OnAdd;
            inventoryEquipped.OnRemove += OnRemove;
            
            inventoryNonEquipped.OnAdd += OnAdd;
            inventoryNonEquipped.OnRemove += OnRemove;

            animator = GetComponent<Animator>();
        }

        private void OnAdd(object sender, EventArgs e) {
            animator.SetTrigger(Pickup);
            // print("Element added...");
        }
        
        private void OnRemove(object sender, EventArgs e) {
            // print("Element removed...");
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