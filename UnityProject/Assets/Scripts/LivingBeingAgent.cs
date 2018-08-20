using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the herbivorous agent
/// TODO : possibility that we can regroup the common code of multiple agents into a mother abstract class
/// </summary>
public abstract class LivingBeingAgent : Agent
{

    protected RayPerception rayPer;
    protected Rigidbody agentRB;
    public LivingBeing LivingBeing { get; protected set; }
    protected float previousLife;
    public System.Action action;

    protected int amountActions = 0;

    public bool useVectorObs; // Use vector observation or visual (pixels, camera) ?

    public enum RewardMode
    {
        Sparse, // very high level reward : harder
        Dense // hand written reward
    }

    public RewardMode rewardMode = RewardMode.Dense;

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
        // Cache the agent rigidbody
        agentRB = GetComponent<Rigidbody>();
        previousLife = LivingBeing.Life + 1; // +1 to push the agent to survive
    }


    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            var rayDistance = 5f;
            float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
            var detectableObjects = new[] { "Untagged" };
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
            AddVectorObs(gameObject.transform.rotation.z);
            AddVectorObs(gameObject.transform.rotation.x);
        }

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        action(); 
        // Reset every 1000 actions or when the agent fell
        if (amountActions > 1000 || transform.position.y < 0)
        {
            amountActions = 0;
            Done();
        }

        // Move
        transform.Rotate(new Vector3(0, Mathf.Clamp(vectorAction[1], -1f, 1f), 0), Time.fixedDeltaTime * 500);
        transform.Translate(new Vector3(0, 0, 0.1f) * Mathf.Clamp(vectorAction[0], 0f, 2f));
        

        amountActions++;
    }



    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void AgentReset()
    {
        transform.position = new Vector3(Random.Range(-3f, 3f), 0.05f, Random.Range(-3f, 3f));
        transform.rotation = new Quaternion(0, 0, 0, 0);
        LivingBeing.Satiety = 49;
        LivingBeing.Life = 99;
        previousLife = 99;
    }
}
