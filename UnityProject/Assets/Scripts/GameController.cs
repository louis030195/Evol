using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles all the game global events
/// </summary>
public class GameController : MonoBehaviour {

    public Brain herbivorousBrain;
    public Brain carnivorousBrain;
    public GameObject herbPrefab;
    public int maxHerbs = 1;
    public GameObject herbivorousAgentPrefab;
    public int amountOfHerbivorousAgents = 1;
    public GameObject CarnivorousAgentPrefab;
    public int amountOfCarnivorousAgents = 1;
    public GameObject groundPrefab;
    public int amountOfWorkers = 10;


    int amountOfHerbs = 0;
    GameObject[] herbsObj;
    private int nbActions = 0;

	// Use this for initialization
	void Start () {

        herbsObj = new GameObject[maxHerbs];


        // Spawn at random position on the map and random rotation
        // TODO : check if the random position doesn't collide with another gameobject (RayCast)
        if (herbivorousBrain.brainType == BrainType.Player)
            amountOfHerbivorousAgents = 1; // Only spawn 1 agent if player mode

        //SpawnHerbs();
        //InvokeRepeating("SpawnHerbs", 0, 2f); // Maybe use frame instead of second

        // If we need the same amount of herbi / carni agents, could simplify with only 1 loop and a List of prefabs

        float groundSize = groundPrefab.GetComponent<MeshRenderer>().bounds.size.x;

        for (int w = 1; w <= amountOfWorkers; w++)
        {
            Instantiate(groundPrefab, new Vector3(2 * groundSize * w, 0, 0), new Quaternion(0, 0, 0, 0));
            GameObject herbObj = Instantiate(herbPrefab, new Vector3(Random.Range(-5f, 5f) + 2 * groundSize * w, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
            herbObj.GetComponent<Herb>().OffsetX = 2 * groundSize * w;
            herbObj.GetComponent<Herb>().OffsetZ = 0;

            for (int i = 0; i < amountOfHerbivorousAgents; i++)
            {
                GameObject agentObj = Instantiate(herbivorousAgentPrefab, new Vector3(Random.Range(-5f, 5f) + 2 * groundSize * w, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
                LivingBeingAgent agent = agentObj.GetComponent<LivingBeingAgent>();
                agent.OffsetX = 2 * groundSize * w;
                agent.GiveBrain(herbivorousBrain); // We need to give brain at runtime when dynamically spawning agent
                                                   // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#instantiating-an-agent-at-runtime
            }

            for (int i = 0; i < amountOfCarnivorousAgents; i++)
            {
                GameObject agentObj = Instantiate(CarnivorousAgentPrefab, new Vector3(Random.Range(-5f, 5f) + 2 * groundSize * w, 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
                LivingBeingAgent agent = agentObj.GetComponent<LivingBeingAgent>();
                agent.OffsetX = 2 * groundSize * w;
                agent.GiveBrain(carnivorousBrain); // We need to give brain at runtime when dynamically spawning agent
                                                   // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#instantiating-an-agent-at-runtime
            }
        }
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    void FixedUpdate()
    {
        nbActions++;
        if (nbActions % 10000 == 0)
            SpawnHerbs();
    }

    void SpawnHerbs()
    {
        for(int i = 0;i < maxHerbs; i++)
        {
            if(amountOfHerbs == maxHerbs) Destroy(herbsObj[i]);
            herbsObj[i] = Instantiate(herbPrefab, new Vector3(Random.Range(-5f, 5f), 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
        }
        amountOfHerbs = maxHerbs;
    }*/
}
