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
    public List<TupleWorker> workers; // You have to assign a Prefab containing the ground and agents
    public List<Brain> brains; // Give all the brains you use in all workers

    [Header("Misc")]
    [Space(10)]
    public GameMode gameMode = GameMode.Test;
    

	// Use this for initialization
	void Start () {
        

        switch (gameMode)
        {
            case GameMode.Train:
                foreach (TupleWorker worker in workers)
                {
                    float groundSize = worker.first.transform.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x;
                    for (int w = 0; w < worker.second; w++)
                    {
                        GameObject workerObject = Instantiate(worker.first, new Vector3(2 * groundSize * w, 0, 0), new Quaternion(0, 0, 0, 0));
                        /*foreach (Transform child in workerObject.transform)
                        {
                            child.position = new Vector3(Random.Range(-groundSize, groundSize) + 2 * groundSize * w, 0.5f, Random.Range(-groundSize, groundSize));
                            child.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
                        }*///

                        // Here we assign the brain to every agent (checking brains list, if the name match with the agent we give brain)
                        foreach (Agent agent in workerObject.GetComponentsInChildren<Agent>())
                            foreach (Brain brain in brains.Where(brain => agent.GetType().Name.Contains(Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]))) 
                                agent.GiveBrain(brain);
                            
                            
                    }
                }
                break;
            case GameMode.Test:
                GameObject workerObject2 = Instantiate(workers[0].first, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                foreach (Agent agent in workerObject2.GetComponents<Agent>())
                    agent.GiveBrain(agent.brain);
                break;
        }
        

    }
}
