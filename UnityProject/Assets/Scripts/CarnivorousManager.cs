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
            
            actionsGauge = Metrics.CreateGauge("actionsCarnivorous", "Amount of actions done until death");
            lifeLossGauge = Metrics.CreateGauge("lifeLossCarnivorous", "Carnivorous life loss per action");
        }
    }
}
