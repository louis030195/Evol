using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Patrol")]
    public class PatrolAction : Action
    {
        [Tooltip("Distance to be from a waypoint to switch to another")] public int precision = 10;
        [Tooltip("Distance to be from a waypoint to switch to another")] public List<Vector3> wayPointList;
        [HideInInspector] public int nextWayPoint;
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            controller.movement.MoveTo(wayPointList[nextWayPoint]);

            if (Vector3.Distance(controller.transform.position, wayPointList[nextWayPoint]) < precision) 
            {
                nextWayPoint = (nextWayPoint + 1) % wayPointList.Count;
            }
        }
    }
}