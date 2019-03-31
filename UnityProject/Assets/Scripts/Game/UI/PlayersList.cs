using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Evol.Game.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayersList : MonoBehaviourPunCallbacks
    {
        private TextMeshProUGUI playersList;
        private void Start()
        {
            playersList = GetComponent<TextMeshProUGUI>();
            foreach (var currentRoomPlayer in PhotonNetwork.CurrentRoom.Players)
            {
                playersList.text += $"\n{currentRoomPlayer.Value.NickName}";
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            playersList.text += $"\n{newPlayer.NickName}";
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            playersList.text = playersList.text.Replace($"\n{otherPlayer.NickName}", "");
        }
    }
}