using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Patrol")]
    public class PatrolAction : Action
    {
        public List<Vector3> wayPointList;
        [HideInInspector] public int nextWayPoint;
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            controller.movement.MoveTo(wayPointList[nextWayPoint]);

            if (controller.movement.RemainingDistance <= controller.movement.StoppingDistance && !controller.movement.PathPending) 
            {
                nextWayPoint = (nextWayPoint + 1) % wayPointList.Count;
                Debug.Write($"lol");
            }
        }
    }
}