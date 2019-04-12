using System.Collections;
using System.Collections.Generic;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Item
{
    public class Loot : MonoBehaviour
    {
        // Currently it's random among all existing items, could think about monsters looting more of a specific item ...
        [HideInInspector] public List<GameObject> items;

        private bool quitting; // https://answers.unity.com/questions/775810/applicationquit-and-ondestroy.html
        private void OnApplicationQuit() {
            quitting = true;
        }
        private void OnDisable()
        {
            if (quitting) return; // Just to avoid calling this function when stopping play mode in editor, or it will cause issues
            
            // Spawn the object at the object position
            var position = transform.position;
            var positionToSpawn = new Vector3(position.x, position.y + 1, position.z);
            
            // For each items i have in my loot
            foreach (var item in items)
            {
                var go = PhotonNetwork.Instantiate(item.name, positionToSpawn,
                    Quaternion.identity);
                // Throw it !
                go.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10 * Time.deltaTime);
            }
        }
    }
}