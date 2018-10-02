using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.Serialization;
using Evol.Utils;

namespace Evol.Agents
{
    /// <summary>
    /// This class handles the behaviour of the agent
    /// </summary>
    public abstract class LivingBeingAgent : Agent
    {
        protected Rigidbody rigidBody;

        protected Perception perception;
        public LivingBeing LivingBeing { get; protected set; }
        public bool Evolve { get; set; }
        public Pool Pool { get; set; }
        public System.Action Action;

        public short AmountActions { get; protected set; } = 0;

        public enum RewardMode
        {
            Sparse, // very high level reward : harder
            Dense // hand written reward
        }

        [Header("Reward stuff")] [Space(10)] public RewardMode rewardMode = RewardMode.Dense;


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
            AmountActions = 0;
        }
    }
}