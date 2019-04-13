using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

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
            var photonView = transform.parent.gameObject.GetPhotonView();
            if (photonView == null)
            {
                Debug.Fail($"OverheadInformations no photonview in parent");
            }
            
            // We don't want to see our own overhead bar, just others
            hideBar = photonView.IsMine;
            
            overheadInformations.SetActive(!hideBar);
            
            // If our parent is not scene view (scene view = not a player)
            if(!photonView.IsSceneView)
                name.text = $"{PhotonNetwork.LocalPlayer.NickName}";
            else
                name.text = $"Creep";
        }
    }
}