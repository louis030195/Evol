using System;
using System.Collections;
using System.Collections.Generic;
using Evol.Game.Misc;
using Evol.Game.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public class Character : MonoBehaviour
    {
        [Tooltip("Prefab array of the characters")] public CharacterData[] characterList;
        [Tooltip("Description text of the character")] public TextMeshProUGUI description;
        [Tooltip("Name text of the character")] public TextMeshProUGUI name;
        [Tooltip("Grid of icons of melee character")] public GameObject meleeGrid;
        [Tooltip("Grid of icons of ranged character")] public GameObject rangedGrid;
        [Tooltip("Prefab for the button with character icon")] public GameObject characterIconTemplate;
        [Tooltip("Where the character will stand")] public GameObject characterPlaceholder;

        private GameObject[] characterListObjects;
        
        private void Start()
        {
            characterListObjects = new GameObject[characterList.Length];
            
            foreach (var character in characterList)
            {
                
                // Set the characters in place
                var charGo = Instantiate(character.placeholder, characterPlaceholder.transform);
                charGo.transform.localScale *= 100;
                //charGo.transform.Rotate(new Vector3(0, 180, 0));
                var position = charGo.transform.position;
                position = new Vector3(position.x, position.y, position.z + 100);
                charGo.transform.position = position;
                characterListObjects[character.id] = charGo; // We use ID for ordering in the list

                // We instanciate all the character buttons with in the right ranged / melee grid,
                // with proper icon and add the listener for this character
                var buttonGo = Instantiate(characterIconTemplate, 
                    character.ranged ? rangedGrid.transform : meleeGrid.transform);
                buttonGo.GetComponent<Image>().sprite = character.icon;
                buttonGo.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnClick(character);
                    });     
            }
        }

        public void OnClick(CharacterData characterData)
        {
            foreach (var c in characterListObjects)
            {
                c.SetActive(false);
            }
            
            characterListObjects[characterData.id].SetActive(true);
            description.text = characterData.description;
            name.text = characterData.characterName;
            // Add the character chosen in the properties of the player or set if we already had a character key
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("character"))
                PhotonNetwork.LocalPlayer.CustomProperties["character"] = characterData.id.ToString();
            else
                PhotonNetwork.LocalPlayer.CustomProperties.Add("character", characterData.id.ToString());
        }
    }
}