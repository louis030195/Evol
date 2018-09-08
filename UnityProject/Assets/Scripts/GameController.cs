using MLAgents;
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
                        Transform childTransform = Instantiate(worker.WorkerPrefab.transform.GetChild(i));
                        childTransform.parent = workerObject.transform;
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
            if (resetWorkers)
            {
                foreach (GameObject workerObject in workerObjects)
                {
                    System.IO.File.WriteAllText(@"evol.txt", $"Amount of living beings : { workerObject.GetComponentsInChildren<LivingBeingAgent>().Length }" +
                        $"\n {workerObject.GetComponentsInChildren<HerbivorousAgent>().Length} Herbivorous" +
                        $"\n {workerObject.GetComponentsInChildren<CarnivorousAgent>().Length} Carnivorous");

                    if (workerObject.GetComponentsInChildren<LivingBeingAgent>().Length > 15)
                    {
                        foreach (LivingBeingAgent agent in workerObject.GetComponentsInChildren<LivingBeingAgent>())
                        {
                            agent.Done();
                            Destroy(agent.GetComponentInParent<MeshFilter>());
                            Destroy(agent.gameObject);
                        }
                    }
                    // TODO : check if any child of LivingBeingAgent is null instead ?
                    if (workerObject.GetComponentInChildren<CarnivorousAgent>() == null || workerObject.GetComponentInChildren<HerbivorousAgent>() == null)
                    {
                        // TODO : Find a cleaner solution than workers[0] ...
                        for (int i = 0; i < workers[0].AmountOfAgentsToAdd.Count; i++)
                        {
                            for (int j = 0; j < workers[0].AmountOfAgentsToAdd[i]; j++)
                            {
                                // Check if the gameobject isnt already present (camera, herbs ...)
                                if (workerObject.transform.Find(workers[0].WorkerPrefab.transform.GetChild(i).name) == null)
                                {
                                    Transform childTransform = Instantiate(workers[0].WorkerPrefab.transform.GetChild(i));
                                    childTransform.parent = workerObject.transform;
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
