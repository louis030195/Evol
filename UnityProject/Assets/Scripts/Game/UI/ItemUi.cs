using System.Collections;
using Evol.Game.Item;
using Evol.Game.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public class ItemUi : MonoBehaviour
    {
        [HideInInspector] public Transform playUi;
        [HideInInspector] public Transform previousContainer;
        [HideInInspector] public ItemData itemData;
        [HideInInspector] public GameObject physicalInstance;
        private int dragCount;
        private void Start()
        {         
            var trigger = gameObject.AddComponent<EventTrigger>();
            
            var entryDrag = new EventTrigger.Entry {eventID = EventTriggerType.Drag};
            entryDrag.callback.AddListener(data =>
            {
                if (dragCount == 0)
                    previousContainer = transform.parent; // Only save at the first drag
                dragCount++;
                
                // Release from the layout
                transform.SetParent(playUi, false); // Required also to be above all other images

                // The item follow the mouse obviously
                transform.position = Input.mousePosition;
                
                EventManager.TriggerEvent("OnItemDrag", new object[]{ transform });
            });
            trigger.triggers.Add(entryDrag);
            
            var entryEndDrag = new EventTrigger.Entry {eventID = EventTriggerType.EndDrag};
            entryEndDrag.callback.AddListener(data =>
            {
                dragCount = 0;
                
                EventManager.TriggerEvent("OnItemEndDrag", new object[]{ transform });
                
                // Didn't get dropped on a proper container
                if(transform.parent == playUi)
                    transform.SetParent(previousContainer);
            });
            trigger.triggers.Add(entryEndDrag);
        }
    }
}