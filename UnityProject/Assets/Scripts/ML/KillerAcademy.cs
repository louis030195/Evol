using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

namespace Evol.ML
{
    public class KillerAcademy : Academy
    {
        public float agentPrecision = 5f;
        public float targetSize = 10f;

        public override void AcademyStep()
        {
            if (GetTotalStepCount() % 1000000 == 0)
            {
                agentPrecision *= 2;
                //targetSize /= 2;
                // print($"New params \nagentPrecision:{agentPrecision}\ntargetSize:{targetSize}");
            }
                
        }
    }
}