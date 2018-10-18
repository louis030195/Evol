﻿using Evol.Utils;
using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Evol.Agents;
using Prometheus;

namespace Evol
{
    /// <summary>
    /// This class is used to update stats of the agent
    /// </summary>
    public abstract class LivingBeingManager : MonoBehaviour
    {

        public bool Evolve = true;
        public Pool Pool { get; set; }
        public float LifeLoss { get; set; } = 0.05f;
        public float RewardOnDeath { get; set; } = -10f;

        protected LivingBeingAgent livingBeingAgent;
        protected float now;
        protected Gauge lifeLossGauge;
        protected Gauge actionsGauge;
        protected Gauge rewardOnDeathGauge;

        // Use this for initialization
        protected virtual void Start()
        {
            livingBeingAgent = GetComponent<LivingBeingAgent>();
            livingBeingAgent.Evolve = Evolve;
            livingBeingAgent.Action = DoAction;
            livingBeingAgent.Pool = Pool;
        }

        // Update is called once per frame
        protected virtual void DoAction()
        {
            lifeLossGauge.Set(LifeLoss);
            
            livingBeingAgent.LivingBeing.Life -= LifeLoss;



            if (transform.position.y < 0)
                livingBeingAgent.LivingBeing.Life = -1;


            // To avoid dying instantly before having his stats reset
            if (livingBeingAgent.LivingBeing.Life < 0)
            {
                rewardOnDeathGauge.Set(RewardOnDeath);
                
                // Punish the death
                livingBeingAgent.AddReward(RewardOnDeath);

                // Punish if it was the last agent of the specie (genocide)
                if(transform.parent.GetComponentsInChildren(livingBeingAgent.GetType()).Length == 1)
                    livingBeingAgent.AddReward(-50f);

                // Remove the agent from the scene
                Pool.ReleaseObject(gameObject);

                livingBeingAgent.Done();
            }

            livingBeingAgent.LivingBeing.Life =
                livingBeingAgent.LivingBeing.Life > 100 ? 100 : livingBeingAgent.LivingBeing.Life;

            livingBeingAgent.LivingBeing.Satiety = livingBeingAgent.LivingBeing.Satiety > 100
                ? 100
                : livingBeingAgent.LivingBeing.Satiety;
            
            
            livingBeingAgent.LivingBeing.Speed = livingBeingAgent.LivingBeing.Speed > 100
                ? 100
                : livingBeingAgent.LivingBeing.Speed < 0 ? 0 : livingBeingAgent.LivingBeing.Speed;
        }

        public void ResetStats()
        {
            livingBeingAgent.LivingBeing.Satiety = 50;
            livingBeingAgent.LivingBeing.Life = 50;
        }

        private void FixedUpdate()
        {
            /*
                // Handling gravity manually ...
                RaycastHit hit;
                // Debug.DrawRay(transform.position,Vector3.down * 10,Color.green);
                if(Physics.Raycast(transform.position, new Vector3(transform.position.x, -0.1f, transform.position.z), out hit, 0.1f))
                {
                    //the ray collided with something, you can interact
                    // with the hit object now by using hit.collider.gameObject
                }
                else{
                    //nothing was below your gameObject within 10m.
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -0.1f, transform.position.z), Time.deltaTime * 2);
                }
                */
        }

        protected virtual void OnEnable()
        {
            now = Time.fixedTime;
        }

        protected virtual void OnDisable()
        {
            livingBeingAgent.LivingBeing.LifeExpectancy = 
                (livingBeingAgent.LivingBeing.LifeExpectancy + livingBeingAgent.AmountActions) / 2;
            actionsGauge.Set(livingBeingAgent.LivingBeing.LifeExpectancy);
            livingBeingAgent.AmountActions = 0;
            ResetStats();
        }
    }
}