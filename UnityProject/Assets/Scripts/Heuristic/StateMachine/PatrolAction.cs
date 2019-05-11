using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Patrol")]
    public class PatrolAction : Action
    {
        [Tooltip("Distance to be from a waypoint to switch to another")] public int precision = 5;
        [Tooltip("Distance to be from a waypoint to switch to another")] public GameObject wayPointList;
        [HideInInspector] private int nextWayPoint;
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            Debug.Assert(wayPointList.transform.childCount > 0, $"wayPointList.transform.childCount <= 0");
            // Debug.Log($"wayPointList.transform.childCount {wayPointList.transform.childCount} - nextWayPoint {nextWayPoint}");
    
            controller.movement.MoveTo(wayPointList.transform.GetChild(nextWayPoint).position);

            if (Vector3.Distance(controller.transform.position, wayPointList.transform.GetChild(nextWayPoint).position) < precision)
            {
                nextWayPoint++;
                nextWayPoint %= wayPointList.transform.childCount;
            }
        }
    }
}