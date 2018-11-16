﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Evol.Game.UI
{
	public class MainMenu : MonoBehaviourPunCallbacks
	{
		public Text Nickname;
		public Button Multiplayer;
		public Button Singleplayer;
		public Text ConnectionState;
		

		public void OnMultiPlayer()
		{
			PhotonNetwork.OfflineMode = false;
			PhotonNetwork.ConnectUsingSettings();
			ConnectionState.text = "Connecting ...";
		}
		
		public void OnSinglePlayer()
		{
			PhotonNetwork.OfflineMode = true;
			SceneManager.LoadScene("Game");
		}

		private void Update()
		{
			if (Nickname.text.Length > 0)
			{
				Singleplayer.interactable = true;
				Multiplayer.interactable = true;
			}
				
		}

		private void OnFailedToConnectToMasterServer( )
		{
			ConnectionState.text = "Failed to connect";
		}

		public override void OnConnectedToMaster()
		{
			ConnectionState.text = "Connected ! Loading game ... ";
			
			if (PhotonNetwork.JoinRoom("Yolo"))
			{
				PhotonNetwork.LocalPlayer.NickName = Nickname.text;
				PhotonNetwork.LoadLevel("Game");
			}
			else
				ConnectionState.text = "Can't join room";

		}
	}
}
