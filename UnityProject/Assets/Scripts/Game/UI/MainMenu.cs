using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.UI
{
	public class MainMenu : MonoBehaviour 
	{
		private void Start()
		{
			PhotonNetwork.ConnectUsingSettings();
		}

		public void OnPlay()
		{
			if(PhotonNetwork.JoinRoom("Yolo"))
				PhotonNetwork.LoadLevel("Game");
		}
		
		
	}
}
