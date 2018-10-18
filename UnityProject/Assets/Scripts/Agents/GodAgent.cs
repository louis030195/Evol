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
                public Pool HerbivorousPool { get; set; }
                
                public Pool CarnivorousPool { get; set; }

                private int carnivorousPreviousAmountActions;
                private int herbivorousPreviousAmountActions;
                
                public override void InitializeAgent()
                {
                }

                public override void CollectObservations()
                {
                        AddVectorObs(carnivorousPreviousAmountActions);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                        
                        //AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnAct);
                        
                        //AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnEat);
                        
                        //AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnReproduce);
                        
                        //AddVectorObs(CarnivorousPool.inUse.First().GetComponent<LivingBeingManager>().RewardOnDeath);
                        
                        
                        // ---------------------------------------------------------------------------------------------
                        
                        AddVectorObs(herbivorousPreviousAmountActions);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingManager>().LifeLoss);
                        
                        AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().LifeGain);
                        
                        //AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnAct);
                        
                        //AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnEat);
                        
                        //AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingAgent>().RewardOnReproduce);
                        
                        //AddVectorObs(HerbivorousPool.inUse.First().GetComponent<LivingBeingManager>().RewardOnDeath);
                }

                public override void AgentAction(float[] vectorAction, string textAction)
                {
                        // Not sure if we even use godai in test
                        if (brain.brainType == BrainType.External || brain.brainType == BrainType.Internal)
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
                                                Mathf.Clamp(vectorAction[2], 0f, 100f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().LifeGain =
                                                Mathf.Clamp(vectorAction[3], 0f, 100f));
                                /*
                                // Handle reward on act
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnAct =
                                                Mathf.Clamp(vectorAction[4], -1f, 0f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnAct =
                                                Mathf.Clamp(vectorAction[5], -1f, 0f));
                                
                                // Handle reward on eat
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnEat =
                                                Mathf.Clamp(vectorAction[6], 0f, 10f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnEat =
                                                Mathf.Clamp(vectorAction[7], 0f, 10f));
                                
                                // Handle reward on reproduce
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnReproduce =
                                                Mathf.Clamp(vectorAction[8], 0f, 10f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingAgent>().RewardOnReproduce =
                                                Mathf.Clamp(vectorAction[9], 0f, 10f));
                                
                                // Handle reward on death
                                HerbivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().RewardOnDeath =
                                                Mathf.Clamp(vectorAction[10], 0f, 20f));
                                CarnivorousPool.inUse.ForEach(go =>
                                        go.GetComponent<LivingBeingManager>().RewardOnDeath =
                                                Mathf.Clamp(vectorAction[11], 0f, 20f));
                                                */
                        }

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
