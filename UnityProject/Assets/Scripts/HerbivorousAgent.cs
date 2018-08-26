using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the herbivorous agent
/// </summary>
public class HerbivorousAgent : LivingBeingAgent
{


    public override void InitializeAgent()
    {
        LivingBeing = new Herbivorous(99, 0, 20, 99, 0);
        base.InitializeAgent();
    }

    public override void CollectObservations()
    {
        base.CollectObservations();

        var rayDistance = 5f;
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

            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 999)
            {
                AddReward(10f);
                amountActions = 0;
                Done();
            }

            if (LivingBeing.Life == 0 || transform.GetChild(0).position.y < -10) // Strange, the parent y position doesn't change but the child, so we check child pos
            {
                AddReward(-10f);
                amountActions = 0;
                Done();
            }

        }
        
        else if(rewardMode == RewardMode.Dense)
        {
            // AddReward(-0.015f);
            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 1000)
            {
                //AddReward(-10f);
                //print("I finished after " + amountActions + " actions");
                //amountActions = 0;
                //Done();
            }

            if (transform.GetChild(0).position.y < -2f)
            {
                print("I jumped from the board after " + amountActions + " actions");
                AddReward(-10f);
                amountActions = 0;
                Done();
            }

            if (Vector3.Distance(transform.GetChild(0).position, prevPosition) > 0)
            {
                AddReward(0.01f);
                prevPosition = transform.GetChild(0).position;
            }
        }

        // Move
        base.AgentAction(vectorAction, textAction);
        //transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 500 * Mathf.Clamp(vectorAction[1], -1f, 1f));
        //transform.Translate(new Vector3(0, 0, 0.1f) * Mathf.Clamp(vectorAction[0], 0f, 2f));

        amountActions++;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Herb>() != null)
        {
            LivingBeing.Satiety += 100;
            if (rewardMode == RewardMode.Dense)
            {
                print("I ate something");
                AddReward(20f);
                Done();
            }
        }

        if (collision.collider.GetComponent<CarnivorousAgent>() != null)
        {
            if (rewardMode == RewardMode.Dense)
            {
                AddReward(-1f);
                //Done();
            }
        }
    }
}
