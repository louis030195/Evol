using System.Collections;
using System.Collections.Generic;
using Prometheus;
using UnityEngine;

namespace Evol
{
    public class HerbivorousController : LivingBeingController
    {
        protected override void Start()
        {
            base.Start();
            
            actionsGauge = Metrics.CreateGauge("actionsHerbivorous", "Amount of actions done until death");
        }
    }
}