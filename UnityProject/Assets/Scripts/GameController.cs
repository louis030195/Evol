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
    public int maxHerbs = 20;
    public GameObject herbivorousAgentPrefab;
    public int amountOfHerbivorousAgents = 10;
    public GameObject CarnivorousAgentPrefab;
    public int amountOfCarnivorousAgents = 10;


    int amountOfHerbs = 0;
    List<GameObject> herbsObj;

	// Use this for initialization
	void Start () {

        herbsObj = new List<GameObject>();


        // Spawn at random position on the map and random rotation
        // TODO : check if the random position doesn't collide with another gameobject (RayCast)
        if (herbivorousBrain.brainType == BrainType.Player)
            amountOfHerbivorousAgents = 1; // Only spawn 1 agent if player mode

        // If we need the same amount of herbi / carni agents, could simplify with only 1 loop and a List of prefabs
        for (int i = 0; i < amountOfHerbivorousAgents; i++)
        {
            GameObject agentObj = Instantiate(herbivorousAgentPrefab, new Vector3(Random.Range(-3f, 3f), 0.05f, Random.Range(-3f, 3f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
            Agent agent = agentObj.GetComponent<Agent>();
            agent.GiveBrain(herbivorousBrain); // We need to give brain at runtime when dynamically spawning agent
                                               // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#instantiating-an-agent-at-runtime
        }

        for (int i = 0; i < amountOfCarnivorousAgents; i++)
        {
            GameObject agentObj = Instantiate(CarnivorousAgentPrefab, new Vector3(Random.Range(-3f, 3f), 0.05f, Random.Range(-3f, 3f)), new Quaternion(0, Random.Range(0, 360), 0, 0));
            Agent agent = agentObj.GetComponent<Agent>();
            agent.GiveBrain(carnivorousBrain); // We need to give brain at runtime when dynamically spawning agent
                                               // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#instantiating-an-agent-at-runtime
        }

        InvokeRepeating("SpawnHerbs", 0, 2f); // Maybe use frame instead of second
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnHerbs()
    {
        // To avoid having to many herbs
        if(amountOfHerbs == maxHerbs)
        {
            Destroy(herbsObj[herbsObj.Count - 1]); // Destroy the first herb GameObject instanciated
            amountOfHerbs--;
        }
        herbsObj.Add(Instantiate(herbPrefab, new Vector3(Random.Range(-5f, 5f), 0.05f, Random.Range(-5f, 5f)), new Quaternion(0, Random.Range(0, 360), 0, 0)));
        amountOfHerbs++;
    }
}
