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


    [Header("Misc")]
    [Space(10)]
    public bool resetWorkers = true;


    private List<GameObject> workerObjects;
    private int frames;

	// Use this for initialization
	void Start () {
        workerObjects = new List<GameObject>();

        // This part is used to initialize the list of objects than need to be added / removed from game a lot during runtime
        List<GameObject> temporaryPrefabs = new List<GameObject>();
        foreach (Worker worker in workers)
        {
            for(int i = 0; i < worker.WorkerPrefab.transform.childCount; i++)
                if(worker.WorkerPrefab.transform.GetChild(i).GetComponent<LivingBeingAgent>() != null ||
                    worker.WorkerPrefab.transform.GetChild(i).GetComponent<Herb>() != null)
                    temporaryPrefabs.Add(worker.WorkerPrefab.transform.GetChild(i).gameObject);
        }
        Pool.Initialize(10 * workers[0].AmountOfWorkers * 3, temporaryPrefabs, brains); // TODO: Pool size changing according limit of living being etc ...

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
                for (int i = 0; i < worker.AmountOfAgentsToAdd.Count; i++)
                {
                    for (int j = 0; j < worker.AmountOfAgentsToAdd[i]; j++)
                    {
                        try
                        {
                            GameObject child = Pool.GetObject(worker.WorkerPrefab.transform.GetChild(i).tag);
                            //Transform childTransform = Instantiate(worker.WorkerPrefab.transform.GetChild(i));
                            child.transform.parent = workerObject.transform;
                        }catch(Exception e) { Debug.Log($"Object { worker.WorkerPrefab.transform.GetChild(i).name } not found in the pool"); }
                    }
                }

                foreach (LivingBeingAgent livingBeingAgent in workerObject.GetComponentsInChildren<LivingBeingAgent>())
                    livingBeingAgent.ResetPosition();


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
        if (frames % 100 == 0)
        {
            System.IO.File.WriteAllText(@"evol.txt", $"Time : {Time.fixedTime} seconds" +
                $"\n Available objects in the pool : {Pool.GetAvailableCount()}" +
                $"\n Objects in use in the pool : {Pool.GetInUseCount()}");
            frames = 0;

            if (resetWorkers)
            {
                foreach (GameObject workerObject in workerObjects)
                {
                    // TODO : check if any child of LivingBeingAgent is null instead ?
                    if (workerObject.GetComponentInChildren<CarnivorousAgent>() == null 
                        || workerObject.GetComponentInChildren<HerbivorousAgent>() == null
                        || workerObject.GetComponentsInChildren<LivingBeingAgent>().Length > 20)
                    {
                        foreach (LivingBeingAgent agent in workerObject.GetComponentsInChildren<LivingBeingAgent>())
                        {
                            agent.Done();
                            //Destroy(agent.GetComponentInParent<MeshFilter>());
                            Pool.ReleaseObject(agent.gameObject);
                        }
                        // TODO : Find a cleaner solution than workers[0] ...
                        for (int i = 0; i < workers[0].AmountOfAgentsToAdd.Count; i++)
                        {
                            for (int j = 0; j < workers[0].AmountOfAgentsToAdd[i]; j++)
                            {
                                //print("Spawn " + workers[0].WorkerPrefab.transform.GetChild(i).name);
                                if (workers[0].WorkerPrefab.transform.GetChild(i).name.Contains("Agent"))
                                {
                                    GameObject child = Pool.GetObject(workers[0].WorkerPrefab.transform.GetChild(i).tag);
                                    //Transform childTransform = Instantiate(workers[0].WorkerPrefab.transform.GetChild(i));
                                    child.transform.parent = workerObject.transform;
                                }
                            }
                        }

                        foreach (LivingBeingAgent livingBeingAgent in workerObject.GetComponentsInChildren<LivingBeingAgent>())
                            livingBeingAgent.ResetPosition();


                        // Here we assign the brain to every agent (checking brains list, if the name match with the agent we give brain)
                        foreach (Agent agent in workerObject.GetComponentsInChildren<Agent>())
                            foreach (Brain brain in brains.Where(brain => agent.GetType().Name.Contains(Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1])))
                                agent.GiveBrain(brain);
                    }
                }
            }
        }
        frames++;
    }
}
