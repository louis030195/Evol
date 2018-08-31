using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;

/// <summary>
/// This class handles the behaviour of the herbivorous agent
/// </summary>
public class HerbivorousAgent : LivingBeingAgent
{


    public override void InitializeAgent()
    {
        LivingBeing = new Herbivorous(99, 0, 0, 99, 0);
        rayPer = GetComponent<RayPerception>();
    }

    public override void CollectObservations()
    {
        var rayDistance = 200f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
        var detectableObjects = new[] { "food", "carnivorous" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(gameObject.transform.rotation.y);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        action();
        if (rewardMode == RewardMode.Sparse)
        {
            AddReward(0.01f); // Reward for staying alive
            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 1000)
            {
                //print("I finished after " + amountActions + " actions");
                amountActions = 0;
                Done();
            }
            else if (transform.position.y < 0)
            {
                // print("I jumped from the board after " + amountActions + " actions");
                // AddReward(-10f);
                LivingBeing.Life -= 100;
                amountActions = 0;
                ResetPosition();
                // Done();
            }else if (LivingBeing.Life == 0)
            {
                AddReward(-10f);
            }
        }

        else if (rewardMode == RewardMode.Dense)
        {
            AddReward(-0.015f);
            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 1000)
            {
                //print("I finished after " + amountActions + " actions");
                amountActions = 0;
                Done();
            }
            else if (transform.position.y < 0)
            {
                // print("I jumped from the board after " + amountActions + " actions");
                AddReward(-10f);
                amountActions = 0;
                ResetPosition();
                Done();
            }
        }

        // Move
        transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 500 * Mathf.Clamp(vectorAction[1], -1f, 1f));
        transform.Translate(new Vector3(0, 0, 1f) * Mathf.Clamp(vectorAction[0], 0f, 2f));

        amountActions++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Herb>() != null)
        {
            LivingBeing.Satiety += 100;
            LivingBeing.Life += 50;
            if (rewardMode == RewardMode.Dense)
            {
                // print("I ate something");
                AddReward(20f);
            }
            Done();
        }

        if (collision.collider.GetComponent<CarnivorousAgent>() != null)
        {
            LivingBeing.Life -= 50;
            if (rewardMode == RewardMode.Dense)
            {
                AddReward(-1f);
            }
            Done();
            ResetPosition();
        }
    }

    public override void AgentReset()
    {
        LivingBeing.Satiety = 100;
        LivingBeing.Life = 100;
    }
}