using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Evol.Heuristic
{
	public class Movement : MonoBehaviour
	{
		public AudioSource MovingAudio;         // Reference to the audio source used to play the movement audio.
		public AudioClip MoveClip;                // Audio that plays when each movement is fired.
		public string MovingAnimation;
		public bool DebugPath;
		
		[HideInInspector] public NavMeshAgent navMeshAgent;

		private Animator anim;

		private List<Vector3> path;
		private LineRenderer lr;


		// Broadcasting navmesh params
		public float RemainingDistance => navMeshAgent.remainingDistance;
		public bool PathPending => navMeshAgent.pathPending;
		public float StoppingDistance => navMeshAgent.stoppingDistance;

		public float Speed
		{
			set => navMeshAgent.speed = value;
			get => navMeshAgent.speed;
		}
		

		private void Awake()
		{
			anim = GetComponent<Animator>();
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
			if (anim && anim.enabled)
				anim.SetBool(MovingAnimation, true);
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