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

		public Button Play;

		public Text ConnectionState;
		
		private void Start()
		{
			PhotonNetwork.ConnectUsingSettings();
		}

		public void OnMultiPlayer()
		{
			PhotonNetwork.LocalPlayer.NickName = Nickname.text;
			if(PhotonNetwork.JoinRoom("Yolo"))
				PhotonNetwork.LoadLevel("Game");
			else
				ConnectionState.text = "Unable to find the server";
		}
		
		public void OnSinglePlayer()
		{

		}

		private void Update()
		{
			if (PhotonNetwork.IsConnected && !Nickname.text.Equals(""))
			{
				Play.interactable = true;
			}
			
			if(PhotonNetwork.IsConnected)
				ConnectionState.text = "Connected to the cloud ! ";
		}
	}
}
