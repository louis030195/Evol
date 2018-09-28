using DesignPattern.Objectpool;
using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This class handles all the game global events
/// </summary>
public class GameController : MonoBehaviour {

    [Header("Workers")]
    [Space(10)]
    public Worker WorkerCarniHerbi;
    public int GroundScale = 10;
    public List<Brain> Brains; // Give all the brains you use in all workers

    [Header("Items To Spawn")]
    [Space(10)]
    public List<GameObject> ItemsToSpawn;

    [Header("Misc")]
    [Space(10)]
    public bool ResetWorkers = true;


    private List<GameObject> workerObjects;
    private int frames;
    private Pool herbivorousPool;
    private Pool carnivorousPool;
    private Pool herbPool;
    private Academy evolAcademy;
    private int configuration = 0;

    // Use this for initialization
    private void Start () {
        evolAcademy = FindObjectOfType<EvolAcademy>();
        
        workerObjects = new List<GameObject>();
        herbivorousPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("herbivorous")));
        carnivorousPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("carnivorous")));
        herbPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("food")));
        
        herbivorousPool.Brain = Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
        carnivorousPool.Brain = Brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);


        SpawnWorkers();
    }

    private void SpawnWorkers()
    {
        int w = 0;
        for (; w < WorkerCarniHerbi.AmountOfWorkers; w++)
        {
            GameObject workerObject = Instantiate(WorkerCarniHerbi.WorkerPrefab, new Vector3(2 * GroundScale * 10 * w, 0, 0),
                new Quaternion(0, 0, 0, 0));
            workerObject.transform.Find("Ground").localScale = new Vector3(GroundScale, 1, GroundScale);
            for (int i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
            {
                try
                {
                    GameObject carnivorousChild = carnivorousPool.GetObject();
                    carnivorousChild.transform.parent = workerObject.transform;
                    carnivorousChild.SetActive(true);
                    carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);

                    GameObject herbivorousChild = herbivorousPool.GetObject();
                    herbivorousChild.transform.parent = workerObject.transform;
                    herbivorousChild.SetActive(true);
                    herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);

                    GameObject herbChild = herbPool.GetObject();
                    herbChild.transform.parent = workerObject.transform;
                    herbChild.SetActive(true);

                }
                catch(Exception e) { Debug.Log($"Object { WorkerCarniHerbi.WorkerPrefab.transform.GetChild(i).name } not found in the pool ex: {e.Message}"); }
                
            }


            // Here we assign the brain to every agent (checking brains list, if the name match with the agent we give brain)
            foreach (Agent agent in workerObject.GetComponentsInChildren<Agent>())
                foreach (Brain brain in Brains.Where(brain => agent.GetType().Name.Contains(Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1])))
                    agent.GiveBrain(brain);

            workerObjects.Add(workerObject);
            
        }
    }
    
    /// <summary>
    /// Configures the agent to his curriculum, adjusting ground scale and amount of agents on academy reset
    /// </summary>
    private void ConfigureAgent()
    {
        // TODO : WARNING:mlagents.trainers:Two or more curriculums will attempt to change the same reset parameter.
        // The result will be non-deterministic.
        // Its not very important ...
        foreach (GameObject workerObject in workerObjects)
        {
            workerObject.transform.Find("Ground").localScale = new Vector3(evolAcademy.resetParameters["ground_scale"],
                1,
                evolAcademy.resetParameters["ground_scale"]);      
        }
        WorkerCarniHerbi.AmountOfAgentsToAdd = (int)evolAcademy.resetParameters["amount_of_agents"];
    }

    private void Reset()
    {
        foreach (GameObject workerObject in workerObjects)
        {
            // TODO : check if any child of LivingBeingAgent is null instead ?
            if (workerObject.GetComponentInChildren<CarnivorousAgent>() == null 
                || workerObject.GetComponentInChildren<HerbivorousAgent>() == null
                || workerObject.GetComponentsInChildren<LivingBeingAgent>().Length > WorkerCarniHerbi.AmountOfAgentsToAdd * 10)
            {
                foreach (HerbivorousAgent agent in workerObject.GetComponentsInChildren<HerbivorousAgent>())
                {
                    agent.GetComponent<LivingBeingController>().ResetStats();
                    agent.Done();
                    herbivorousPool.ReleaseObject(agent.gameObject);
                }
                foreach (CarnivorousAgent agent in workerObject.GetComponentsInChildren<CarnivorousAgent>())
                {
                    agent.GetComponent<LivingBeingController>().ResetStats();
                    agent.Done();
                    carnivorousPool.ReleaseObject(agent.gameObject);
                }
                // TODO : Find a cleaner solution than workers[0] ...
                for (int i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
                {
                    GameObject carnivorousChild = carnivorousPool.GetObject();
                    carnivorousChild.transform.parent = workerObject.transform;
                    carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                    carnivorousChild.SetActive(true);
                    

                    GameObject herbivorousChild = herbivorousPool.GetObject();
                    herbivorousChild.transform.parent = workerObject.transform;
                    herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                    herbivorousChild.SetActive(true);
                    
                }
            }
        }
    }
    

    private void FixedUpdate()
    {
        ConfigureAgent();
        
        
        if (frames % 10 == 0)
        {
            System.IO.File.WriteAllText(@"evol.txt", $"\nTime : {Time.fixedTime} seconds \nHerbivorous {herbivorousPool.ToString()}");
            System.IO.File.AppendAllText(@"evol.txt", $"\nTime : {Time.fixedTime} seconds \nCarnivorous {carnivorousPool.ToString()}");
            System.IO.File.AppendAllText(@"evol.txt", $"\nTime : {Time.fixedTime} seconds \nHerb {herbPool.ToString()}");
            frames = 0;

            if (ResetWorkers)
            {
                Reset();
            }
        }
        frames++;
    }
}
