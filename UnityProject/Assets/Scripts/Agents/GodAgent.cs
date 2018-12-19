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

                public int HerbivorousSpeciesLifeExpectency { get; set; }
                
                public int CarnivorousSpeciesLifeExpectency { get; set; }
                
                private int herbivorousPreviousSpeciesLifeExpectency;
                private int carnivorousPreviousSpeciesLifeExpectency;

                private Gauge cumulativeRewardGauge;
                
                public override void InitializeAgent()
                {
                        if(cumulativeRewardGauge == null)
                                cumulativeRewardGauge =
                                        Metrics.CreateGauge("cumulativeRewardGod", "Cumulative reward of god");

                }

                public override void CollectObservations()
                {
                        AddVectorObs(carnivorousPreviousSpeciesLifeExpectency);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnAct);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnEat);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnReproduce);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().RewardOnDeath);
                        
                        
                        // ---------------------------------------------------------------------------------------------
                        
                        AddVectorObs(herbivorousPreviousSpeciesLifeExpectency);
                        
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
                                                Mathf.Clamp(vectorAction[8] * 10, 0f, 10f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnEat =
                                                Mathf.Clamp(vectorAction[9] * 10, 0f, 10f));

                                // Handle reward on reproduce
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnReproduce =
                                                Mathf.Clamp(vectorAction[10] * 10, 0f, 10f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnReproduce =
                                                Mathf.Clamp(vectorAction[11] * 10, 0f, 10f));

                                // Handle reward on death
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().RewardOnDeath =
                                                Mathf.Clamp(vectorAction[12] * 20, 0f, 20f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().RewardOnDeath =
                                                Mathf.Clamp(vectorAction[13] * 20, 0f, 20f));
                        }
                        
                        // Reward the god agent if he succeed to make the species live longer over time
                        if(CarnivorousSpeciesLifeExpectency > carnivorousPreviousSpeciesLifeExpectency)
                                AddReward(0.0005f);
                        else
                                AddReward(-0.0005f);


                        if(HerbivorousSpeciesLifeExpectency > herbivorousPreviousSpeciesLifeExpectency)
                                AddReward(0.0005f);
                        else
                                AddReward(-0.0005f);
                        
                        
                        // Store the previous species life expectency if different
                        // else -1 to push the goal forward
                        carnivorousPreviousSpeciesLifeExpectency =  
                                carnivorousPreviousSpeciesLifeExpectency != CarnivorousSpeciesLifeExpectency ?
                                        CarnivorousSpeciesLifeExpectency :
                                        carnivorousPreviousSpeciesLifeExpectency - 1;

                        herbivorousPreviousSpeciesLifeExpectency =  
                                herbivorousPreviousSpeciesLifeExpectency != HerbivorousSpeciesLifeExpectency ?
                                        HerbivorousSpeciesLifeExpectency :
                                        herbivorousPreviousSpeciesLifeExpectency - 1;


                }
                
                public override void AgentReset()
                {
                        cumulativeRewardGauge.Set(GetCumulativeReward());
                }

        }
}
