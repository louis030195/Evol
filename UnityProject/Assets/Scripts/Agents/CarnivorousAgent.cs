using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;
using Evol.Utils;
using System;
using Prometheus;


namespace Evol.Agents
{
    /// <summary>
    /// This class handles the behaviour of the carnivorous agent
    /// </summary>
    public class CarnivorousAgent : LivingBeingAgent
    {

        public override void InitializeAgent()
        {
            base.InitializeAgent();
            LivingBeing = new Carnivorous(50, 0, 0, 50, 0, 50);
            
            eatCounter = Metrics.CreateCounter("eatCarnivorous", "How many times carnivorous has eaten");
            reproductionCounter = 
                Metrics.CreateCounter("reproductionCarnivorous", "How many times carnivorous has reproduced");
            cumulativeRewardGauge = 
                Metrics.CreateGauge("cumulativeRewardCarnivorous", "Cumulative reward of carnivorous");
            lifeGainGauge =
                Metrics.CreateGauge("lifeGainCarnivorous", "Life gain on eat of carnivorous");
            rewardOnActGauge =
                Metrics.CreateGauge("rewardOnActCarnivorous", "Reward on act carnivorous");
            rewardOnEatGauge =
                Metrics.CreateGauge("rewardOnEatCarnivorous", "Reward on eat carnivorous");
            rewardOnReproduceGauge =
                Metrics.CreateGauge("rewardOnReproduceCarnivorous", "Reward on reproduce carnivorous");
            speedGauge =
                Metrics.CreateGauge("speedCarnivorous", "Speed carnivorous");
        }


        public override void CollectObservations()
        {
            var rayDistance = transform.parent.Find("Ground") != null ?
                transform.parent.Find("Ground").GetComponent<MeshRenderer>() == null ?
                    transform.parent.Find("Ground").GetComponent<Terrain>().terrainData.size.x / 4 :
                    transform.parent.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 4
                : 0; // For example if ground is of scale 10 = size 100 / 2
            float[] rayAngles = {0f, 45f, 90f, 135f, 180f, 110f, 70f};
            detectableObjects = new[] {"Herbivorous", "Carnivorous", "Herb"};
            var detectableObjects2 = new[] {"Ground"};
            AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f, Reproduction, ReproductionTreshold));
            //AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects2, 1f, -10f, Reproduction, ReproductionTreshold));
            Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
            AddVectorObs(localVelocity.x);
            AddVectorObs(localVelocity.z);
            AddVectorObs(gameObject.transform.rotation.y);
            AddVectorObs(LivingBeing.Life / 100);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.GetComponent<HerbivorousAgent>() != null)
            {
                eatCounter.Inc(1.1);
                rewardOnEatGauge.Set(RewardOnEat);
                
                LivingBeing.Satiety += 30;
                LivingBeing.Life += LifeGain;
                AddReward(RewardOnEat);
                Done();
            }

            if (collision.collider.GetComponent<CarnivorousAgent>() != null)
            {
                if (Reproduction)
                {
                    
                    if (LivingBeing.Life >= ReproductionTreshold &&
                        collision.collider.GetComponent<CarnivorousAgent>().LivingBeing.Life > ReproductionTreshold)
                    {
                        reproductionCounter.Inc(1.1);
                        rewardOnReproduceGauge.Set(RewardOnReproduce);
                        
                        LivingBeing.Life -= 30;
                        collision.collider.GetComponent<CarnivorousAgent>().LivingBeing.Life -= 30;

                        AddReward(RewardOnReproduce);

                        GameObject go = Pool.GetObject();
                        go.transform.parent = transform.parent;
                        go.SetActive(true);
                        go.transform.position = transform.position;
                        
                        if (Evolution)
                        {
                            go.GetComponent<LivingBeingAgent>().LivingBeing.Speed = (LivingBeing.Speed +
                                                                                     collision.collider
                                                                                         .GetComponent<LivingBeingAgent
                                                                                         >().LivingBeing.Speed) / 2
                                                                                    + UnityEngine.Random.Range(-0.1f,
                                                                                        0.1f);
                        }

                        Done();
                    }
                }
            }
        }
    }
}