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
            base.InitializeAgent();
            LivingBeing = new Herbivorous(50, 0, 0, 50, 0, 10);

            eatCounter = Metrics.CreateCounter("eatHerbivorous", "How many times herbivorous has eaten");
            reproductionCounter = 
                Metrics.CreateCounter("reproductionHerbivorous", "How many times herbivorous has reproduced");
            cumulativeRewardGauge =
                Metrics.CreateGauge("cumulativeRewardHerbivorous", "Cumulative reward of herbivorous");
            lifeGainGauge =
                Metrics.CreateGauge("lifeGainHerbivorous", "Life gain on eat of herbivorous");
            rewardOnActGauge =
                Metrics.CreateGauge("rewardOnActHerbivorous", "Reward on act herbivorous");
            rewardOnEatGauge =
                Metrics.CreateGauge("rewardOnEatHerbivorous", "Reward on eat herbivorous");
            rewardOnReproduceGauge =
                Metrics.CreateGauge("rewardOnReproduceHerbivorous", "Reward on reproduce herbivorous");
            speedGauge =
                Metrics.CreateGauge("speedHerbivorous", "Speed herbivorous");
        }

        public override void CollectObservations()
        {
            var rayDistance = 50;
            float[] rayAngles = {0f, 45f, 90f, 135f, 180f, 110f, 70f};
            detectableObjects = new[] {"Herb", "Carnivorous", "Herbivorous", "Ground"};
            AddVectorObs(perception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f, Reproduction, ReproductionTreshold));
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
                rewardOnEatGauge.Set(RewardOnEat);
                
                LivingBeing.Satiety += 30;
                LivingBeing.Life += LifeGain;
                AddReward(RewardOnEat);
                Done();
            }

            if (collision.collider.GetComponent<CarnivorousAgent>() != null)
            {
                LivingBeing.Life = -1;
            }

            if (collision.collider.GetComponent<HerbivorousAgent>() != null)
            {
                if (Reproduction)
                {
                    
                    if (LivingBeing.Life >= ReproductionTreshold &&
                        collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life >= ReproductionTreshold)
                    {
                        AmountReproductions++;
                        reproductionCounter.Inc(1.1);
                        rewardOnReproduceGauge.Set(RewardOnReproduce);

                        LivingBeing.Life -= 30;
                        collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life -= 30;

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