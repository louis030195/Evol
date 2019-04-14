using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Player
{
    /// <summary>
    /// https://unity3d.com/learn/tutorials/modules/beginner/5-pre-order-beta/state-machine-behaviours
    /// </summary>
    public class BasicStateMachineBehaviour : StateMachineBehaviour
    {
        [HideInInspector] public bool isAnimationPlaying;
        public GameObject particles;            // Prefab of the particle system to play in the state.
        public AvatarIKGoal attackLimb;         // The limb that the particles should follow.
        [HideInInspector] public MonoBehaviour exampleMb;
    
        private Transform particlesTransform;       // Reference to the instantiated prefab's transform.
        private ParticleSystem particleSystem;      // Reference to the instantiated prefab's particle system.
    
    
        // This will be called when the animator first transitions to this state.
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If the particle system already exists then exit the function.
            if(particlesTransform != null)
                return;

            isAnimationPlaying = true;
            
            // Otherwise instantiate the particles and set up references to their components.
            GameObject particlesInstance = Instantiate(particles);
            particlesTransform = particlesInstance.transform;
            particleSystem = particlesInstance.GetComponent <ParticleSystem> ();
        }
    
    
        // This will be called once the animator has transitioned out of the state.
        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // When leaving the special move state, stop the particles.
            particleSystem.Stop();

            isAnimationPlaying = false;
        }
    
    
        // This will be called every frame whilst in the state.
        public override void OnStateIK (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // OnStateExit may be called before the last OnStateIK so we need to check the particles haven't been destroyed.
            if (particleSystem == null || particlesTransform == null)
                return;
            
            // Find the position and rotation of the limb the particles should follow.
            Vector3 limbPosition = animator.GetIKPosition(attackLimb);
            Quaternion limbRotation = animator.GetIKRotation (attackLimb);
            
            // Set the particle's position and rotation based on that limb.
            particlesTransform.position = limbPosition;
            particlesTransform.rotation = limbRotation;
    
            // If the particle system isn't playing, play it.
            if(!particleSystem.isPlaying)
                particleSystem.Play();
        }
    }
}