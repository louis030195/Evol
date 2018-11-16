using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

public class PlayerManagerOnline : PlayerManager 
{
	
	private PhotonView photonView;
	public GameObject playerCamera;
	public MonoBehaviour[] playerControlScripts;
	
	protected override void Start()
	{
		DeactivateNonLocal();
		base.Start();
	}

	protected override void Awake()
	{
		photonView = GetComponent<PhotonView>();
		base.Awake();
	}

	// Update is called once per frame
	protected override void Update ()
	{
		if (photonView.IsMine)
			base.Update();
	}

	protected override void FixedUpdate()
	{
		if(photonView.IsMine)
			base.FixedUpdate();
	}
	
	/// <summary>
	/// Only the local player should access his own camera and control scripts
	/// </summary>
	private void DeactivateNonLocal()
	{
		playerCamera.SetActive(photonView.IsMine);

            
		foreach (var playerControlScript in playerControlScripts)
		{
			playerControlScript.enabled = photonView.IsMine;
		}
	}
}
