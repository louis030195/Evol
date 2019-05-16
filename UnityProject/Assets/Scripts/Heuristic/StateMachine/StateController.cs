using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
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
		[HideInInspector] public Health health;
		[HideInInspector] public AudioSource audioSource;
		[HideInInspector] public Transform master;
		[HideInInspector] public Transform target;
		[HideInInspector] public float stateTimeElapsed;

		private bool aiActive;


		private void Awake()
		{
			attack = GetComponent<Attack>();
			attack.alliesTag = parameters.allies.ToList();
			attack.enemiesTag = parameters.enemies.ToList();
			movement = GetComponent<Movement>();
			health = GetComponent<Health>();
			audioSource = GetComponent<AudioSource>();
		}

		private void Start()
		{
			movement.Speed = parameters.moveSpeed;
		}

		public void SetupAi(bool activate)
		{
			movement.enabled = aiActive = activate;
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