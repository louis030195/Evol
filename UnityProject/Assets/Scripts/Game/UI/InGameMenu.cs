﻿using System.Collections;
using System.Collections.Generic;
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
				// In case we exited the menu from settings, reset stuff
				if (SettingsPauseUI.activeInHierarchy)
				{
					OnSettings();
					OnControls();
				}
				
				PauseUI.SetActive(!PauseUI.activeInHierarchy);
				PlayUI.SetActive(!PlayUI.activeInHierarchy);

				// Disable / enable the character movement
				GetComponent<RPGCharacterControllerFREE>().Lock(true, true, !PauseUI.activeInHierarchy, 0, 0);
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
