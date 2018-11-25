using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Evol.Game.Networking;
using MLAgents;
using UnityEngine;

public class ServerOffline : Server 
{
    protected override void Start()
    {
        base.Start();
        Initialize();
        var player = Instantiate(PlayerPrefab, Vector3.up, Quaternion.identity);
        var camera = Instantiate(CameraPrefab);
        Mode(player);
    }


    protected override IEnumerator SpawnAgents()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 10));

            Vector2 direction = Random.insideUnitCircle;
            Vector3 position = Vector3.zero;
            Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);

            var herbivorousObject = HerbivorousPool.GetObject();
            herbivorousObject.GetComponent<Agent>().GiveBrain(Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]));
            //herbivorousObject.GetComponent<Agent>().brain.InitializeBrain(FindObjectOfType<Academy>(), null);
            herbivorousObject.transform.parent = Ground.transform;
            herbivorousObject.SetActive(true);
            //herbivorousObject.GetComponent<LivingBeingAgent>().ResetPosition(Ground.transform);
            
            
            
            
            var carnivorousObject = CarnivorousPool.GetObject();
            carnivorousObject.GetComponent<Agent>().GiveBrain(Brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]));
            carnivorousObject.transform.parent = Ground.transform;
            carnivorousObject.SetActive(true);
            
            
            var herbObject = HerbPool.GetObject();
            herbObject.transform.parent = Ground.transform;
            herbObject.SetActive(true);
            
        }
    }
}
