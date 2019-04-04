using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Evol.Game.Item
{
    public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
    {

        public Item Item { get; set; }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = Vector3.zero;
        }
    }
}