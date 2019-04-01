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
        [Tooltip("Prefab array of the characters")] public GameObject[] characterList;
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
                var characterData = character.GetComponent<CastBehaviour>().characterData;
                
                // Set the characters in place
                var charGo = Instantiate(characterData.Placeholder, characterPlaceholder.transform);
                charGo.transform.localScale *= 100;
                charGo.transform.Rotate(new Vector3(0, 180, 0));
                charGo.transform.localPosition = new Vector3(charGo.transform.position.x, charGo.transform.position.y, charGo.transform.position.z - 100);
                characterListObjects[characterData.Id] = charGo; // We use ID for ordering in the list

                // We instanciate all the character buttons with in the right ranged / melee grid,
                // with proper icon and add the listener for this character
                var buttonGo = Instantiate(characterIconTemplate, 
                    characterData.Ranged ? rangedGrid.transform : meleeGrid.transform);
                buttonGo.GetComponent<Image>().sprite = characterData.Icon;
                buttonGo.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnClick(characterData);
                    });     
            }
        }

        public void OnClick(CharacterData characterData)
        {
            foreach (var c in characterListObjects)
            {
                c.SetActive(false);
            }
            
            characterListObjects[characterData.Id].SetActive(true);
            description.text = characterData.Description;
            name.text = characterData.CharacterName;
            // Add the character chosen in the properties of the player or set if we already had a character key
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("character"))
                PhotonNetwork.LocalPlayer.CustomProperties["character"] = characterData.Id.ToString();
            else
                PhotonNetwork.LocalPlayer.CustomProperties.Add("character", characterData.Id.ToString());
        }
    }
}