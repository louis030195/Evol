using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
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
		public Button Play;
		public Text ConnectionState;

		public GameObject MainMenuu;
		public GameObject CharacterMenu;
		public GameObject SelectionEffect;

		private bool multiplayerGame;
		private GameObject selectionEffectObject;

		private void SwitchMenu()
		{
			MainMenuu.SetActive(false);
			CharacterMenu.SetActive(true);
		}
		
		public void OnMultiPlayer()
		{
			multiplayerGame = true;
			SwitchMenu();
		}
		
		public void OnSinglePlayer()
		{
			multiplayerGame = false;
			SwitchMenu();
		}

		public void OnPlay()
		{
			if (multiplayerGame)
			{
				PhotonNetwork.OfflineMode = false;
				PhotonNetwork.ConnectUsingSettings();
				ConnectionState.text = "Connecting ...";
			}
			else
			{
				//SceneManager.LoadScene("Game");
				PhotonNetwork.OfflineMode = true;
				//PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.LeaveRoom();
				OnConnectedToMaster();
			}
				
		}

		public void OnCharacterSelection(GameObject character)
		{
			Destroy(selectionEffectObject);
			
			PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){{"character", character.name}});
			selectionEffectObject = Instantiate(SelectionEffect, new Vector3(character.transform.position.x, -30, 90), new Quaternion(90, 0, 0, 90), transform.parent);
			Play.interactable = true;
		}

		private void Update()
		{
			if (Nickname.text.Length > 0)
			{
				Singleplayer.interactable = true;
				Multiplayer.interactable = true;
			}
				
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
