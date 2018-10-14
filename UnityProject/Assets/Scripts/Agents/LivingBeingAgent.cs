using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.Serialization;
using Evol.Utils;
using Prometheus;

namespace Evol.Agents
{
    /// <summary>
    /// This class handles the behaviour of the agent
    /// </summary>
    public abstract class LivingBeingAgent : Agent
    {
        protected Rigidbody rigidBody;
        protected Perception perception;
        protected Counter eatCounter;
        protected Gauge cumulativeRewardGauge;
        protected Counter reproductionCounter;
        protected Gauge lifeGainGauge;
        
        
        public LivingBeing LivingBeing { get; protected set; }
        public bool Evolve { get; set; }
        public Pool Pool { get; set; }
        public System.Action Action;

        public int AmountActions { get; set; } = 0;
        public float LifeGain { get; set; } = 50f;

        public enum RewardMode
        {
            Sparse, // very high level reward : harder
            Dense // hand written reward
        }

        [Header("Reward stuff")] [Space(10)] public RewardMode rewardMode = RewardMode.Dense;

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            lifeGainGauge.Set(LifeGain);
            
            Action();
            AddReward(-0.01f);
            

            // Move
            rigidBody.AddForce(LivingBeing.Speed * transform.forward * Mathf.Clamp(vectorAction[0], -1f, 1f),
                ForceMode.VelocityChange);
            transform.Rotate(new Vector3(0, 1f, 0), Time.fixedDeltaTime * 1000 * Mathf.Clamp(vectorAction[1], -1f, 1f));

            AmountActions++;
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
    }
}