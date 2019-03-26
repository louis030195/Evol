using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Misc
{
	public class ModeHandler : MonoBehaviour
	{
		
		// Use this for initialization
		private void Start () {
	
			if (PhotonNetwork.OfflineMode)
			{
				GetComponent<ServerOffline>().enabled = true;
			}
			else if (!PhotonNetwork.OfflineMode)
			{
				GetComponent<ServerOnline>().enabled = true;
			}
				
		}
		
	
	}
}
