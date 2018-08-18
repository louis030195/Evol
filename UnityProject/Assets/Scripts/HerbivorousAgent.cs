using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the herbivorous agent
/// </summary>
public class HerbivorousAgent : Agent
{

    RayPerception rayPer;
    Rigidbody agentRB;
    LivingBeing livingBeing;
    float previousLife;

    int amountActions = 0;
    
    public bool useVectorObs; // Use vector observation or visual (pixels, camera) ?

    public LivingBeing getLivingBeing()
    {
        return livingBeing;
    }

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
        // Cache the agent rigidbody
        agentRB = GetComponent<Rigidbody>();
        livingBeing = new Herbivorous(99, 0, 20, 99, 0);
        previousLife = livingBeing.Life + 1; // +1 to push the agent to survive
    }


    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            var rayDistance = 5f;
            float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
            var detectableObjects = new[] { "food" };
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
            AddVectorObs(gameObject.transform.rotation.z);
            AddVectorObs(gameObject.transform.rotation.x);
        }

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // AddReward(-0.01f);


        // Reset every 1000 actions or when the agent fell
        if (amountActions > 1000 || transform.position.y < 0)
        {
            amountActions = 0;
            Done(); 
        }
        // Move
        transform.Rotate(new Vector3(0, Mathf.Clamp(vectorAction[1], -1f, 1f), 0), Time.fixedDeltaTime * 500);
        transform.Translate(new Vector3(0, 0, 0.1f) * Mathf.Clamp(vectorAction[0], 0f , 2f));
        
        if (livingBeing.Life == 0) // Dead
        {

            AddReward(-10f);
            Done();
        }
        else
            AddReward(0.01f);
           
        if(amountActions > 10) // After a certain amount of actions
            previousLife = livingBeing.Life;

        amountActions++;
    }



    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void AgentReset()
    {
        transform.position = new Vector3(Random.Range(-3f, 3f), 0.05f, Random.Range(-3f, 3f));
        transform.rotation = new Quaternion(0, 0, 0, 0);
        livingBeing.Satiety = 99;
        livingBeing.Life = 99;
        previousLife = 99;
    }
}
