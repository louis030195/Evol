using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Utils;
using MLAgents;
using Prometheus;
using UnityEngine;

namespace Evol.Agents
{
        /// <summary>
        /// The goal of this agent is to balance the eco system so that all species survive the longest time possible
        /// </summary>
        public class GodAgent : Agent
        {
                public bool Enable;
                
                public Pool HerbivorousPool { get; set; }
                
                public Pool CarnivorousPool { get; set; }

                public int HerbivorousSpeciesLifeExpectancy { get; set; }
                
                public int CarnivorousSpeciesLifeExpectancy { get; set; }
                
                private int herbivorousPreviousSpeciesLifeExpectancy;
                private int carnivorousPreviousSpeciesLifeExpectancy;
                
                public int HerbivorousReproductionExpectancy { get; set; }
                
                public int CarnivorousReproductionExpectancy { get; set; }
                
                private int herbivorousPreviousReproductionExpectancy;
                private int carnivorousPreviousReproductionExpectancy;

                private Gauge cumulativeRewardGauge;
                private Gauge herbivorousReproductionExpectancyGauge;
                private Gauge carnivorousReproductionExpectancyGauge;
                
                public override void InitializeAgent()
                {
                        if (cumulativeRewardGauge == null)
                        {
                                cumulativeRewardGauge =
                                        Metrics.CreateGauge("cumulativeRewardGod", "Cumulative reward of god");

                                herbivorousReproductionExpectancyGauge =
                                        Metrics.CreateGauge("herbivorousReproductionExpectancyGod", "herbivorous Reproduction Expectancy God");

                                carnivorousReproductionExpectancyGauge =
                                        Metrics.CreateGauge("carnivorousReproductionExpectancyGod", "carnivorous Reproductio nExpectancy God");

                        }

                }

                public override void CollectObservations()
                {
                        AddVectorObs(carnivorousPreviousReproductionExpectancy);
                        
                        AddVectorObs(carnivorousPreviousSpeciesLifeExpectancy);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnAct);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnEat);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnReproduce);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().RewardOnDeath);
                        
                        
                        // ---------------------------------------------------------------------------------------------
                        
                        AddVectorObs(herbivorousPreviousReproductionExpectancy);
                        
                        AddVectorObs(herbivorousPreviousSpeciesLifeExpectancy);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnAct);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnEat);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnReproduce);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingManager>().RewardOnDeath);
                }

                public override void AgentAction(float[] vectorAction, string textAction)
                {
                        // This is just to disable the god agent in game mode
                        if (Enable)
                        {
                                // Handle life loss per action
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().LifeLoss =
                                                Mathf.Clamp(vectorAction[0], 0f, 1f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().LifeLoss =
                                                Mathf.Clamp(vectorAction[1], 0f, 1f));

                                // Handle life gain on eat
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().LifeGain =
                                                Mathf.Clamp(vectorAction[2] * 100, 30f, 100f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().LifeGain =
                                                Mathf.Clamp(vectorAction[3] * 100, 30f, 100f));

                                // Handle speed
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                                                Mathf.Clamp(vectorAction[4] * 100, 30f, 100f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                                                Mathf.Clamp(vectorAction[5] * 100, 30f, 100f));


                                // Handle reward on act
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnAct =
                                                Mathf.Clamp(vectorAction[6], -1f, 0f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnAct =
                                                Mathf.Clamp(vectorAction[7], -1f, 0f));

                                // Handle reward on eat
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnEat =
                                                Mathf.Clamp(vectorAction[8] * 20, 10f, 20f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnEat =
                                                Mathf.Clamp(vectorAction[9] * 20, 10f, 20f));

                                // Handle reward on reproduce
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnReproduce =
                                                Mathf.Clamp(vectorAction[10] * 20, 10f, 20f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnReproduce =
                                                Mathf.Clamp(vectorAction[11] * 20, 10f, 20f));

                                // Handle reward on death
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().RewardOnDeath =
                                                Mathf.Clamp(vectorAction[12] * -10, -5f, -10f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().RewardOnDeath =
                                                Mathf.Clamp(vectorAction[13] * -10, -5f, -10f));
                        }
                        
                        // Reward the god agent if he succeed to make the species live longer over time
                        if(CarnivorousSpeciesLifeExpectancy > carnivorousPreviousSpeciesLifeExpectancy)
                                AddReward(0.0005f);
                        else
                                AddReward(-0.0005f);


                        if(HerbivorousSpeciesLifeExpectancy > herbivorousPreviousSpeciesLifeExpectancy)
                                AddReward(0.0005f);
                        else
                                AddReward(-0.0005f);
                        
                        
                        if(CarnivorousReproductionExpectancy > carnivorousPreviousReproductionExpectancy)
                                AddReward(0.0005f);
                        else
                                AddReward(-0.0005f);


                        if(HerbivorousReproductionExpectancy > herbivorousPreviousReproductionExpectancy)
                                AddReward(0.0005f);
                        else
                                AddReward(-0.0005f);
                        
                        
                        // Store the previous species life expectency if different
                        // else -1 to push the goal forward
                        carnivorousPreviousSpeciesLifeExpectancy =  
                                carnivorousPreviousSpeciesLifeExpectancy != CarnivorousSpeciesLifeExpectancy ?
                                        CarnivorousSpeciesLifeExpectancy :
                                        carnivorousPreviousSpeciesLifeExpectancy - 1;

                        herbivorousPreviousSpeciesLifeExpectancy =  
                                herbivorousPreviousSpeciesLifeExpectancy != HerbivorousSpeciesLifeExpectancy ?
                                        HerbivorousSpeciesLifeExpectancy :
                                        herbivorousPreviousSpeciesLifeExpectancy - 1;
                        
                        carnivorousPreviousReproductionExpectancy =  
                                carnivorousPreviousReproductionExpectancy != CarnivorousReproductionExpectancy ?
                                        CarnivorousReproductionExpectancy :
                                        carnivorousPreviousReproductionExpectancy - 1;

                        herbivorousPreviousReproductionExpectancy =  
                                herbivorousPreviousReproductionExpectancy != HerbivorousReproductionExpectancy ?
                                        HerbivorousReproductionExpectancy :
                                        herbivorousPreviousReproductionExpectancy - 1;


                }
                
                public override void AgentReset()
                {
                        cumulativeRewardGauge.Set(GetCumulativeReward());
                        herbivorousReproductionExpectancyGauge.Set(herbivorousPreviousReproductionExpectancy);
                        carnivorousReproductionExpectancyGauge.Set(carnivorousPreviousReproductionExpectancy);
                }

        }
}
