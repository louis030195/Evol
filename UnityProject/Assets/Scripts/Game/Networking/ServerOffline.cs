using System.Collections;
using System.Collections.Generic;
using Evol.Game.Networking;
using UnityEngine;

public class ServerOffline : Server 
{
    protected override void Start()
    {
        base.Start();
        Initialize();
        var player = Instantiate(PlayerPrefab, Vector3.up, Quaternion.identity);
        Mode(player);
    }
}
