using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Utils;
using UnityEngine;
using UnityEngine.AI;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

namespace Evol.Heuristic
{
	public class Movement : MonoBehaviour
	{
		[Header("Parameters")]
		public bool DebugPath;

		[Tooltip("Whether to use NavMeshAgent or Rigidbody")] public bool useNavMeshAgent = true;
		
		[Header("Audio")]
		public AudioClip[] MoveClip;                // Audio that plays when each movement is fired.
		
		[Header("Animations")]
		public string[] WalkingAnimations;
		public string[] RunningAnimations;
		
		[HideInInspector] public NavMeshAgent navMeshAgent;

		private Animator animator;
		private Rigidbody rbody;
		private List<Vector3> path;
		private LineRenderer lr;
		private int speedFloat; // Speed parameter on the Animator.
		private AudioSource audioSource;

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
			rbody = GetComponent<Rigidbody>();
			audioSource = GetComponent<AudioSource>();
			if (DebugPath)
			{
				path = new List<Vector3>();
				lr = gameObject.AddComponent<LineRenderer>();
			}
			
			speedFloat = Animator.StringToHash("speed");
			if(speedFloat == 0) print($"failed hash {gameObject.name}");
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
			animator.SetFloat(speedFloat, Mathf.Clamp(Vector3.Distance(transform.position, destination), 0, 1), 0.1f, Time.deltaTime);
			// animator.SetFloat(speedFloat, navMeshAgent.velocity.magnitude, 0.1f, Time.deltaTime);
			if (useNavMeshAgent)
			{
				navMeshAgent.destination = destination;
				Resume();
			}
			else
			{
				
			}

			if (DebugPath)
			{
				path.Add(destination);
				if (path.Count > 10) // Clear a bit
				{
					path.RemoveRange(0, 10);
				}
			}

			//navMeshAgent.velocity.Set(navMeshAgent.velocity.x, navMeshAgent.velocity.y - 0.1f, navMeshAgent.velocity.z);
			// navMeshAgent.FindClosestEdge(out var hit);
			// transform.position = Vector3.Slerp(transform.position, hit.position, Time.deltaTime);
			
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