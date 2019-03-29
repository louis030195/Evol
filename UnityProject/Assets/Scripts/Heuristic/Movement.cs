using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Evol.Heuristic
{
	public class Movement : MonoBehaviour
	{
		[Header("Parameters")]
		public bool DebugPath;
		
		[Header("Audio")]
		public AudioSource MovingAudio;         // Reference to the audio source used to play the movement audio.
		public AudioClip MoveClip;                // Audio that plays when each movement is fired.
		
		[Header("Animations")]
		public string[] WalkingAnimations;
		public string[] RunningAnimations;
		
		[HideInInspector] public NavMeshAgent navMeshAgent;

		private Animator animator;

		private List<Vector3> path;
		private LineRenderer lr;


		// Broadcasting navmesh params
		public float? RemainingDistance
		{
			get
			{
				if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
					return navMeshAgent.remainingDistance;
				return null;
			}
		}

		
		public bool PathPending => navMeshAgent.pathPending;
		public float StoppingDistance => navMeshAgent.stoppingDistance;

		public float Speed
		{
			set => navMeshAgent.speed = value;
			get => navMeshAgent.speed;
		}
		

		private void Awake()
		{
			animator = GetComponent<Animator>();
			navMeshAgent = GetComponent<NavMeshAgent>();
			if (DebugPath)
			{
				path = new List<Vector3>();
				lr = gameObject.AddComponent<LineRenderer>();
			}
		}

		private void Update()
		{
			if (DebugPath)
			{
				if (path != null && path.Count > 1)
				{
					lr.positionCount = path.Count;
					for (int i = 0; i < path.Count; i++)
					{
						lr.SetPosition(i, path[i]);
					}
				}
			}
		}

		public void MoveTo(Vector3 destination)
		{
			if (animator) 
			{
				if (RunningAnimations.Length > 0)
				{
					var maxRandom = RunningAnimations.Length == 1 ? 0 : RunningAnimations.Length;
					// If there is running animations for this object
					animator.SetBool(RunningAnimations[Random.Range(0, maxRandom)], true);
				}
				else if (WalkingAnimations.Length > 0)
				{
					var maxRandom = WalkingAnimations.Length == 1 ? 0 : WalkingAnimations.Length;
					// Else if there is walking animations for this object
					animator.SetBool(WalkingAnimations[Random.Range(0, maxRandom)], true);
				}
			}
			navMeshAgent.destination = destination;
			if (DebugPath)
			{
				path.Add(destination);
				if (path.Count > 10) // Clear a bit
				{
					path.RemoveRange(0, 10);
				}
			}
			Resume();
		}

		/// <summary>
		/// Useful to let access outside the class to the resume
		/// </summary>
		public void Resume()
		{
			navMeshAgent.isStopped = false;
		}

		/// <summary>
		/// Useful to let access outside the class to the stop
		/// </summary>
		public void Stop()
		{
			navMeshAgent.isStopped = true;
		}
	}
}