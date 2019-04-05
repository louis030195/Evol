using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public enum ItemLocation{ Equipped, NonEquipped }
    public class Inventory : MonoBehaviour
    {

        
        private List<Item.Item> items = new List<Item.Item>();

        private void Start()
        {
            // playerObject.GetComponent<PlayerManager>().inventory
        }

        public void RemoveItem(Item.Item item)
        {
            items.Remove(item);
            item.OnDrop();
            var go = new GameObject();
            go.AddComponent<Image>().sprite = item.itemData.icon;
            // Instantiate(go, itemsGroundScrollContent.transform);
        }

        public void AddItem(Item.Item item, ItemLocation location)
        {
            items.Add(item);
            item.OnPickup();
            var go = new GameObject();
            go.AddComponent<Image>().sprite = item.itemData.icon;
            // Instantiate(go, location == ItemLocation.NonEquipped ? inventoryNonEquipped.transform : inventoryEquipped.transform);
        }
    }
}