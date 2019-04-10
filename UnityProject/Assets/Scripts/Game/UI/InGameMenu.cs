using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Misc;
using Evol.Game.Player;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evol.Game.UI
{
	public class InGameMenu : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		public GameObject playUi;
		public GameObject pauseUi;

		[Header("Paused")] 
		public GameObject mainMenuPauseUi;
		public GameObject settingsPauseUi;
		public Button resumePauseButton;
		public Button settingsPauseButton;
		public Button exitGamePauseButton;

		[Header("Settings")] 
		public GameObject settingsControlsUi;
		public Button settingsControlsButton;
		public GameObject settingsGraphicsUi;
		public Button settingsGraphicsButton;
		public GameObject settingsAudioUi;
		public Button settingsAudioButton;
		
		[Header("Overlay top bar")] 
		public TextMeshProUGUI playersAlive;
		public TextMeshProUGUI time;
		public TextMeshProUGUI kills;
		
		[Header("Overlay items (center of the screen)")] 
		[Tooltip("Layout containing the inventory & ground")] public GameObject itemsUi;
		[Tooltip("Layout containing the ground items")] public GameObject itemsGround;
		[Tooltip("Layout containing the inventory items")] public GameObject itemsInventory;
		
		private void Awake()
		{
			// We don't wanna see others players overlay (is there any cleaner solution ?)
			if(gameObject.transform.parent.gameObject.GetPhotonView() != null && !gameObject.transform.parent.gameObject.GetPhotonView().IsMine)
				playUi.transform.parent.gameObject.SetActive(false);
			
			// Set the onclick actions of buttons
			ResetListener(resumePauseButton, OnResume);
			ResetListener(settingsPauseButton, OnSettings);
			ResetListener(exitGamePauseButton, OnExitGame);
			
			ResetListener(settingsControlsButton, OnControls);
			ResetListener(settingsGraphicsButton, OnGraphics);
			ResetListener(settingsAudioButton, OnAudio);
			
			// Initialize the players alive counter
			UpdatePlayersAliveUI();
		}	

		// Update is called once per frame
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) // Display pause panel
			{
				Cursor.visible = !Cursor.visible;
				OnPause();
			}
			
			if (Input.GetKeyDown(KeyCode.LeftAlt)) // Display inventory panel + ground panel
			{
				Cursor.visible = !Cursor.visible;
				
				// We keep the play UI
				if(pauseUi.activeInHierarchy)
					pauseUi.SetActive(false);
				
				itemsUi.SetActive(!itemsUi.activeInHierarchy);
				itemsGround.SetActive(itemsUi.activeInHierarchy);
				itemsInventory.SetActive(itemsUi.activeInHierarchy);
			}

			if (Input.GetKeyDown(KeyCode.I)) // Display the inventory panel only
			{
				Cursor.visible = !Cursor.visible;
				
				// We keep the play UI
				if(pauseUi.activeInHierarchy)
					pauseUi.SetActive(false);
				
				itemsUi.SetActive(!itemsUi.activeInHierarchy);
				itemsGround.SetActive(itemsUi.activeInHierarchy);
				itemsInventory.SetActive(false);
			}
			
			UpdateTimeUI();
		}
		
		private void ResetListener(Button button, UnityAction unityAction)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(unityAction);
		}
		

		private void OnPause()
		{
			// In case we exited the menu from settings, reset stuff
			if (settingsPauseUi.activeInHierarchy)
			{
				OnSettings();
				OnControls();
			}
				
			pauseUi.SetActive(!pauseUi.activeInHierarchy);
			playUi.SetActive(!playUi.activeInHierarchy);
		}

		/// <summary>
		/// Disable Pause UI
		/// Enable Play UI
		/// </summary>
		public void OnResume()
		{
			pauseUi.SetActive(false);
			playUi.SetActive(true);
			
			// if (GetComponent<CastBehaviour>() != null)
			//	GetComponent<CastBehaviour>().Lock = !GetComponent<CastBehaviour>().Lock;
		}

		/// <summary>
		/// Switch from settings to main menu & vice-versa
		/// </summary>
		public void OnSettings()
		{
			settingsPauseUi.SetActive(!settingsPauseUi.activeInHierarchy);
			mainMenuPauseUi.SetActive(!mainMenuPauseUi.activeInHierarchy);
		}
		
		/// <summary>
		/// Exit game
		/// </summary>
		public void OnExitGame()
		{
			// Leave current room and go back to main menu
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel("Login");
			#if UNITY_EDITOR
				// UnityEditor.EditorApplication.isPlaying = false;
			#else
				// Application.Quit();
			#endif

		}

		/// <summary>
		/// Enable control panel only
		/// </summary>
		public void OnControls()
		{
			settingsControlsUi.SetActive(true);
			settingsGraphicsUi.SetActive(false);
			settingsAudioUi.SetActive(false);
		}
		
		/// <summary>
		/// Enable graphics panel only
		/// </summary>
		public void OnGraphics()
		{
			settingsControlsUi.SetActive(false);
			settingsGraphicsUi.SetActive(true);
			settingsAudioUi.SetActive(false);
		}
		
		/// <summary>
		/// Enable audio panel only
		/// </summary>
		public void OnAudio()
		{
			settingsControlsUi.SetActive(false);
			settingsGraphicsUi.SetActive(false);
			settingsAudioUi.SetActive(true);
		}

		public void UpdatePlayersAliveUI()
		{
			playersAlive.text = (PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount : 1).ToString();
		}

		public void UpdateTimeUI()
		{
			var minutes = Mathf.FloorToInt(Time.time / 60f);
			var seconds = Mathf.FloorToInt(Time.time - minutes * 60);
			time.text = $"{minutes:00}:{seconds:00}";
		}
		
		/// <inheritdoc />
		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == 0)
			{
				var data = (photonEvent.CustomData as object[])[0];
				if (data.Equals("Monster") || data.Equals("Tree"))
				{
					int.TryParse(kills.text, out var value);
					// Increment the kills counter
					kills.text = (value + 1).ToString();
				}
				else
				{
					int.TryParse(playersAlive.text, out var value);
					// Decrement the players alive counter
					playersAlive.text = (value > 0 ? value-- : 0).ToString();
				}
			}
		}

		public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
			int.TryParse(playersAlive.text, out var value);
			// Increment the players alive counter
			playersAlive.text = (value > 0 ? value++ : 0).ToString();
		}

		public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			int.TryParse(playersAlive.text, out var value); // TODO: player dead left ?
			// Increment the players alive counter
			playersAlive.text = (value > 0 ? value-- : 0).ToString();
		}
	}
}
