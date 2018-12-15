using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Evol.Game.Networking;
using MLAgents;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ServerOffline : Server 
{
    protected override void Start()
    {
        base.Start();
        Initialize();
        var player = Instantiate(PlayerPrefabs.ToList().Find(p => p.name.Contains(PhotonNetwork.LocalPlayer.CustomProperties.Values.First() as string)), 
            Vector3.up,
            Quaternion.identity);
        if (player.GetComponent<Agent>())
        {
            player.GetComponent<Agent>().GiveBrain(Brains.Find(brain
                => brain.name.Contains("Teacher")).GetComponent<Brain>());
            Brains.Find(brain
                => brain.name.Contains("Teacher")).GetComponent<Brain>().brainType = BrainType.External;
        }

        //var camera = Instantiate(CameraPrefab);
        //camera.GetComponent<SmoothFollow>().target = player.transform;
        Mode(player);
        /*
        for (var i = 0; i < 6; i++)
        {
            var carnivorous = SpawnCarnivorous();
            carnivorous.GetComponent<Agent>().GiveBrain(Brains.Find(brain 
                => brain.name.Contains("Student")).GetComponent<Brain>());
        }*/
    }


    protected override IEnumerator SpawnAgents()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 10));
            SpawnCarnivorous();
            SpawnHerbivorous();
            SpawnHerb();

            
        }
    }
}
