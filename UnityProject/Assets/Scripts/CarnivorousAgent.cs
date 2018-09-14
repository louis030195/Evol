﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;
using DesignPattern.Objectpool;
using System;

/// <summary>
/// This class handles the behaviour of the carnivorous agent
/// </summary>
public class CarnivorousAgent : LivingBeingAgent
{
    public override void InitializeAgent()
    {
        LivingBeing = new Carnivorous(50, 0, 0, 50, 0);
        perception = GetComponent<Perception>();
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        var rayDistance = 200f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
        var detectableObjects = new[] { "carnivorous", "herbivorous", "food" };
        AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(gameObject.transform.rotation.y);
        AddVectorObs(LivingBeing.Life);
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
                //print("I jumped from the board after " + amountActions + " actions");
                //AddReward(-10f);
                LivingBeing.Life -= 100;
                amountActions = 0;
                ResetPosition();
                Done();
            }
            else if (LivingBeing.Life == 0)
            {
                AddReward(-10f);
            }
        }

        else if (rewardMode == RewardMode.Dense)
        {
            AddReward(-0.01f);
            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 1000)
            {
                //print("I finished after " + amountActions + " actions");
                amountActions = 0;
                Done();
            }
            else if (transform.position.y < 0)
            {
                //print("I jumped from the board after " + amountActions + " actions");
                amountActions = 0;
                LivingBeing.Life = -1;
            }
        }

        // Move
        rigidBody.AddForce(moveSpeed * transform.forward * Mathf.Clamp(vectorAction[0], -1f, 1f), ForceMode.VelocityChange);
        transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 500 * Mathf.Clamp(vectorAction[1], -1f, 1f));
        //transform.Translate(new Vector3(0, 0, 1f) * Mathf.Clamp(vectorAction[0], 0f, 2f));

        amountActions++;
    }

    public override void AgentReset()
    {
        rigidBody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            LivingBeing.Satiety += 100;
            LivingBeing.Life += 50;
            if (rewardMode == RewardMode.Dense)
                AddReward(20f);
            Done();
        }
        if (collision.collider.GetComponent<CarnivorousAgent>() != null)
        {
            if (Evolve)
            {
                if (LivingBeing.Life >= 90 && collision.collider.GetComponent<CarnivorousAgent>().LivingBeing.Life > 90)
                {
                    LivingBeing.Life -= 50;
                    collision.collider.GetComponent<CarnivorousAgent>().LivingBeing.Life -= 50;

                    if (rewardMode == RewardMode.Dense)
                    {
                        AddReward(10f);
                    }
                    //Instantiate(gameObject, transform.parent); // Create child
                    GameObject go = Pool.GetObject(gameObject.tag);
                    go.transform.parent = transform.parent;
                    go.transform.position = transform.position;
                    Done();
                }
            }
        }
    }
}