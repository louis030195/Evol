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
        public ItemData itemData;
        public GameObject AnimationOnSpawn;

        private PhotonView photonView;

        private void OnEnable()
        {
            // Spawn animation
            PhotonNetwork.Instantiate(AnimationOnSpawn.name, transform.position, new Quaternion(-90,0,0, 90));
            
            // My name is set to item name + instanceID for example Shako1463827, because Unity doing find with string GG
            name = $"{itemData.itemName}{gameObject.GetInstanceID()}";

            // Get my photon view
            photonView = gameObject.GetPhotonView();
        } 
        private void OnTriggerEnter(Collider other)
        {
            // If a player is close to me, tell him i'm here
            if (other.CompareTag("Player"))
                EventManager.TriggerEvent ("OnItemAroundAdd", new object[] { gameObject });
        }

        private void OnTriggerExit(Collider other)
        {
            // TODO: try if OnTriggerExit is called when item is disabled / destroyed (for example another player took the item)
            // In that case we could call OnTriggerExit in OnDisable / OnDestroy
            // If a player went away from me, tell him
            if (other.CompareTag("Player"))
                EventManager.TriggerEvent ("OnItemAroundRemove", new object[] { gameObject });
        }

        public override string ToString()
        {
            return itemData.ToString();
        }
    }
}