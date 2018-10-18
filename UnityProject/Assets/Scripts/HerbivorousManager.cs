using System.Collections;
using System.Collections.Generic;
using Prometheus;
using UnityEngine;

namespace Evol
{
    public class HerbivorousManager : LivingBeingManager
    {
        protected override void Start()
        {
            base.Start();
            
            actionsGauge = Metrics.CreateGauge("actionsHerbivorous", "Amount of actions done until death herbivorous");
            lifeLossGauge = Metrics.CreateGauge("lifeLossHerbivorous", "Life loss per action herbivorous");
            rewardOnDeathGauge = Metrics.CreateGauge("rewardOnDeathHerbivorous", "Reward on death herbivorous");
        }
    }
}