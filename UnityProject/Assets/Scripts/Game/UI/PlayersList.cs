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
                UpdateTextForNewPlayer(currentRoomPlayer.Value);
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

        private void UpdateTextForNewPlayer(Photon.Realtime.Player player, bool add = true)
        {
            var chosenChar = player.CustomProperties.ContainsKey("character")
                ? player.CustomProperties["character"]
                : 0;
            var lastDamageDealt = player.CustomProperties.ContainsKey("damageDealt")
                ? player.CustomProperties["damageDealt"]
                : 0;
            if (add)
            {
                playersList.text += $"\n{player.NickName}" +
                                    $"\nChosen character {chosenChar}" +
                                    $"\nDamage dealt last game {lastDamageDealt}";
            }
            else
            {
                playersList.text = playersList.text.Replace($"\n{player.NickName}" +
                                                                     $"\nChosen character {chosenChar}" +
                                                                     $"\nDamage dealt last game {lastDamageDealt}", "");
            }
        }
    }
}