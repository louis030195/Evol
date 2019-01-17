using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using RPGCharacterAnims;
using UnityEngine;

namespace Evol.Game.UI
{
	public class InGameMenu : MonoBehaviour
	{
		
		public GameObject PlayUI;
		public GameObject PauseUI;

		// While in pause ...
		public GameObject MainMenuPauseUI;
		public GameObject SettingsPauseUI;

		// While in settings ...
		public GameObject SettingsControlsUI;
		public GameObject SettingsGraphicsUI;
		public GameObject SettingsAudioUI;
		
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Cursor.visible = !Cursor.visible;

				// In case we exited the menu from settings, reset stuff
				if (SettingsPauseUI.active)
				{
					OnSettings();
					OnControls();
				}
				
				PauseUI.SetActive(!PauseUI.active);
				PlayUI.SetActive(!PlayUI.active);

				// Disable / enable the character movement
				if (GetComponent<PlayerController>() != null)
					GetComponent<PlayerController>().Lock = !GetComponent<PlayerController>().Lock;
			}
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
			SettingsPauseUI.SetActive(!SettingsPauseUI.active);
			MainMenuPauseUI.SetActive(!MainMenuPauseUI.active);
		}
		
		/// <summary>
		/// Exit game
		/// </summary>
		public void OnExitGame()
		{
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
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
	}
}
