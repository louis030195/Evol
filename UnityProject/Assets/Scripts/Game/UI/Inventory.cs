using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Misc;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayerManager = Evol.Game.Player.PlayerManager;

namespace Evol.Game.UI
{
	public class Inventory : MonoBehaviour
	{
		[Header("Parents layout")]
		[Tooltip("Layout containing the play")] public GameObject playUi;
		[Tooltip("Item/Ground Layout containing the ground items")] public GameObject itemsGround;
		
		[Header("Grid layouts (usually final child)")]
		[Tooltip("Item/Ground/Content Grid containing the items icons")] public GameObject itemsGroundContent;
		[Tooltip("Item/Equipped/Content Grid containing the items icons")] public GameObject itemsInventoryEquippedContent;
		[Tooltip("Item/NonEquipped/Content Grid containing the items icons")] public GameObject itemsInventoryNonEquippedContent;
		
		[Tooltip("Item/InformationPanel Panel of information to show when the cursor is above and item")] public GameObject itemInformationPanel;
		[Tooltip("Item/InformationPanel/Text Text in the information panel")] public TextMeshProUGUI itemInformationText;
		
		[Header("Placeholders")]
		[Tooltip("Prefab for the item icon")] public GameObject itemsEquippedSlotPlaceholder;
		[Tooltip("Prefab for the item icon")] public GameObject itemsIconPlaceholder;

		private PlayerManager playerManager;
		private bool lastPointerEnterIsGround; // Do we drop it on the ground ?
		private bool draggedFromGround; // Do we drag it from the ground ?
		private int draggedFromAbilityNumber; // Last ability GameObject hovered by mouse
		private int draggedToAbilityNumber; // Last ability GameObject hovered by mouse
		// Abilities GameObject instances in the equipped inventory
		private List<GameObject> abilities = new List<GameObject>(); 
		private int dragCount; // To check if we hit the first call of dragging function

		private void Awake()
		{
			playerManager = GetComponentInParent<PlayerManager>();
			
			// Fill the equipped UI with our abilities
			foreach (var i in Enumerable.Range(0, playerManager.characterData.abilities.Length))
			{
				var go = Instantiate(itemsEquippedSlotPlaceholder, itemsInventoryEquippedContent.transform);
				var trigger = go.AddComponent<EventTrigger>();

				var entryPointerEnter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
				entryPointerEnter.callback.AddListener((data) =>
				{
					draggedToAbilityNumber = i;
					lastPointerEnterIsGround = false;
				});
				trigger.triggers.Add(entryPointerEnter);

				abilities.Add(go);
			}

			// We actually add the event trigger to his parent, because the grid is probably empty, 
			var triggerNonEquipped =
				itemsInventoryNonEquippedContent.transform.parent.gameObject.AddComponent<EventTrigger>();

			var entryPointerEnterNonEquipped = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entryPointerEnterNonEquipped.callback.AddListener((data) =>
			{
				draggedToAbilityNumber = -1;
				lastPointerEnterIsGround = false;
			});
			triggerNonEquipped.triggers.Add(entryPointerEnterNonEquipped);

			// Add trigger pointer enter for ground
			var triggerGround = itemsGround.AddComponent<EventTrigger>();

			var entryPointerEnterGround = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entryPointerEnterGround.callback.AddListener((data) => { lastPointerEnterIsGround = true; });
			triggerGround.triggers.Add(entryPointerEnterGround);

			// Listen to the event "An item is now close to me"
			EventManager.StartListening("OnItemAroundAdd", ItemAroundAdd);

			// Listen to the event "An item went away from me"
			EventManager.StartListening("OnItemAroundRemove", ItemAroundRemove);
		}

		private void ItemAroundAdd(object[] item)
		{
			// Icon isn't networked so no PhotonNetwork.Instanciate()
			var go = Instantiate(itemsIconPlaceholder, itemsGroundContent.transform);
			go.name = (item[0] as GameObject).name; // Set the same name than the GameObject instance
			var itemComponent = (item[0] as GameObject).GetComponent<Item.Item>();
			go.GetComponent<Image>().sprite = itemComponent.itemData.icon;

			// Already have these event (when taking an item dropping it ...)
			if (go.GetComponent<EventTrigger>() != null) return;

			var trigger = go.AddComponent<EventTrigger>();

			var entryPointerEnter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entryPointerEnter.callback.AddListener((data) =>
			{
				InformationPointerEnter(data as PointerEventData, itemComponent.itemData);
			});
			trigger.triggers.Add(entryPointerEnter);

			var entryPointerExit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
			entryPointerExit.callback.AddListener((data) => { InformationPointerExit(data as PointerEventData); });
			trigger.triggers.Add(entryPointerExit);

			var entryDrag = new EventTrigger.Entry {eventID = EventTriggerType.Drag};
			entryDrag.callback.AddListener((data) => { Drag(data as PointerEventData, go); });
			trigger.triggers.Add(entryDrag);

			var entryEndDrag = new EventTrigger.Entry {eventID = EventTriggerType.EndDrag};
			entryEndDrag.callback.AddListener((data) =>
			{
				EndDrag(data as PointerEventData, go, itemComponent);
			});
			trigger.triggers.Add(entryEndDrag);
		}

