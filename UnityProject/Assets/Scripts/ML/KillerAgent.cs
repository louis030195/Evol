using System.Collections.Generic;
using System.Linq;
using Evol.Game.Ability;
using Evol.Game.Player;
using Evol.Heuristic;
using Evol.Utils;
using UnityEngine;
using MLAgents;
using UnityEngine.Events;

namespace Evol.ML
{
    public class KillerAgent : Agent
    {
        public string[] alliesTag;
        public string[] enemiesTag;
        public Terrain ground;
        public Transform target;
        public float speed = 1;
        [HideInInspector] public UnityEvent<float> hitEnemy = new FloatEvent();

        private Attack attack;
        private Movement movement;
        private Rigidbody rbody;
        private Rigidbody targetRigidBody;
        private KillerAcademy academy;

        private void Awake()
        {
            attack = GetComponent<Attack>();
            attack.offline = true;
            movement = GetComponent<Movement>();
            rbody = GetComponent<Rigidbody>();
            targetRigidBody = GetComponent<Rigidbody>();
            hitEnemy.AddListener(data => // If we hit enemy, reward
            {
                //print("hit");
                SetReward(data); // Reward proportional to damage dealt
                Done();
            });
            attack.alliesTag = alliesTag.ToList();
            attack.enemiesTag = enemiesTag.ToList();
            academy = GameObject.Find("Academy").GetComponent<KillerAcademy>();
        }

        public override void CollectObservations()
        {
            // Target and Agent positions
            var position = target.position;
            AddVectorObs(position);
            //AddVectorObs(targetRigidBody.velocity); // The agent knows if his target is moving
            //AddVectorObs(targetRigidBody.rotation); // The agent know the target rotation (useful to predict movement direction)
            //AddVectorObs(Vector3.Distance(transform.position, position));
            /*
            AddVectorObs(ground.terrainData.size.x);
            AddVectorObs(ground.terrainData.size.y);
            var data = ground.terrainData;
            var terrainData = new List<float>();
            for (var i = 0; i < data.size.x; i++) {
                for (var j = 0; j < data.size.y; j++)
                {
                    terrainData.Add(data.GetHeight(i, j));
                    // print(ground.terrainData.GetHeight(i, j));
                }
            }
            AddVectorObs(terrainData);*/
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            // var pos = transform.position; 
            /*
            transform.Rotate(transform.up * vectorAction[1], Time.fixedDeltaTime * 200f);
            rbody.AddForce(transform.forward * vectorAction[0] * speed,
                ForceMode.VelocityChange);*/

            // The agent give as output a position relative to his own
            /*
            if (vectorAction.Any(value => value != 0.0f))
            {
                // print($"Moving at { vectorAction[0] } - { vectorAction[1] }");
                // Only call if moving
                movement.MoveTo(new Vector3(pos.x + vectorAction[0], pos.y + vectorAction[1], pos.z + vectorAction[2]));
                // movement.MoveTo(new Vector3(pos.x + vectorAction[0], pos.y, pos.z + vectorAction[1]));
            }
            else
            {
                movement.Stop();
            }*/

            // Rewards
            /*
            var currentDistanceWithTarget = Vector3.Distance(transform.position,
                target.position);
            
            // SetReward(Mathf.Clamp(currentDistanceWithTarget, 1, 0));
            AddReward(Mathf.Clamp(currentDistanceWithTarget, 1, 0));
    
            // Reached target
            if (currentDistanceWithTarget < 1.42f)
            {
                Done();
            }
    
            // Fell off platform
            if (transform.position.y < 0)
            {
                Done();
            }*/
            if (vectorAction[0] > 0.0f) // If he wanna throw ability
            {
                var position = target.position;
                var vec = new Vector3(
                    position.x + Mathf.Clamp(vectorAction[1] * academy.agentPrecision, -academy.agentPrecision,
                        academy.agentPrecision),
                    position.y + Mathf.Clamp(vectorAction[2] * academy.agentPrecision, -academy.agentPrecision,
                        academy.agentPrecision),
                    position.z + Mathf.Clamp(vectorAction[3] * academy.agentPrecision, -academy.agentPrecision,
                        academy.agentPrecision));
                //print($"{vectorAction[1] * academy.agentPrecision}\n{vectorAction[1]}\n{academy.agentPrecision}\n{Mathf.Clamp(vectorAction[1] * academy.agentPrecision, -academy.agentPrecision,academy.agentPrecision)}");
                attack.AttackNow(vec);
            }
        }

        public override void AgentReset()
        {
            // print($"Cumulative reward {GetCumulativeReward()} - Reward {GetReward()}");
            //if (transform.position.y < 0)
            //{
            // If the Agent fell, reset his position
            var terrainData = ground.terrainData;
            transform.position = Position.AboveGround(new Vector3(Random.value * terrainData.size.x * 0.8f,
                50f,
                Random.value * terrainData.size.x * 0.8f), 1);
            rbody.velocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;
            //}

            // Move the target to a new spot
            target.position = Position.AboveGround(new Vector3(Random.value * terrainData.size.x * 0.8f,
                50f,
                Random.value * terrainData.size.x * 0.8f), target.localScale.magnitude * 2);
            targetRigidBody.velocity = Vector3.zero;
            targetRigidBody.angularVelocity = Vector3.zero;
        }

/*
    private bool IsGrounded()
    {
        var ray = new Ray(transform.position + Vector3.up * 2, Vector3.down);
        return Physics.SphereCast(ray, 1, 1 + 0.2f); 
    }

    private void FixedUpdate()
    {
        if(!IsGrounded())
            rbody.AddForce(-transform.up, ForceMode.VelocityChange);
    }*/
    }
}