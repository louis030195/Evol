using System.Collections;
using System.Collections.Generic;
using Prometheus;
using UnityEngine;

namespace Evol
{
    public class CarnivorousManager : LivingBeingManager
    {
        protected override void Start()
        {
            base.Start();
            
            actionsGauge = Metrics.CreateGauge("actionsCarnivorous", "Amount of actions done until death carnivorous");
            lifeLossGauge = Metrics.CreateGauge("lifeLossCarnivorous", "Life loss per action carnivorous");
            rewardOnDeathGauge = Metrics.CreateGauge("rewardOnDeathCarnivorous", "Reward on death carnivorous");
        }
    }
}
