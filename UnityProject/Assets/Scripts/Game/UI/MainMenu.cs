using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Evol.Game.UI
{
	public class MainMenu : MonoBehaviour
	{
		public Text Nickname;

		public Button Multiplayer;
		public Button Singleplayer;

		public Text ConnectionState;
		
		private void Start()
		{
			
		}

		public void OnMultiPlayer()
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.LocalPlayer.NickName = Nickname.text;
			if (PhotonNetwork.JoinRoom("Yolo"))
			{
				PhotonNetwork.LoadLevel("Game");
				PlayerPrefs.SetInt("mode", 1); // Multiplayer or singleplayer ?
			}
			else
				ConnectionState.text = "Unable to find the server";
		}
		
		public void OnSinglePlayer()
		{
			SceneManager.LoadScene("Game");
			PlayerPrefs.SetInt("mode", 0); // Multiplayer or singleplayer ?
		}

		private void Update()
		{
			if (Nickname.text.Length > 0)
			{
				Singleplayer.interactable = true;
				Multiplayer.interactable = true;
			}
			
			if(PhotonNetwork.IsConnected)
				ConnectionState.text = "Connected to the cloud ! ";
		}
	}
}
