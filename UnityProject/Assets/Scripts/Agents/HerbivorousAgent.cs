using System.Collections;
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
            // InitializeAgent seems to be called when gameobject is enabled, we only need to call it once
            if (LivingBeing != null) return;
            
            LivingBeing = new Herbivorous(50, 0, 0, 50, 0, 50);
            perception = GetComponent<Perception>();
            rigidBody = GetComponent<Rigidbody>();

            eatCounter = Metrics.CreateCounter("eatHerbivorous", "How many times herbivorous has eaten");
            reproductionCounter = 
                Metrics.CreateCounter("reproductionHerbivorous", "How many times herbivorous has reproduced");
            cumulativeRewardGauge =
                Metrics.CreateGauge("cumulativeRewardHerbivorous", "Cumulative reward of herbivorous");
            lifeGainGauge =
                Metrics.CreateGauge("lifeGainHerbivorous", "Life gain on eat of herbivorous");
        }

        public override void CollectObservations()
        {
            var rayDistance = transform.parent.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2; // For example if ground is of scale 10 = size 100 / 2
            float[] rayAngles = {0f, 45f, 90f, 135f, 180f, 110f, 70f};
            var detectableObjects = new[] {"food", "carnivorous", "herbivorous"};
            var detectableObjects2 = new[] {"ground"};
            AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f, Evolve));
            //AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects2, 1f, -10f, Evolve));
            AddVectorObs(gameObject.transform.rotation.y);
            Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
            AddVectorObs(localVelocity.x);
            AddVectorObs(localVelocity.z);
            AddVectorObs(LivingBeing.Life / 100);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.GetComponent<Herb>() != null)
            {
                eatCounter.Inc(1.1);
                
                LivingBeing.Satiety += 100;
                LivingBeing.Life += LifeGain;
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
                        reproductionCounter.Inc(1.1);

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