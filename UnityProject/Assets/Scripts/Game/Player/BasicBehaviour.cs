using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace Evol.Game.Player
{
// This class manages which player behaviour is active or overriding, and call its local functions.
// Contains basic setup and common functions used by all the player behaviours.
	public class BasicBehaviour : MonoBehaviour
	{
		[Header("Parameters")]
		[Tooltip("Debugging mode = doesn't care about the cursor visible stuff")] public bool debug;
		[Tooltip("Reference to the camera that focus the player.")] public Transform playerCamera;
		[Tooltip("Speed of turn when moving to match camera facing.")] public float turnSmoothing = 0.06f;
		[Tooltip("the FOV to use on the camera when player is sprinting.")] public float sprintFOV = 100f;
		[Tooltip("Default sprint button input name.")] public string sprintButton = "Sprint";

		private float h; // Horizontal Axis.
		private float v; // Vertical Axis.
		private int currentBehaviour; // Reference to the current player behaviour.
		private int defaultBehaviour; // The default behaviour of the player when any other is not active.
		private int behaviourLocked; // Reference to temporary locked behaviour that forbids override.
		private Vector3 lastDirection; // Last direction the player was moving.
		private Animator anim; // Reference to the Animator component.
		private ThirdPersonOrbitCamBasic camScript; // Reference to the third person camera script.
		private bool sprint; // Boolean to determine whether or not the player activated the sprint mode.
		private bool changedFOV; // Boolean to store when the sprint action has changed de camera FOV.
		private int hFloat; // Animator variable related to Horizontal Axis.
		private int vFloat; // Animator variable related to Vertical Axis.
		private List<GenericBehaviour> behaviours; // The list containing all the enabled player behaviours.
		private List<GenericBehaviour> overridingBehaviours; // List of current overriding behaviours.
		private Rigidbody rBody; // Reference to the player's rigidbody.
		private int groundedBool; // Animator variable related to whether or not the player is on the ground.
		private Vector3 colExtents; // Collider extents for ground test. 

		// Get current horizontal and vertical axes.
		public float GetH => h;
		public float GetV => v;

		// Get the player camera script.
		public ThirdPersonOrbitCamBasic GetCamScript => camScript;

		// Get the player's rigid body.
		public Rigidbody GetRigidBody => rBody;

		// Get the player's animator controller.
		public Animator GetAnim => anim;

		// Get current default behaviour.
		public int GetDefaultBehaviour => defaultBehaviour;

		private void Awake()
		{
			// Multiplayer, deactivate the component if it's not mine
			if(gameObject.GetPhotonView() != null && !gameObject.GetPhotonView().IsMine && PhotonNetwork.InRoom) // InRoom check is for debugging offline
				enabled = false;
			
			// Set up the references.
			behaviours = new List<GenericBehaviour>();
			overridingBehaviours = new List<GenericBehaviour>();
			anim = GetComponent<Animator>();
			hFloat = Animator.StringToHash("H");
			vFloat = Animator.StringToHash("V");
			camScript = playerCamera.GetComponent<ThirdPersonOrbitCamBasic>();
			rBody = GetComponent<Rigidbody>();

			// Grounded verification variables.
			groundedBool = Animator.StringToHash("grounded");
			colExtents = GetComponent<Collider>().bounds.extents;
		}

		private void Update()
		{
			// If cursor is visible lock the rotation
			if (Cursor.visible && !debug) 
				return;

			// Store the input axes.
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");

			// Set the input axes on the Animator Controller.
			anim.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
			anim.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

			// Toggle sprint by input.
			sprint = Input.GetButton(sprintButton);

			// Set the correct camera FOV for sprint mode.
			if (IsSprinting())
			{
				changedFOV = true;
				camScript.SetFOV(sprintFOV);
			}
			else if (changedFOV)
			{
				camScript.ResetFOV();
				changedFOV = false;
			}

			// Set the grounded test on the Animator Controller.
			anim.SetBool(groundedBool, IsGrounded());
		}

		// Call the FixedUpdate functions of the active or overriding behaviours.
		private void FixedUpdate()
		{
			// If cursor is visible lock the rotation
			if (Cursor.visible)
				return;	
			
			// Call the active behaviour if no other is overriding.
			var isAnyBehaviourActive = false;
			if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
			{
				foreach (var behaviour in behaviours)
				{
					if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
					{
						isAnyBehaviourActive = true;
						behaviour.LocalFixedUpdate();
					}
				}
			}
			// Call the overriding behaviours if any.
			else
			{
				foreach (var behaviour in overridingBehaviours)
				{
					behaviour.LocalFixedUpdate();
				}
			}

			// Ensure the player will stand on ground if no behaviour is active or overriding.
			if (!isAnyBehaviourActive && overridingBehaviours.Count == 0)
			{
				rBody.useGravity = true;
				Repositioning();
			}
		}

		// Call the LateUpdate functions of the active or overriding behaviours.
		private void LateUpdate()
		{
			// Call the active behaviour if no other is overriding.
			if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
			{
				foreach (var behaviour in behaviours)
				{
					if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
					{
						behaviour.LocalLateUpdate();
					}
				}
			}
			// Call the overriding behaviours if any.
			else
			{
				foreach (var behaviour in overridingBehaviours)
				{
					behaviour.LocalLateUpdate();
				}
			}

		}

		// Put a new behaviour on the behaviours watch list.
		public void SubscribeBehaviour(GenericBehaviour behaviour)
		{
			behaviours.Add(behaviour);
		}

		// Set the default player behaviour.
		public void RegisterDefaultBehaviour(int behaviourCode)
		{
			defaultBehaviour = behaviourCode;
			currentBehaviour = behaviourCode;
		}

		// Attempt to set a custom behaviour as the active one.
		// Always changes from default behaviour to the passed one.
		public void RegisterBehaviour(int behaviourCode)
		{
			if (currentBehaviour == defaultBehaviour)
			{
				currentBehaviour = behaviourCode;
			}
		}

		// Attempt to deactivate a player behaviour and return to the default one.
		public void UnregisterBehaviour(int behaviourCode)
		{
			if (currentBehaviour == behaviourCode)
			{
				currentBehaviour = defaultBehaviour;
			}
		}

		// Attempt to override any active behaviour with the behaviours on queue.
		// Use to change to one or more behaviours that must overlap the active one (ex.: aim behaviour).
		public bool OverrideWithBehaviour(GenericBehaviour behaviour)
		{
			// Behaviour is not on queue.
			if (!overridingBehaviours.Contains(behaviour))
			{
				// No behaviour is currently being overridden.
				if (overridingBehaviours.Count == 0)
				{
					// Call OnOverride function of the active behaviour before overrides it.
					foreach (var overriddenBehaviour in behaviours)
					{
						if (overriddenBehaviour.isActiveAndEnabled &&
						    currentBehaviour == overriddenBehaviour.GetBehaviourCode())
						{
							overriddenBehaviour.OnOverride();
							break;
						}
					}
				}

				// Add overriding behaviour to the queue.
				overridingBehaviours.Add(behaviour);
				return true;
			}

			return false;
		}

		// Attempt to revoke the overriding behaviour and return to the active one.
		// Called when exiting the overriding behaviour (ex.: stopped aiming).
		public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
		{
			if (overridingBehaviours.Contains(behaviour))
			{
				overridingBehaviours.Remove(behaviour);
				return true;
			}

			return false;
		}

		// Check if any or a specific behaviour is currently overriding the active one.
		public bool IsOverriding(GenericBehaviour behaviour = null)
		{
			if (behaviour == null)
				return overridingBehaviours.Count > 0;
			return overridingBehaviours.Contains(behaviour);
		}

		// Check if the active behaviour is the passed one.
		public bool IsCurrentBehaviour(int behaviourCode)
		{
			return currentBehaviour == behaviourCode;
		}

		// Check if any other behaviour is temporary locked.
		public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
		{
			return behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf;
		}

		// Atempt to lock on a specific behaviour.
		//  No other behaviour can overrhide during the temporary lock.
		// Use for temporary transitions like jumping, entering/exiting aiming mode, etc.
		public void LockTempBehaviour(int behaviourCode)
		{
			if (behaviourLocked == 0)
			{
				behaviourLocked = behaviourCode;
			}
		}

		// Attempt to unlock the current locked behaviour.
		// Use after a temporary transition ends.
		public void UnlockTempBehaviour(int behaviourCode)
		{
			if (behaviourLocked == behaviourCode)
			{
				behaviourLocked = 0;
			}
		}

		// Common functions to any behaviour:

		// Check if player is sprinting.
		public virtual bool IsSprinting()
		{
			return sprint && IsMoving() && CanSprint();
		}

		// Check if player can sprint (all behaviours must allow).
		public bool CanSprint()
		{
			foreach (var behaviour in behaviours)
			{
				if (!behaviour.AllowSprint())
					return false;
			}

			foreach (var behaviour in overridingBehaviours)
			{
				if (!behaviour.AllowSprint())
					return false;
			}

			return true;
		}

		// Check if the player is moving on the horizontal plane.
		public bool IsHorizontalMoving()
		{
			return h != 0;
		}

		// Check if the player is moving.
		public bool IsMoving()
		{
			return h != 0 || v != 0;
		}

		// Get the last player direction of facing.
		public Vector3 GetLastDirection()
		{
			return lastDirection;
		}

		// Set the last player direction of facing.
		public void SetLastDirection(Vector3 direction)
		{
			lastDirection = direction;
		}

		// Put the player on a standing up position based on last direction faced.
		public void Repositioning()
		{
			if (lastDirection != Vector3.zero)
			{
				lastDirection.y = 0;
				var targetRotation = Quaternion.LookRotation(lastDirection);
				var newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmoothing);
				rBody.MoveRotation(newRotation);
			}
		}

		// Function to tell whether or not the player is on ground.
		public bool IsGrounded()
		{
			// TODO: think about this 0.2f value corresponding to sensibility to say if im grounded or not
			Debug.DrawRay(transform.position + Vector3.up * 2 * colExtents.x, Vector3.down, Color.green, colExtents.x + 0.2f, true);
			var ray = new Ray(transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
			return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f); 
		}
	}
}
