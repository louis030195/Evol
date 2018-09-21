using DesignPattern.Objectpool;
using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// This class handles all the game global events
/// </summary>
public class GameController : MonoBehaviour {

    [Header("Workers")]
    [Space(10)]
    public List<Worker> workers;
    public List<Brain> brains; // Give all the brains you use in all workers

    [Header("Items To Spawn")]
    [Space(10)]
    public List<GameObject> itemsToSpawn;

    [Header("Misc")]
    [Space(10)]
    public bool resetWorkers = true;


    private List<GameObject> workerObjects;
    private int frames;
    private Pool herbivorousPool;
    private Pool carnivorousPool;
    private Pool herbPool;

    // Use this for initialization
    void Start () {
        workerObjects = new List<GameObject>();
        herbivorousPool = new Pool(itemsToSpawn.FirstOrDefault(go => go.CompareTag("herbivorous")));
        carnivorousPool = new Pool(itemsToSpawn.FirstOrDefault(go => go.CompareTag("carnivorous")));
        herbPool = new Pool(itemsToSpawn.FirstOrDefault(go => go.CompareTag("food")));
        
        herbivorousPool.Brain = brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
        carnivorousPool.Brain = brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);


        SpawnWorkers();
    }

    void SpawnWorkers()
    {
        int w = 0;
        foreach (Worker worker in workers)
        {
            float groundSize = worker.WorkerPrefab.transform.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x;
            for (; w < worker.AmountOfWorkers; w++)
            {
                GameObject workerObject = Instantiate(worker.WorkerPrefab, new Vector3(2 * groundSize * w, 0, 0), new Quaternion(0, 0, 0, 0));
                for (int i = 0; i < worker.AmountOfAgentsToAdd; i++)
                {
                    try
                    {
                        GameObject CarnivorousChild = carnivorousPool.GetObject();
                        CarnivorousChild.transform.parent = workerObject.transform;
                        CarnivorousChild.SetActive(true);
                        CarnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition();

                        GameObject herbivorousChild = herbivorousPool.GetObject();
                        herbivorousChild.transform.parent = workerObject.transform;
                        herbivorousChild.SetActive(true);
                        herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition();

                        GameObject herbChild = herbPool.GetObject();
                        herbChild.transform.parent = workerObject.transform;
                        herbChild.SetActive(true);

                    }
                    catch(Exception e) { Debug.Log($"Object { worker.WorkerPrefab.transform.GetChild(i).name } not found in the pool ex: {e.Message}"); }
                    
                }


                // Here we assign the brain to every agent (checking brains list, if the name match with the agent we give brain)
                foreach (Agent agent in workerObject.GetComponentsInChildren<Agent>())
                    foreach (Brain brain in brains.Where(brain => agent.GetType().Name.Contains(Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1])))
                        agent.GiveBrain(brain);

                workerObjects.Add(workerObject);
            }
        }
    }
    

    void FixedUpdate()
    {
        if (frames % 10 == 0)
        {
            System.IO.File.WriteAllText(@"evol.txt", $"\nTime : {Time.fixedTime} seconds \n{herbivorousPool.ToString()}");
            System.IO.File.AppendAllText(@"evol.txt", $"\nTime : {Time.fixedTime} seconds \n{carnivorousPool.ToString()}");
            System.IO.File.AppendAllText(@"evol.txt", $"\nTime : {Time.fixedTime} seconds \n{herbPool.ToString()}");
            frames = 0;

            if (resetWorkers)
            {
                foreach (GameObject workerObject in workerObjects)
                {
                    // TODO : check if any child of LivingBeingAgent is null instead ?
                    if (workerObject.GetComponentInChildren<CarnivorousAgent>() == null 
                        || workerObject.GetComponentInChildren<HerbivorousAgent>() == null
                        || workerObject.GetComponentsInChildren<LivingBeingAgent>().Length > workers[0].AmountOfAgentsToAdd * 10)
                    {
                        foreach (HerbivorousAgent agent in workerObject.GetComponentsInChildren<HerbivorousAgent>())
                        {
                            agent.GetComponent<LivingBeingController>().ResetStats();
                            agent.ResetPosition();
                            agent.Done();
                            herbivorousPool.ReleaseObject(agent.gameObject);
                        }
                        foreach (CarnivorousAgent agent in workerObject.GetComponentsInChildren<CarnivorousAgent>())
                        {
                            agent.GetComponent<LivingBeingController>().ResetStats();
                            agent.ResetPosition();
                            agent.Done();
                            carnivorousPool.ReleaseObject(agent.gameObject);
                        }
                        // TODO : Find a cleaner solution than workers[0] ...
                        for (int i = 0; i < workers[0].AmountOfAgentsToAdd; i++)
                        {
                            GameObject CarnivorousChild = carnivorousPool.GetObject();
                            CarnivorousChild.SetActive(true);
                            CarnivorousChild.transform.parent = workerObject.transform;

                            GameObject herbivorousChild = herbivorousPool.GetObject();
                            herbivorousChild.SetActive(true);
                            herbivorousChild.transform.parent = workerObject.transform;
                        }
                    }
                }
            }
        }
        frames++;
    }
}
