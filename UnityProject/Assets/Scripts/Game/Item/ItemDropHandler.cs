using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Evol.Game.Item
{
    public class ItemDropHandler : MonoBehaviour, IDropHandler
    {
        public Inventory inventory;

        public void OnDrop(PointerEventData eventData)
        {
            var invPanel = transform as RectTransform;

            if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel,
                Input.mousePosition))
            {

                var item = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>().Item;
                if (item != null)
                {
                    inventory.RemoveItem(item);
                    item.OnDrop();
                }

            }
        }
    }
}