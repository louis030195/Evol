using System.Collections;
using System.Collections.Generic;
using Evol.Game.Item;
using Evol.Game.Misc;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Evol.Game.Item
{
    public abstract class Item : MonoBehaviour
    {
        // Instance in the scene
        [HideInInspector] public GameObject instance;
        public ItemData itemData;

        private PhotonView photonView;

        private void Awake()
        {
            instance = gameObject;
            
            // My name is set to item name + instanceID for example Shako1463827, because Unity doing find with string GG
            name = $"{itemData.itemName}{instance.GetInstanceID()}";

            // Get my photon view
            photonView = gameObject.GetPhotonView();
        } 
        private void OnTriggerEnter(Collider other)
        {
            // If a player is close to me, tell him i'm here
            if (other.CompareTag("Player"))
                EventManager.TriggerEvent ("OnItemAroundAdd", new object[] { instance });
        }

        private void OnTriggerExit(Collider other)
        {
            // TODO: try if OnTriggerExit is called when item is disabled / destroyed (for example another player took the item)
            // In that case we could call OnTriggerExit in OnDisable / OnDestroy
            // If a player went away from me, tell him
            if (other.CompareTag("Player"))
                EventManager.TriggerEvent ("OnItemAroundRemove", new object[] { instance });
        }
    }
}