		private void ItemAroundRemove(object[] data)
		{
			var found = itemsGroundContent.transform.Find((data[0] as GameObject).name);

			// Destroy the item icon from the UI grid when it went too far away
			if (found) // Happen that we can't find the icon because it has been dragged out (if moving while dragging)
				Destroy(found.gameObject);
		}

		private void EndDrag(PointerEventData data, GameObject itemIcon, Item.Item itemComponent)
		{
			dragCount = 0;

			// TODO: will probably (not sure) break with stackable object // duplicates ... (due to the remove maybe check ID or idk)

			// If it was not from the ground, we should remove from the appropriate list
			if (!draggedFromGround)
			{
				// Try to remove from the non equipped list
				if (playerManager.inventoryNonEquipped.Remove(itemComponent))
				{
					// Try to remove from the equipped list
					playerManager.abilitiesRunes[draggedFromAbilityNumber].Remove((itemComponent as Rune).itemData as RuneData);
				}
			}

			// Dropped in inventory
			if (!lastPointerEnterIsGround)
			{
				// If it was dragged to the non equipped inventory
				if (draggedToAbilityNumber == -1)
				{
					// Put it in the non equipped inventory grid
					itemIcon.transform.SetParent(itemsInventoryNonEquippedContent.transform, false);

					// Add to the list
					playerManager.inventoryNonEquipped.Add(itemComponent);
				}
				else // Else it was dragged to the equipped inventory
				{
					// Put it in the right ability runes grid
					itemIcon.transform.SetParent(abilities[draggedToAbilityNumber].transform, false);

					// Add to the list
					playerManager.abilitiesRunes[draggedToAbilityNumber].Add((itemComponent as Rune).itemData as RuneData);
				}

				// Remove from the ground
				if (itemComponent.instance)
				{
					PhotonNetwork.Destroy(itemComponent.instance);
				}
			}
			else // Dropped on the ground
			{



				// We don't want to make another instance if it was dragged from ground to ground
				if (draggedFromGround)
				{
					// Put it in ground grid
					itemIcon.transform.SetParent(itemsGroundContent.transform, false);
					return;
				}


				Destroy(itemIcon);

				// Spawn the object at the player position
				var parentForward = transform.parent.transform.position;
				var positionToSpawn =
					new Vector3(parentForward.x, parentForward.y + 1f,
						parentForward.z + 2f); // TODO: tweak the + on position to make it realistic
				var go = PhotonNetwork.InstantiateSceneObject(itemComponent.itemData.prefab.name, positionToSpawn,
					Quaternion.identity);
				// go.GetComponent<Item.Item>().instance = go; // Shouldn't need this since its in the awake()
				// Throw it !
				go.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10 * Time.deltaTime);
			}
		}

		private void Drag(PointerEventData data, GameObject item)
		{
			// If we started to drag while the pointer hovering ground, it means that it come from the ground
			if (dragCount == 0)
			{
				// Obviously the drag event is called multiple times, we just want to call this at the first time
				draggedFromGround = lastPointerEnterIsGround;
				draggedFromAbilityNumber = item.transform.parent.GetSiblingIndex();
			}

			// Release from the layout
			item.transform.SetParent(playUi.transform, false);

			// The item follow the mouse obviously
			item.transform.position = Input.mousePosition;

			dragCount++;
		}

		private void InformationPointerEnter(PointerEventData data, ItemData item)
		{
			itemInformationPanel.SetActive(true);

			// Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position);
			// TODO: make the windows always visible to screen using camera and stuff
			// 

			// Show the panel a little above the mouse
			itemInformationPanel.transform.position = new Vector3(Input.mousePosition.x * 0.8f,
				Input.mousePosition.y * 1.2f, Input.mousePosition.z);
			itemInformationText.text = item.ToString();
		}

		private void InformationPointerExit(PointerEventData data)
		{
			itemInformationPanel.SetActive(false);
		}
	}
}