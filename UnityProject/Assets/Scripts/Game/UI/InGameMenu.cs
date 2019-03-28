using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Evol.Game.Misc;
using Evol.Game.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
	public class InGameMenu : MonoBehaviour
	{
		
		public GameObject PlayUI;
		public GameObject PauseUI;

		[Header("Paused")] 
		public GameObject MainMenuPauseUI;
		public GameObject SettingsPauseUI;

		[Header("Settings")] 
		public GameObject SettingsControlsUI;
		public GameObject SettingsGraphicsUI;
		public GameObject SettingsAudioUI;
		
		[Header("Overlay")] 
		// Top bar
		public TextMeshProUGUI PlayersAlive;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI Kills;

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Cursor.visible = !Cursor.visible;

				// In case we exited the menu from settings, reset stuff
				if (SettingsPauseUI.activeInHierarchy)
				{
					OnSettings();
					OnControls();
				}
				
				PauseUI.SetActive(!PauseUI.activeInHierarchy);
				PlayUI.SetActive(!PlayUI.activeInHierarchy);

				// Disable / enable the character movement
				if (GetComponent<PlayerController>() != null)
					GetComponent<PlayerController>().Lock = !GetComponent<PlayerController>().Lock;
			}
			
			UpdateTimeUI();
		}

		/// <summary>
		/// Disable Pause UI
		/// Enable Play UI
		/// </summary>
		public void OnResume()
		{
			PauseUI.SetActive(false);
			PlayUI.SetActive(true);
			
			if (GetComponent<PlayerController>() != null)
				GetComponent<PlayerController>().Lock = !GetComponent<PlayerController>().Lock;
		}

		/// <summary>
		/// Switch from settings to main menu & vice-versa
		/// </summary>
		public void OnSettings()
		{
			SettingsPauseUI.SetActive(!SettingsPauseUI.activeInHierarchy);
			MainMenuPauseUI.SetActive(!MainMenuPauseUI.activeInHierarchy);
		}
		
		/// <summary>
		/// Exit game
		/// </summary>
		public void OnExitGame()
		{
			// Leave current room and go back to main menu
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel("login");
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
			SettingsControlsUI.SetActive(true);
			SettingsGraphicsUI.SetActive(false);
			SettingsAudioUI.SetActive(false);
		}
		
		/// <summary>
		/// Enable graphics panel only
		/// </summary>
		public void OnGraphics()
		{
			SettingsControlsUI.SetActive(false);
			SettingsGraphicsUI.SetActive(true);
			SettingsAudioUI.SetActive(false);
		}
		
		/// <summary>
		/// Enable audio panel only
		/// </summary>
		public void OnAudio()
		{
			SettingsControlsUI.SetActive(false);
			SettingsGraphicsUI.SetActive(false);
			SettingsAudioUI.SetActive(true);
		}

		public void UpdatePlayersAliveUI()
		{
			// PlayersAlive
		}

		public void UpdateTimeUI()
		{
			Time.text = ((int)UnityEngine.Time.time).ToString();
		}

		public void UpdateKillUI()
		{
			//
		}
	}
}
