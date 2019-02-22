using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// using Complete;

namespace Evol.Heuristic.StateMachine
{
	public class StateController : MonoBehaviour
	{

		public State currentState;
		public Parameters parameters;
		public Transform eyes;
		public State remainState;


		[HideInInspector] public Movement movement;
		[HideInInspector] public Attack attack;
		[HideInInspector] public List<Transform> wayPointList;
		[HideInInspector] public int nextWayPoint;
		[HideInInspector] public Transform chaseTarget;
		[HideInInspector] public float stateTimeElapsed;

		private bool aiActive;


		private void Awake()
		{
			attack = GetComponent<Attack>();
			movement = GetComponent<Movement>();
		}

		public void SetupAi(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
		{
			wayPointList = wayPointsFromTankManager;
			aiActive = aiActivationFromTankManager;
			if (aiActive)
			{
				movement.enabled = true;
			}
			else
			{
				movement.enabled = false;
			}
		}

		private void Update()
		{
			if (!aiActive)
				return;
			currentState.UpdateState(this);
		}

		private void OnDrawGizmos()
		{
			if (currentState != null && eyes != null)
			{
				Gizmos.color = currentState.sceneGizmoColor;
				Gizmos.DrawWireSphere(eyes.position, parameters.lookSphereCastRadius);
			}
		}

		public void TransitionToState(State nextState)
		{
			if (nextState != remainState)
			{
				currentState = nextState;
				OnExitState();
			}
		}

		public bool CheckIfCountDownElapsed(float duration)
		{
			stateTimeElapsed += Time.deltaTime;
			return (stateTimeElapsed >= duration);
		}

		private void OnExitState()
		{
			stateTimeElapsed = 0;
		}
	}
}