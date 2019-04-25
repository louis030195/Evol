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

        private Camera cam;
        private PhotonView photonView;
        
        // Start is called before the first frame update
        private void Start()
        {
            photonView = GetComponentInParent<PhotonView>();
            if (photonView == null)
            {
                Debug.Fail($"OverheadInformations no photonview in parent");
            }
            
            // We don't want to see our own overhead bar, just others
            hideBar = photonView.IsMine;
            
            overheadInformations.SetActive(!hideBar);
            
            // If our parent is not scene view (scene view = not a player)
            name.text = !photonView.IsSceneView ? $"{PhotonNetwork.LocalPlayer.NickName}" : $"Creep";
            
            cam = Camera.main; // Get camera
        }

        private void Update()
        {
            /*
             // TODO: fix this (someone create game, other join ok it works, but the guy joining get this =>
             // TODO: NullReferenceException: Object reference not set to an instance of an object
             // TODO: Evol.Game.UI.OverheadInformations.Update () (at Assets/Scripts/Game/UI/OverheadInformations.cs:45)
             // TODO: (cant find the main camera ? (our own cam)   
                
            if (!photonView.IsMine) // Only needed to rotate on others since we don't see our own bar
            {
                // Make the canvas always look to the camera
                var rotation = cam.transform.rotation;
                overheadInformations.transform.LookAt(transform.position + rotation * Vector3.back,
                    rotation * Vector3.up);
            }*/
        }
    }
}