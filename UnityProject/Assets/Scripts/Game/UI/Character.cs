using System.Collections;
using System.Collections.Generic;
using Evol.Game.Misc;
using Evol.Game.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Evol.Game.UI
{
    public class Character : MonoBehaviour
    {
        public GameObject[] CharacterList;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Name;
    
        public void OnClick(CharacterData characterData)
        {
            Debug.Log(characterData.Id);
            Debug.Log(characterData.CharacterName);
            Debug.Log(characterData.Description);
            Debug.Log(characterData.Icon);
            Debug.Log(characterData.Prefab);

            foreach (var c in CharacterList)
            {
                c.SetActive(false);
            }
            
            CharacterList[characterData.Id].SetActive(true);
            Description.text = characterData.Description;
            Name.text = characterData.CharacterName;
            // Set the character chosen in the properties of the player
            PhotonNetwork.LocalPlayer.CustomProperties.Add("character", characterData.Id.ToString());
        }
    }
}