using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
	public class MainMenu : MonoBehaviour
	{
		public Text Nickname;
		
		private void Start()
		{
			PhotonNetwork.ConnectUsingSettings();
		}

		public void OnPlay()
		{
			PhotonNetwork.LocalPlayer.NickName = Nickname.text.Equals("") ? "Retard°" + PhotonNetwork.PlayerList.Length + 1
				: Nickname.text;
			if(PhotonNetwork.JoinRoom("Yolo"))
				PhotonNetwork.LoadLevel("Game");
		}
		
		
	}
}
