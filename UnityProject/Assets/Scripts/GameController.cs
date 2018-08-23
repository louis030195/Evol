using MLAgents;
using System.Collections;
using System.Collections.Generic;
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

    



    [Header("HerbivorousAgent")]
    public GameObject herbivorousAgentPrefab;
    public Brain herbivorousBrain;
    public int amountOfHerbivorousAgents = 1;

    [Header("CarnivorousAgent")]
    public GameObject CarnivorousAgentPrefab;
    public Brain carnivorousBrain;
    public int amountOfCarnivorousAgents = 1;

    [Header("CameraAgent")]
    public GameObject camPrefab;
    public Brain cameraBrain;

    [Header("Misc")]
    public GameMode gameMode = GameMode.Test;
    public GameObject groundPrefab;
    public int amountOfWorkers = 10;
    public GameObject herbPrefab;
    public int maxHerbs = 1;

    int amountOfHerbs = 0;
    GameObject[] herbsObj;
    private int nbActions = 0;

	// Use this for initialization
	void Start () {

        

        if (herbivorousBrain.brainType == BrainType.Player)
            amountOfHerbivorousAgents = 1; // Only spawn 1 agent if player mode

        float groundSize = groundPrefab.GetComponent<MeshRenderer>().bounds.size.x;
        float offsetX;

        switch (gameMode)
        {
            case GameMode.Train:
                for (int w = 1; w <= amountOfWorkers; w++)
                {
                    offsetX = 2 * groundSize * w;
                    Instantiate(groundPrefab, new Vector3(2 * groundSize * w, 0, 0), new Quaternion(0, 0, 0, 0));
                    GameObject herbObj = Instantiate(herbPrefab, new Vector3(Random.Range(-5f, 5f) + 2 * groundSize * w, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
                    herbObj.GetComponent<Herb>().OffsetX = offsetX;

                    SpawnAgents(offsetX);
                }
                break;
            case GameMode.Test:
                Instantiate(groundPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                SpawnHerbs();
                SpawnAgents(0);
                break;
        }
        

    }


    void SpawnAgents(float offsetX)
    {
        GameObject camObj = Instantiate(camPrefab, new Vector3(Random.Range(-5f, 5f) + offsetX, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
        CameraAgent camAgent = camObj.GetComponent<CameraAgent>();
        camAgent.GiveBrain(cameraBrain);
        // Spawn at random position on the map and random rotation
        // TODO : check if the random position doesn't collide with another gameobject (RayCast)
        for (int i = 0; i < amountOfHerbivorousAgents; i++)
        {
            GameObject agentObj = Instantiate(herbivorousAgentPrefab, new Vector3(Random.Range(-5f, 5f) + offsetX, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
            camAgent.ThingsToWatch.Add(agentObj);
            LivingBeingAgent agent = agentObj.GetComponent<LivingBeingAgent>();
            agent.OffsetX = offsetX;
            agent.GiveBrain(herbivorousBrain); // We need to give brain at runtime when dynamically spawning agent
                                               // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#instantiating-an-agent-at-runtime
        }

        for (int i = 0; i < amountOfCarnivorousAgents; i++)
        {
            GameObject agentObj = Instantiate(CarnivorousAgentPrefab, new Vector3(Random.Range(-5f, 5f) + offsetX, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
            camAgent.ThingsToWatch.Add(agentObj);
            LivingBeingAgent agent = agentObj.GetComponent<LivingBeingAgent>();
            agent.OffsetX = offsetX;
            agent.GiveBrain(carnivorousBrain); // We need to give brain at runtime when dynamically spawning agent
                                               // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#instantiating-an-agent-at-runtime
        }

        
    }
	
    /*
    void FixedUpdate()
    {
        nbActions++;
        if (nbActions % 10000 == 0)
            SpawnHerbs();
    }
    */
    void SpawnHerbs()
    {
        for(int i = 0;i < maxHerbs; i++)
        {
            Instantiate(herbPrefab, new Vector3(Random.Range(-5f, 5f), 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
        }
    }
}
