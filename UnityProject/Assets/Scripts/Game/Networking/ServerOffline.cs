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
        // TODO: make it work for 0.6


        if (player.GetComponent<Agent>())
        {
            
            //player.GetComponent<Agent>().GiveBrain(Brains.Find(brain => brain.name.Contains("Player")));
            /*
            player.AddComponent<DemonstrationRecorder>();
            player.GetComponent<DemonstrationRecorder>().demonstrationName = "AgentDemo";
            player.GetComponent<DemonstrationRecorder>().record = true;*/
            //player.GetComponent<DemonstrationRecorder>().
            
            /*
            for (var i = 0; i < 6; i++)
            {
                var carnivorous = SpawnCarnivorous();
                carnivorous.GetComponent<Agent>().GiveBrain(Brains.Find(brain => brain.name.Contains("Student")));
            }*/
        }


        //var camera = Instantiate(CameraPrefab);
        //camera.GetComponent<SmoothFollow>().target = player.transform;

        //SpawnHerbivorous();
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
