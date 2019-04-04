using System.Collections;
using System.Collections.Generic;
using Evol.Game.Item;
using UnityEngine;

namespace Evol.Game.Item
{
    public abstract class Item : MonoBehaviour
    {
        public ItemData itemData;

        /// <summary>
        /// What happen when we pickup the item
        /// </summary>
        public abstract void OnPickup();

        /// <summary>
        /// What happen when we drop the item
        /// </summary>
        public abstract void OnDrop();
    }
}