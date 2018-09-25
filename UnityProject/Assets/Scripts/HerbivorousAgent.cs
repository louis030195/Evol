using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;
using System;
using DesignPattern.Objectpool;

/// <summary>
/// This class handles the behaviour of the herbivorous agent
/// </summary>
public class HerbivorousAgent : LivingBeingAgent
{


    public override void InitializeAgent()
    {
        LivingBeing = new Herbivorous(50, 0, 0, 50, 0, 50);
        perception = GetComponent<Perception>();
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        var rayDistance = 200f;
        float[] rayAngles = {0f, 45f, 90f, 135f, 180f, 110f, 70f };
        var detectableObjects = new[] { "herbivorous", "food", "carnivorous" };
        AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(gameObject.transform.rotation.y);
        Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(LivingBeing.Life);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        action();

        if (rewardMode == RewardMode.Sparse)
        {
            AddReward(0.01f); // Reward for staying alive
            // Reset every 1000 actions or when the agent fell
            if (AmountActions >= 1000)
            {
                AmountActions = 0;
                Done();
            }
        }

        else if (rewardMode == RewardMode.Dense)
        {
            AddReward(-0.01f);
            // Reset every 1000 actions or when the agent fell
            if (!Evolve)
            {
                if (AmountActions >= 1000)
                {
                    AmountActions = 0;
                    Done();
                }
            }
        }

        // Move
        rigidBody.AddForce(LivingBeing.Speed * transform.forward * Mathf.Clamp(vectorAction[0], -1f, 1f), ForceMode.VelocityChange);
        transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 500 * Mathf.Clamp(vectorAction[1], -1f, 1f));


        AmountActions++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Herb>() != null)
        {
            LivingBeing.Satiety += 100;
            LivingBeing.Life += 50;
            if (rewardMode == RewardMode.Dense)
            {
                AddReward(20f);
            }
            Done();
        }

        if (collision.collider.GetComponent<CarnivorousAgent>() != null)
        {
            LivingBeing.Life = -1;
            //print("hit by carni" + LivingBeing.Life);
        }
        if (collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            if (Evolve)
            {
                if (LivingBeing.Life >= 90 && collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life >= 90)
                {

                    LivingBeing.Life -= 50;
                    collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life -= 50;

                    if (rewardMode == RewardMode.Dense)
                    {
                        AddReward(10f);
                    }
                    
                    GameObject go = Pool.GetObject();
                    go.transform.parent = transform.parent;
                    go.transform.position = transform.position;
                    go.SetActive(true);
                    Done();

                }
            }
        }
    }
}