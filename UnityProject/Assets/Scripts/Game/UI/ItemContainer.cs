using Evol.Game.Misc;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public enum Location
    {
        Ground,
        Inventory,
        Rune
    }
    public class ItemContainer: MonoBehaviour
    {
        public Location location;
        private void Start()
        {
            EventManager.StartListening("OnItemEndDrag", item =>
            {
                // print(Vector3.Distance(transform.position, (item[0] as Transform).position));
                if (Vector3.Distance(transform.position, (item[0] as Transform).position) < 30)
                {
                    // Precision 30
                    (item[0] as Transform).SetParent(transform);
                    
                    if (location == Location.Ground)
                    {
                        // Spawn the object at the player position
                        var parentForward = transform.root.transform.position;
                        var positionToSpawn =
                            new Vector3(parentForward.x, parentForward.y + 1f,
                                parentForward.z + 2f); // TODO: tweak the + on position to make it realistic
                        var go = PhotonNetwork.InstantiateSceneObject((item[0] as Transform).GetComponent<ItemUi>().itemData.prefab.name,
                            positionToSpawn, 
                            Quaternion.identity);
                        Destroy((item[0] as Transform).gameObject);
                        // Throw it !
                        go.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10 * Time.deltaTime);
                    }
                }
            });
        }
    }
}