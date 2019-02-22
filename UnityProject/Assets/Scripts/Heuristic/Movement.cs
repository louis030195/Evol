using System;
using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Evol.Heuristic
{
	public class Movement : MonoBehaviour
	{
		public AudioSource MovingAudio;         // Reference to the audio source used to play the movement audio.
		public AudioClip MoveClip;                // Audio that plays when each movement is fired.

		
		[HideInInspector] public NavMeshAgent navMeshAgent;

		private Animator anim;

		private void Awake()
		{
			anim = GetComponent<Animator>();
			navMeshAgent = GetComponent<NavMeshAgent>();
			navMeshAgent.speed = 20;
		}

		private void Update()
		{
			if (Math.Abs(navMeshAgent.remainingDistance) < 20)
			{
				if (anim && anim.enabled)
				{
					anim.SetBool("run", false);
					anim.SetBool("creep", true);
				}
			}
		}

		public void MoveTo(Vector3 destination)
		{
			if (anim && anim.enabled)
				anim.SetBool("run", true);
			navMeshAgent.destination = destination;
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