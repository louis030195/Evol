using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Item
{
    public class Inventory : MonoBehaviour
    {      
        private List<Item> items = new List<Item>();
        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }
    }
}