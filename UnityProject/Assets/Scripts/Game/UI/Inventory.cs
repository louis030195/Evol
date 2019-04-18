using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Misc;
using Evol.Utils;
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
		[Tooltip("Size of the non equipped inventory")] public int inventoryNonEquippedSize = 5;
		
		[Tooltip("Item/InformationPanel Panel of information to show when the cursor is above and item")] public GameObject itemInformationPanel;
		[Tooltip("Item/InformationPanel/Text Text in the information panel")] public TextMeshProUGUI itemInformationText;
		
		[Header("Placeholders")]
		[Tooltip("Prefab for the ability runes container (ability image + 5x item container)")] public GameObject abilityRunesContainerPlaceholder;
		[Tooltip("Prefab for any item container")] public GameObject itemContainerPlaceholder;
		[Tooltip("Prefab for the item icon")] public GameObject itemsPlaceholder;

		private PlayerManager playerManager;
		private bool lastPointerEnterIsGround; // Do we drop it on the ground ?
		private bool draggedFromGround; // Do we drag it from the ground ?
		private int draggedFromAbilityNumber; // Last ability GameObject hovered by mouse
		private int draggedToAbilityNumber; // Last ability GameObject hovered by mouse
		// Abilities GameObject instances in the equipped inventory
		private List<GameObject> abilities = new List<GameObject>();
		private List<GameObject> groundItems = new List<GameObject>();
		private int dragCount; // To check if we hit the first call of dragging function

		private void Awake()
		{
			playerManager = GetComponentInParent<PlayerManager>();
			
			// Fill the equipped UI with our abilities
			foreach (var i in Enumerable.Range(0, playerManager.characterData.abilities.Length))
			{
				var go = Instantiate(abilityRunesContainerPlaceholder, itemsInventoryEquippedContent.transform);
				// Set the appropriate ability icon
				go.transform.GetChild(0).GetComponent<Image>().sprite = playerManager.characterData.abilities[i].icon; 
				abilities.Add(go);
			}
			
			// Fill the non equipped UI with containers
			foreach (var i in Enumerable.Range(0, inventoryNonEquippedSize))
			{
				var go = Instantiate(itemContainerPlaceholder, itemsInventoryNonEquippedContent.transform);
				go.GetComponent<ItemContainer>().location = Location.Inventory;
			}
			

			// Listen to the event "An item is now close to me"
			if (EventManager.Instance)
			{
				EventManager.StartListening("OnItemAroundAdd", ItemAroundAdd);

				// Listen to the event "An item went away from me"
				EventManager.StartListening("OnItemAroundRemove", ItemAroundRemove);
				
				EventManager.StartListening("OnItemEndDrag", OnItemEndDrag);
			}
			else
			{
				Debug.LogWarning("No EventManager here !!");
			}
		}
		
		private void OnItemEndDrag(object[] item)
		{
			StartCoroutine(WaitEvent((Transform) item[0]));
		}

		private IEnumerator WaitEvent(Transform t)
		{
			// We have to wait a litte delay after event
			// Because there is multiple listeners to this event, we can afford to be the last in the stack
			/*while(t.parent.parent == null)
			{
				yield return null;
			}*/
			
			yield return new WaitForSeconds(0.1f);
			if (t == null) yield break; // Case when it's been dropped to ground, no need to have 2 icon for 1 item
			var previousContainer = t.GetComponent<ItemUi>().previousContainer.GetComponent<ItemContainer>();

			if (previousContainer.location == Location.Rune) // Picked from abilities runes
			{
				// Remove from runes
				playerManager.abilitiesRunes[previousContainer.transform.parent.GetSiblingIndex()]
					.Remove(t.GetComponent<ItemUi>().itemData as RuneData);
			} 
			else if (previousContainer.location == Location.Inventory) // Picked from inventory
			{
				// Remove from runes
				playerManager.inventoryNonEquipped.Remove(t.GetComponent<ItemUi>().itemData);
			}
			else if(previousContainer.location == Location.Ground) // Picked from ground
			{
				var physicalItem = groundItems.Find(i => i == t.GetComponent<ItemUi>().physicalInstance);
				groundItems.Remove(physicalItem);
				PhotonNetwork.Destroy(physicalItem); // Hide physical item
			}
			
			var container = t.parent.GetComponent<ItemContainer>();
			// print($"{container.transform.parent.GetSiblingIndex()}");

			if (container.location == Location.Rune) // ability rune container
			{
				// Add to the list
				playerManager.abilitiesRunes[t.parent.parent.GetSiblingIndex()]
					.Add(t.GetComponent<ItemUi>().itemData as RuneData);
				
				// Using getsiblingindex
				// means that we have to have the rune as grand child of the ability layout so we know to which ability its added
				// Eg ability1/container1/item
				//    ability1/container.../item
				//    ability2/container/item
			} 
			else if (container.location == Location.Inventory)
			{
				// Add to the list
				playerManager.inventoryNonEquipped.Add(t.GetComponent<ItemUi>().itemData);
			}
		}

		private void ItemAroundAdd(object[] item)
		{
			var physicalItem = item[0] as GameObject;
			groundItems.Add(physicalItem);
			/*
			foreach (Transform child in itemsGroundContent.transform)
			{
				if (child.name.Equals(physicalItem.name)) return;
				// child.GetComponent<ItemUi>().physicalItem.GetComponent<Item.
			}*/
			
			// Icon isn't networked so no PhotonNetwork.Instanciate()
			var go = Instantiate(itemsPlaceholder, itemsGroundContent.transform);
			go.GetComponent<ItemUi>().playUi = playUi.transform;
			go.GetComponent<ItemUi>().itemData = physicalItem.GetComponent<Item.Item>().itemData;
			go.GetComponent<ItemUi>().physicalInstance = physicalItem;
			go.name = physicalItem.gameObject.name; // Set the same name than the GameObject instance
			go.GetComponent<Image>().sprite = physicalItem.GetComponent<Item.Item>().itemData.icon;
			
			// go.AddComponent<Item.Item>(itemComponent);

			// Already have these event (when taking an item dropping it ...)
			if (go.GetComponent<EventTrigger>() != null) return;

			var trigger = go.AddComponent<EventTrigger>();

			var entryPointerEnter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entryPointerEnter.callback.AddListener(data =>
			{
				InformationPointerEnter(data as PointerEventData, go.GetComponent<ItemUi>().itemData);
			});
			trigger.triggers.Add(entryPointerEnter);

			
			var entryPointerExit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
			entryPointerExit.callback.AddListener((data) => { InformationPointerExit(data as PointerEventData); });
			trigger.triggers.Add(entryPointerExit);
		}

		private void ItemAroundRemove(object[] data)
		{
			groundItems.Add(data[0] as GameObject);
			
			var found = itemsGroundContent.transform.Find((data[0] as GameObject).name);

			// Destroy the item icon from the UI grid when it went too far away
			if (found) // Happen that we can't find the icon because it has been dragged out (if moving while dragging)
				Destroy(found.gameObject);
		}

		/*
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
					playerManager.abilitiesRunes[draggedFromAbilityNumber].Remove((itemComponent as Rune).itemData as RuneData); // TODO: doesnt remove ? doesnt work ?
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
		}*/

		private void InformationPointerEnter(PointerEventData data, ItemData item)
		{
			if (item == null) return;
			
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