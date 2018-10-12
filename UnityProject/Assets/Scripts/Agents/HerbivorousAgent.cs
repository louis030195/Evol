﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;
using System;
using Evol.Utils;
using Prometheus;

namespace Evol.Agents
{
    /// <summary>
    /// This class handles the behaviour of the herbivorous agent
    /// </summary>
    public class HerbivorousAgent : LivingBeingAgent
    {

        
        public override void InitializeAgent()
        {
            // InitializeAgent seems to be called when gameobject enabled
            LivingBeing = LivingBeing ?? new Herbivorous(50, 0, 0, 50, 0, 50);
            perception = GetComponent<Perception>();
            rigidBody = GetComponent<Rigidbody>();
            
            eatCounter = Metrics.CreateCounter("eatHerbivorous", "How many times herbivorous has eaten");
        }

        public override void CollectObservations()
        {
            var rayDistance = 200f;
            float[] rayAngles = {0f, 45f, 90f, 135f, 180f, 110f, 70f};
            var detectableObjects = new[] {"food"/*, "carnivorous", "herbivorous"*/};
            var detectableObjects2 = new[] {"ground"};
            AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f, Evolve));
            //AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects2, 1f, -10f, Evolve));
            AddVectorObs(gameObject.transform.rotation.y);
            Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
            AddVectorObs(localVelocity.x);
            AddVectorObs(localVelocity.z);
            AddVectorObs(LivingBeing.Life / 100);
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            Action();
            AddReward(-0.01f);
            
            // Move
            rigidBody.AddForce(LivingBeing.Speed * transform.forward * Mathf.Clamp(vectorAction[0], -1f, 1f),
                ForceMode.VelocityChange);
            //transform.Translate(LivingBeing.Speed * transform.forward * Mathf.Clamp(vectorAction[0], -1f, 1f));
            transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 1000 * Mathf.Clamp(vectorAction[1], -1f, 1f));


            AmountActions++;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.GetComponent<Herb>() != null)
            {
                eatCounter.Inc(1.1);
                
                LivingBeing.Satiety += 100;
                LivingBeing.Life += 50;
                AddReward(20f);
                Done();
            }

            if (collision.collider.GetComponent<CarnivorousAgent>() != null)
            {
                LivingBeing.Life = -1;
            }

            if (collision.collider.GetComponent<HerbivorousAgent>() != null)
            {
                if (Evolve)
                {
                    if (LivingBeing.Life >= 90 &&
                        collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life >= 90)
                    {

                        LivingBeing.Life -= 50;
                        collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life -= 50;

                        AddReward(10f);

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
}