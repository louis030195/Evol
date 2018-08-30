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

    public enum GameMode
    {
        Train,
        Test // Other modes ? Like real video game ?
    }

    [Header("Workers")]
    [Space(10)]
    public List<Worker> workers;
    public List<Brain> brains; // Give all the brains you use in all workers


    [Header("Useless atm")]
    [Space(10)]
    public GameMode gameMode = GameMode.Train;
    

	// Use this for initialization
	void Start () {
        

        switch (gameMode)
        {
            case GameMode.Train:
                foreach (Worker worker in workers)
                {
                    float groundSize = worker.WorkerPrefab.transform.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x;
                    for (int w = 0; w < worker.AmountOfWorkers; w++)
                    {
                        GameObject workerObject = Instantiate(worker.WorkerPrefab, new Vector3(2 * groundSize * w, 0, 0), new Quaternion(0, 0, 0, 0));
                        for (int i = 0; i < worker.AmountOfAgents.Count; i++){
                            for (int j = 0; j < worker.AmountOfAgents[i]; j++) {
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
                            
                            
                    }
                }
                break;
            case GameMode.Test:
                GameObject workerObject2 = Instantiate(workers[0].WorkerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                foreach (Agent agent in workerObject2.GetComponents<Agent>())
                    agent.GiveBrain(agent.brain);
                break;
        }
        

    }
}
