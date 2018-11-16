using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeHandler : MonoBehaviour
{
	
	// Use this for initialization
	private void Start () {

		if (PlayerPrefs.GetInt("mode") == 0)
		{
			GetComponent<ServerOffline>().enabled = true;
		}
		else if (PlayerPrefs.GetInt("mode") == 1)
		{
			GetComponent<ServerOnline>().enabled = true;
		}
			
	}
	

}
