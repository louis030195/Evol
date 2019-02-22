using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		/// <summary>
		/// List of tuples character spot ; is free ?
		/// </summary>
		private List<Tuple<Vector3, bool>> charactersOffsets;

		private void SwitchMenu()
		{
			MainMenuu.SetActive(false);
			CharacterMenu.SetActive(true);
			charactersOffsets = new List<Tuple<Vector3, bool>>();
			charactersOffsets.Add(Tuple.Create(Vector3.zero, true));
			foreach (var character in CharacterMenu.transform.Find("Characters"))
			{
				charactersOffsets.Add(Tuple.Create((character as Transform).localPosition, false));
			}
			
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
			// selectionEffectObject = Instantiate(SelectionEffect, new Vector3(character.transform.position.x, -20, 70), new Quaternion(90, 0, 0, 90), transform.parent);
			//character.transform.position = Vector3.Lerp(character.transform.position, Vector3.right, Time.deltaTime);
			// Character in a free spot
			var destination = Vector3.zero;
			var positionIndex = charactersOffsets.FindIndex(
				offset => Vector3.Distance(character.transform.localPosition, offset.Item1) < 20); // Maybe find a better solution
			if (Vector3.Distance(character.transform.localPosition, destination) > 10)
			{
				// Empty the spot
				charactersOffsets[positionIndex] = Tuple.Create(charactersOffsets[positionIndex].Item1, true);
				charactersOffsets[0] = Tuple.Create(charactersOffsets[0].Item1, false);
			}
			else // Character in the middle
			{
				// Free the spot
				charactersOffsets[positionIndex] = Tuple.Create(charactersOffsets[positionIndex].Item1, false);
				charactersOffsets[0] = Tuple.Create(charactersOffsets[0].Item1, true);
				// Find a free spot
				destination = charactersOffsets.Find(offset => offset.Item2 == false).Item1;
			}
			StartCoroutine(SlowlyMoveTo(character, character.transform.localPosition, destination));

			character.GetComponent<Animator>().SetTrigger("Attack1Trigger");
			// character.transform.Translate(Vector3.right * 10);
			// character.transform.position = Vector3.MoveTowards(character.transform.position, Vector3.zero, Time.deltaTime * 100);
			Play.interactable = true;
		}

		IEnumerator SlowlyMoveTo(GameObject go, Vector3 origin, Vector3 destination)
		{
			float timeSinceStarted = 0f;
			while (true)
			{
				timeSinceStarted += Time.deltaTime;
				go.transform.localPosition = Vector3.Lerp(origin, destination, timeSinceStarted);

				// If the object has arrived, stop the coroutine
				if (go.transform.position == Vector3.zero)
				{
					yield break;
				}

				// Otherwise, continue next frame
				yield return null;
			}
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
			
			// If offline mode joinroom return true even if we're not on master
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
