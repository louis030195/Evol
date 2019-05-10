using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Patrol")]
    public class PatrolAction : Action
    {
        [Tooltip("Distance to be from a waypoint to switch to another")] public int precision = 5;
        [Tooltip("Distance to be from a waypoint to switch to another")] public GameObject[] wayPointList;
        [HideInInspector] public int nextWayPoint;
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            controller.movement.MoveTo(wayPointList[nextWayPoint].transform.position);

            if (Vector3.Distance(controller.transform.position, wayPointList[nextWayPoint].transform.position) < precision) 
            {
                nextWayPoint = (nextWayPoint + 1) % wayPointList.Length;
            }
        }
    }
}