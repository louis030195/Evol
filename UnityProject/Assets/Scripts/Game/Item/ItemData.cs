using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Item
{   
    /// <summary>
    /// Item data is used to store fixed data about item
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        public int id;
        public string itemName = "New Item"; // The field isn't named "name" because it's used by unity
        public string description = "New Description";
        public Sprite icon;
        public int price = 0;
        // public GameObject prefab;
    }
}