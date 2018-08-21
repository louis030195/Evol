using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the carnivorous agent
/// TODO : possibility that we can regroup the common code of multiple agents into a mother abstract class
/// </summary>
public class CarnivorousAgent : LivingBeingAgent
{
    public override void InitializeAgent()
    {
        LivingBeing = new Carnivorous(99, 0, 20, 99, 0);
        base.InitializeAgent();
    }

    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            var rayDistance = 5f;
            float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
            var detectableObjects = new[] { "herbivorous" };
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
            AddVectorObs(gameObject.transform.rotation.y);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        action();
        if (rewardMode == RewardMode.Sparse)
        {

            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 999)
            {
                AddReward(10f);
                amountActions = 0;
                Done();
            }

            if (LivingBeing.Life == 0 || transform.position.y < 0) // Dead
            {
                AddReward(-10f);
                amountActions = 0;
                Done();
            }

        }

        else if (rewardMode == RewardMode.Dense)
        {
            AddReward(-0.01f);
            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 1000)
            {
                //AddReward(-10f);
                print("I finished after " + amountActions + " actions");
                amountActions = 0;
                Done();
            }
            else if (transform.position.y < 0)
            {
                print("I jumped from the board after " + amountActions + " actions");
                AddReward(-10f);
                amountActions = 0;
                Done();
            }
        }

        // Move
        transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 500 * Mathf.Clamp(vectorAction[1], -1f, 1f));
        transform.Translate(new Vector3(0, 0, 0.1f) * Mathf.Clamp(vectorAction[0], 0f, 2f));

        amountActions++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            print("Hit HerbivorousAgent");
            LivingBeing.Satiety += 10;
            collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life -= 25;
            if(rewardMode == RewardMode.Dense)
                AddReward(10f);
            Done();
        }
    }

}
