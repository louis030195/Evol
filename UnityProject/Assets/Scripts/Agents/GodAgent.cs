using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Utils;
using MLAgents;
using UnityEngine;

namespace Evol.Agents
{
        /// <summary>
        /// The goal of this agent is to balance the eco system so that all species survive the longest time possible
        /// </summary>
        public class GodAgent : Agent
        {
                public Pool HerbivorousPool { get; set; }
                
                public Pool CarnivorousPool { get; set; }

                private int carnivorousPreviousAmountActions;
                private int herbivorousPreviousAmountActions;
                
                public override void InitializeAgent()
                {
                }

                public override void CollectObservations()
                {
                        // We use average even if they all have the same brain so the same reward,
                        // just for having a single instead of an array
                        AddVectorObs(carnivorousPreviousAmountActions);
                        
                        // AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                        
                        AddVectorObs(herbivorousPreviousAmountActions);
                        
                        // AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LivingBeing.Speed);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                }

                public override void AgentAction(float[] vectorAction, string textAction)
                {

                        // The agent can make the speed decrease or increase in order to maximize the cumulative reward
                        /*
                        HerbivorousPool.inUse.FindAll(go => go.GetComponent(typeof(LivingBeingAgent)))
                                .ForEach(go => go.GetComponent<LivingBeingAgent>().LivingBeing.Speed += Mathf.Clamp(vectorAction[0], -0.1f, 0.1f));
                        CarnivorousPool.inUse.FindAll(go => go.GetComponent(typeof(LivingBeingAgent)))
                                .ForEach(go => go.GetComponent<LivingBeingAgent>().LivingBeing.Speed += Mathf.Clamp(vectorAction[1], -0.1f, 0.1f));
                        */
                        
                        // Handle life loss per action
                        HerbivorousPool.inUse.ForEach(go => go.GetComponent<LivingBeingManager>().LifeLoss = Mathf.Clamp(vectorAction[0], 0f, 1f));
                        CarnivorousPool.inUse.ForEach(go => go.GetComponent<LivingBeingManager>().LifeLoss = Mathf.Clamp(vectorAction[1], 0f, 1f));
                        
                        // Handle life gain on eat
                        HerbivorousPool.inUse.ForEach(go => go.GetComponent<LivingBeingAgent>().LifeGain = Mathf.Clamp(vectorAction[2], 0f, 100f));
                        CarnivorousPool.inUse.ForEach(go => go.GetComponent<LivingBeingAgent>().LifeGain = Mathf.Clamp(vectorAction[3], 0f, 100f));


                        // If the cumulative reward is higher than the previous cumulative reward,
                        // we reward the god agent so that he learns to adjust well the stats of the agents
                        
                        
                        if(CarnivorousPool.inUse
                                .Select(go => go.GetComponent<LivingBeingAgent>().LivingBeing.LifeExpectancy)
                                          .Average() - carnivorousPreviousAmountActions > 0
                                &&
                        HerbivorousPool.inUse
                                .Select(go => go.GetComponent<LivingBeingAgent>().LivingBeing.LifeExpectancy)
                                .Average() - herbivorousPreviousAmountActions > 0)
                                AddReward(1f);
                        else
                                AddReward(-1f);


                        

                        
                        // Store the previous amount of actions
                        carnivorousPreviousAmountActions = (int)CarnivorousPool.inUse
                                .Select(go => go.GetComponent<LivingBeingAgent>().LivingBeing.LifeExpectancy).Average();
                        
                        herbivorousPreviousAmountActions = (int)HerbivorousPool.inUse
                                .Select(go => go.GetComponent<LivingBeingAgent>().LivingBeing.LifeExpectancy).Average();
                }

        }
}
