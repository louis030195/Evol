using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Evol.Game.UI
{
    public class OverheadInformations : MonoBehaviour
    {
        public bool hideBar = true;
        public GameObject overheadInformations;
        [Tooltip("Reference to the text above the head")] public TextMeshProUGUI name;
        [Tooltip("Default name to give to non players objects")] public string defaultName = "Creep";
        
        // Start is called before the first frame update
        private void Start()
        {
            // We don't want to see our own overhead bar, just others
            if (gameObject.GetPhotonView().IsMine)
            {
                hideBar = true;
            }
            
            overheadInformations.SetActive(!hideBar);
            
            // If our parent has photonview (kinda hardcoded) and is not scene view (scene view = not a player)
            if(gameObject.GetPhotonView() != null && !gameObject.GetPhotonView().IsSceneView)
                name.text = $"{PhotonNetwork.LocalPlayer.NickName}";
            else
                name.text = $"Creep";
        }
    }
}