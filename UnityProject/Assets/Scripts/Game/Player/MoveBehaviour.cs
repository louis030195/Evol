﻿using UnityEngine;

namespace Evol.Game.Player
{
// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
	public class MoveBehaviour : GenericBehaviour
	{
		public float walkSpeed = 0.15f; // Default walk speed.
		public float runSpeed = 1.0f; // Default run speed.
		public float sprintSpeed = 2.0f; // Default sprint speed.
		public float speedDampTime = 0.1f; // Default damp time to change the animations based on current speed.
		public string jumpButton = "Jump"; // Default jump button.
		public float jumpHeight = 1.5f; // Default jump height.
		public float jumpIntertialForce = 10f; // Default horizontal inertial force when jumping.

		private float speed, speedSeeker; // Moving speed.
		private int jumpBool; // Animator variable related to jumping.
		private int groundedBool; // Animator variable related to whether or not the player is on ground.
		private bool jump; // Boolean to determine whether or not the player started a jump.
		private bool isColliding; // Boolean to determine if the player has collided with an obstacle.

		// Start is always called after any Awake functions.
		private void Start()
		{
			// Set up the references.
			jumpBool = Animator.StringToHash("Jump");
			groundedBool = Animator.StringToHash("Grounded");
			behaviourManager.GetAnim.SetBool(groundedBool, true);

			// Subscribe and register this behaviour as the default behaviour.
			behaviourManager.SubscribeBehaviour(this);
			behaviourManager.RegisterDefaultBehaviour(behaviourCode);
			speedSeeker = runSpeed;
		}

		// Update is used to set features regardless the active behaviour.
		private void Update()
		{
			// Get jump input.
			if (!jump && Input.GetButtonDown(jumpButton) && behaviourManager.IsCurrentBehaviour(behaviourCode) &&
			    !behaviourManager.IsOverriding())
			{
				jump = true;
			}
		}

		// LocalFixedUpdate overrides the virtual function of the base class.
		public override void LocalFixedUpdate()
		{
			// Call the basic movement manager.
			MovementManagement(behaviourManager.GetH, behaviourManager.GetV);

			// Call the jump manager.
			JumpManagement();
		}

		// Execute the idle and walk/run jump movements.
		private void JumpManagement()
		{
			// Start a new jump.
			if (jump && !behaviourManager.GetAnim.GetBool(jumpBool) && behaviourManager.IsGrounded())
			{
				// Set jump related parameters.
				behaviourManager.LockTempBehaviour(behaviourCode);
				behaviourManager.GetAnim.SetBool(jumpBool, true);
				// Is a locomotion jump?
				if (behaviourManager.GetAnim.GetFloat(speedFloat) > 0.1)
				{
					// Temporarily change player friction to pass through obstacles.
					GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
					GetComponent<CapsuleCollider>().material.staticFriction = 0f;
					// Set jump vertical impulse velocity.
					var velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
					velocity = Mathf.Sqrt(velocity);
					behaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
				}
			}
			// Is already jumping ?
			else if (behaviourManager.GetAnim.GetBool(jumpBool))
			{
				// Keep forward movement while in the air. Allow controlling movement in the air #tricks
				if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
				{
					behaviourManager.GetRigidBody.AddForce(
						transform.forward * jumpIntertialForce * Physics.gravity.magnitude * sprintSpeed,
						ForceMode.Acceleration);
				}

				// Has just landed ?
				if (behaviourManager.GetRigidBody.velocity.y < 0 && behaviourManager.IsGrounded())
				{
					behaviourManager.GetAnim.SetBool(groundedBool, true);
					// Change back player friction to default.
					GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f; // Hardcode ?
					GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
					// Set jump related parameters.
					jump = false;
					behaviourManager.GetAnim.SetBool(jumpBool, false);
					behaviourManager.UnlockTempBehaviour(behaviourCode);
				}
			}
		}

		// Deal with the basic player movement
		private void MovementManagement(float horizontal, float vertical)
		{
			// On ground, obey gravity.
			if (behaviourManager.IsGrounded())
				behaviourManager.GetRigidBody.useGravity = true;

			// Call function that deals with player orientation.
			Rotating(horizontal, vertical);

			// Set proper speed.
			var dir = new Vector2(horizontal, vertical);
			speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
			// This is for PC only, gamepads control speed via analog stick.
			speedSeeker += Input.GetAxis("Mouse ScrollWheel"); // Hardcode
			speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
			speed *= speedSeeker;
			if (behaviourManager.IsSprinting())
			{
				speed = sprintSpeed;
			}

			behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
		}

		// Rotate the player to match correct orientation, according to camera and key pressed.
		private Vector3 Rotating(float horizontal, float vertical)
		{
			// Get camera forward direction, without vertical component.
			var forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

			// Player is moving on ground, Y component of camera facing is not relevant.
			forward.y = 0.0f;
			forward = forward.normalized;

			// Calculate target direction based on camera forward and direction key.
			var right = new Vector3(forward.z, 0, -forward.x);
			var targetDirection = forward * vertical + right * horizontal;

			// Lerp current direction to calculated target direction.
			if (behaviourManager.IsMoving() && targetDirection != Vector3.zero)
			{
				var targetRotation = Quaternion.LookRotation(targetDirection);

				var newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation,
					behaviourManager.turnSmoothing);
				behaviourManager.GetRigidBody.MoveRotation(newRotation);
				behaviourManager.SetLastDirection(targetDirection);
			}

			// If idle, Ignore current camera facing and consider last moving direction.
			if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
			{
				behaviourManager.Repositioning();
			}

			return targetDirection;
		}

		// Collision detection.
		private void OnCollisionStay(Collision collision)
		{
			isColliding = true;
		}

		private void OnCollisionExit(Collision collision)
		{
			isColliding = false;
		}
	}
}