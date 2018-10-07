using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Utils;
using MLAgents;
using UnityEngine;

namespace Evol.Agents
{
        public class GodAgent : Agent
        {
                public Pool HerbivorousPool { get; set; }
                
                public Pool CarnivorousPool { get; set; }

                private float carnivorousPreviousCumulativeReward;
                private float herbivorousPreviousCumulativeReward;
                
                public override void InitializeAgent()
                {
                }

                public override void CollectObservations()
                {
                        // We use average even if they all have the same brain so the same reward,
                        // just for having a single instead of an array
                        AddVectorObs(HerbivorousPool.inUse.Select(go => go.GetComponent<LivingBeingAgent>().GetCumulativeReward()).ToArray().Average());
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                        AddVectorObs(CarnivorousPool.inUse.Select(go => go.GetComponent<LivingBeingAgent>().GetCumulativeReward()).ToArray().Average());
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                }

                public override void AgentAction(float[] vectorAction, string textAction)
                {
                        /*
                        if (Time.fixedTime > 100 && Time.fixedTime % 50 < 1)
                        {
                                print($"Carnivorous previous cumulative reward {carnivorousPreviousCumulativeReward}");
                                print($"Speed {CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed}");
                                print($"Herbivorous previous cumulative reward {herbivorousPreviousCumulativeReward}");
                                print($"Speed {HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed}");
                        }*/
/*
                        // The agent can make the speed decrease or increase in order to maximize the cumulative reward
                        HerbivorousPool.inUse.FindAll(go => go.GetComponent(typeof(LivingBeingAgent)))
                                .ForEach(go => go.GetComponent<LivingBeingAgent>().LivingBeing.Speed += Mathf.Clamp(vectorAction[0], -0.1f, 0.1f));
                        CarnivorousPool.inUse.FindAll(go => go.GetComponent(typeof(LivingBeingAgent)))
                                .ForEach(go => go.GetComponent<LivingBeingAgent>().LivingBeing.Speed += Mathf.Clamp(vectorAction[1], -0.1f, 0.1f));


                        // If the cumulative reward is higher than the previous cumulative reward,
                        // we reward the god agent so that he learns to adjust well the stats of the agents
                        if (carnivorousPreviousCumulativeReward < CarnivorousPool.inUse
                                    .Select(go => go.GetComponent<LivingBeingAgent>().GetCumulativeReward()).ToArray()
                                    .Average()
                            && herbivorousPreviousCumulativeReward < HerbivorousPool.inUse
                                    .Select(go => go.GetComponent<LivingBeingAgent>().GetCumulativeReward()).ToArray()
                                    .Average())
                        {
                                AddReward(1f);
                        }
                        else
                        {
                                AddReward(-1f);
                        }
                        
                        // Storage the previous cumulative reward
                        carnivorousPreviousCumulativeReward = CarnivorousPool.inUse
                                .Select(go => go.GetComponent<LivingBeingAgent>().GetCumulativeReward()).ToArray()
                                .Average();
                        
                        herbivorousPreviousCumulativeReward = HerbivorousPool.inUse
                                .Select(go => go.GetComponent<LivingBeingAgent>().GetCumulativeReward()).ToArray()
                                .Average();*/
                }

        }
}
