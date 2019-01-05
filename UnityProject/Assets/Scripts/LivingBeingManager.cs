using Evol.Utils;
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

        public bool Reproduction = true;
        public bool Evolution = false;
        public Pool Pool { get; set; }
        public float LifeLoss { get; set; } = 0f;
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
            livingBeingAgent.Reproduction = Reproduction;
            livingBeingAgent.Evolution = Evolution;
            livingBeingAgent.Action = DoAction;
            livingBeingAgent.Pool = Pool;
        }

        // Update is called once per frame
        protected virtual void DoAction()
        {
            lifeLossGauge.Set(LifeLoss);
            
            //if(livingBeingAgent.LivingBeing.Satiety < 90)
            livingBeingAgent.LivingBeing.Life -= LifeLoss;



            if (transform.position.y < 0)
                livingBeingAgent.LivingBeing.Life = -1;


            if (livingBeingAgent.LivingBeing.Life < 0)
            {
                rewardOnDeathGauge.Set(RewardOnDeath);
                
                // Punish the death
                livingBeingAgent.AddReward(RewardOnDeath);

                // Punish if it was the last agent of the specie (genocide)
                //if(transform.parent.GetComponentsInChildren(livingBeingAgent.GetType()).Length == 1)
                //    livingBeingAgent.AddReward(-50f);

                // Remove the agent from the scene
                Pool.ReleaseObject(gameObject);

                livingBeingAgent.Done();
            }

            ClipStats();
        }

        protected void ClipStats()
        {
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

        protected virtual void OnEnable()
        {
            now = Time.fixedTime;
        }

        protected virtual void OnDisable()
        {
            livingBeingAgent.ReproductionsExpectancy = 
                (livingBeingAgent.ReproductionsExpectancy + livingBeingAgent.AmountReproductions) / 2;
            livingBeingAgent.AmountReproductions = 0;
            // Average between old life expectancy and current
            livingBeingAgent.LivingBeing.LifeExpectancy = 
                (livingBeingAgent.LivingBeing.LifeExpectancy + livingBeingAgent.AmountActions) / 2;
            actionsGauge.Set(livingBeingAgent.LivingBeing.LifeExpectancy);
            livingBeingAgent.AmountActions = 0;
            ResetStats();
        }
    }
}
