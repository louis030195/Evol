﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Networking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ServerOnline : Server
{
	protected override void Start()
	{
		base.Start();
		PhotonNetwork.ConnectUsingSettings();
	}



	
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{

		//var player = PlayerPool.GetObject();
		//player.SetActive(true);
		// PhotonNetwork.Instantiate("Actors/PlayerControlled/Player") 
		print(newPlayer.CustomProperties.Values.First());
		var player = PhotonNetwork.Instantiate(
			PlayerPrefabs.ToList().Find(p => p.name.Contains(newPlayer.CustomProperties.Values.First() as string)).name,
			Vector3.up, 
			Quaternion.identity);
		//var camera = Instantiate(CameraPrefab);
		//player.GetComponent<Agent>().brain.InitializeBrain(FindObjectOfType<Academy>(), null);
		//player.GetComponent<Agent>().GiveBrain(Brains[0]);
		//player.SetActive(true); // Required to give brain on disabled GO, then active it
		player.name = newPlayer.NickName;
            
		player.GetPhotonView().TransferOwnership(newPlayer);
            
            
		players.Add(player);

            
		Debug.Log($"OnPlayerEnteredRoom() { newPlayer.NickName }");
	}

	public override void OnCreatedRoom()
	{
		Debug.Log($"OnCreatedRoom()"); 
            
		Initialize();
	}
	
	
	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		Debug.Log($"OnPlayerLeftRoom() { otherPlayer.NickName }"); 
            
		// PlayerPool.ReleaseObject(players.Find(p => p.name.Equals(otherPlayer.NickName)));
		Destroy(players.Find(p => p.name.Equals(otherPlayer.NickName)), 1f);
		players.Remove(players.Find(p => p.name.Equals(otherPlayer.NickName)));
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log($"OnConnectedToMaster()");
            
		// The server create the room
		var newRoomOptions = new RoomOptions();
		newRoomOptions.IsOpen = true;
		newRoomOptions.IsVisible = true;
		newRoomOptions.MaxPlayers = MaxPlayersPerRoom;
            
		PhotonNetwork.CreateRoom("Yolo", newRoomOptions);
            
	}
	
	
	private void OnConnectedToServer()
	{
		Debug.Log($"OnConnectedToServer()");
	}


	public override void OnJoinedLobby()
	{
		Debug.Log($"OnJoinedLobby()"); 
	}
	
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log($"OnJoinRandomFailed() - {returnCode} - {message}"); 
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log($"OnJoinRoomFailed() - {returnCode} - {message}"); 
	}

	public override void OnJoinedRoom()
	{
		Debug.Log($"OnJoinedRoom()"); 
	}
}
