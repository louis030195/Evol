﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.Serialization;
using Evol.Utils;
using Prometheus;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Evol.Agents
{
    /// <summary>
    /// This class handles the behaviour of the agent
    /// </summary>
    public abstract class LivingBeingAgent : Agent
    {
        protected Rigidbody rigidBody;
        protected Perception perception;
        protected string[] detectableObjects;
        
        // Grafana / Prometheus logs
        protected Counter eatCounter;
        protected Gauge cumulativeRewardGauge;
        protected Counter reproductionCounter;
        protected Gauge lifeGainGauge;
        protected Gauge rewardOnEatGauge;
        protected Gauge rewardOnReproduceGauge;
        protected Gauge rewardOnActGauge;
        
        
        public LivingBeing LivingBeing { get; protected set; }
        public bool Reproduction { get; set; }
        public bool Evolution { get; set; }
        public Pool Pool { get; set; }
        public System.Action Action;

        public int AmountActions { get; set; } = 0;
        
        // Parameters
        public float LifeGain { get; set; } = 50f;
        public float ReproductionTreshold { get; set; } = 51f;
        
        public float RewardOnEat { get; set; } = 10f;
        public float RewardOnReproduce { get; set; } = 10f;
        public float RewardOnAct { get; set; } = -0.01f;

        public enum RewardMode
        {
            Sparse, // very high level reward : harder
            Dense // hand written reward
        }

        public override void InitializeAgent()
        {
            // InitializeAgent seems to be called when gameobject is enabled, we only need to call it once
            if (LivingBeing != null) return;
            perception = GetComponent<Perception>();
            rigidBody = GetComponent<Rigidbody>();

        }

        [Header("Reward stuff")] [Space(10)] public RewardMode rewardMode = RewardMode.Dense;

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            lifeGainGauge.Set(LifeGain);
            rewardOnActGauge.Set(RewardOnAct);
            
            Action();
            AddReward(RewardOnAct);

            // Move
            rigidBody.AddForce(LivingBeing.Speed * transform.forward * Mathf.Clamp(vectorAction[0], -1f, 1f),
                ForceMode.VelocityChange);
            transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 1000 * Mathf.Clamp(vectorAction[1], -1f, 1f));

            AmountActions++;

            if (brain.brainType == BrainType.Heuristic)
            {
                bool detectedSomething = false;
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10);
                int i = 0;
                foreach (var collider in hitColliders)
                {
                    // If we detected our target nearby, reach it
                    if (collider.CompareTag(detectableObjects[0]))
                    {
                        transform.position = Vector3.Lerp(transform.position, collider.transform.position, Time.deltaTime * 1f);
                        detectedSomething = true;
                    }
                }
                if(!detectedSomething)
                    transform.position = Vector3.Lerp(transform.position,
                        new Vector3(Random.Range(transform.position.x - 1f, transform.position.x + 1f),
                            transform.position.y, 
                            Random.Range(transform.position.z - 1f, transform.position.z + 1f)),
                        1f);
            }
        }

        public void ResetPosition(Transform worker)
        {
            rigidBody.velocity = Vector3.zero;
            float groundSize = worker.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2;
            float offsetX = transform.parent.position.x;
            transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, 0.5f,
                Random.Range(-groundSize, groundSize));
            transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
        }

        public override void AgentReset()
        {
            cumulativeRewardGauge.Set(GetCumulativeReward());
        }

        private void OnCollisionExit(Collision other)
        {
            // Custom gravity
            /*
            if(other.gameObject.CompareTag("ground"))
                rigidBody.AddForce(Vector3.down * 50, ForceMode.VelocityChange);
                */
        }

        private void FixedUpdate()
        {
            /*
            // Handling gravity manually ...
            RaycastHit hit;
            // Debug.DrawRay(transform.position,Vector3.down * 10,Color.green);
            try // Just in case we try to get the collider while we are being deactivated
            {
                if (Physics.Raycast(GetComponent<Collider>().bounds.min, -transform.up, out hit, 0.1f))
                {
                    //the ray collided with something, you can interact
                    // with the hit object now by using hit.collider.gameObject
                }
                else
                {
                    //nothing was below your gameObject within 10m.
                    // transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -0.1f, transform.position.z), Time.deltaTime * 2);

                    rigidBody.AddForce(Vector3.down * 50, ForceMode.VelocityChange);
                }
            }
            catch (ArgumentNullException e)
            {
                print(e.Message);
            }*/
        }
    }
}