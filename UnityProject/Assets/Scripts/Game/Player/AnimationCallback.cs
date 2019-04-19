using System.Collections;
using System.Collections.Generic;
using Evol.Game.Misc;
using Evol.Heuristic;
using Evol.Utils;
using UnityEngine;
using UnityEngine.Events;
using Debug = System.Diagnostics.Debug;

namespace Evol.Game.Player
{
    /// <summary>
    /// https://unity3d.com/learn/tutorials/modules/beginner/5-pre-order-beta/state-machine-behaviours
    /// Useful to throw event for animations
    /// </summary>
    public class AnimationCallback : StateMachineBehaviour
    {
        [HideInInspector] public List<GameObject> targets = new List<GameObject>();

        private void Awake()
        {
            // MonoBehaviourFromScriptableObject.instance.StartCoroutine(WaitUntilTargetIsSet());
        }

        /*
        private IEnumerator WaitUntilTargetIsSet()
        {
            yield return new WaitUntil(() => targets.Count > 0);
            // Melee == meleehit or ranged castbehaviour
            foreach (var target in targets)
            {
                var melee = target.GetComponent<MeleeHit>();
                var player = target.GetComponent<CastBehaviour>();
                if (melee)
                {
                    melee.onAbilityAnimationStart.AddListener(melee.CallAbilityAnimationStart);
                    melee.onAbilityAnimationEnd.AddListener(melee.CallAbilityAnimationEnd);
                }
                else
                {
                    player.onAbilityAnimationStart.AddListener(player.CallAbilityAnimationStart);
                    player.onAbilityAnimationEnd.AddListener(player.CallAbilityAnimationEnd);
                }
            }
        }*/



        // This will be called when the animator first transitions to this state.
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // EventManager.TriggerEvent("OnAbilityAnimationStart", null);
            // Melee == meleehit or ranged castbehaviour
            foreach (var target in targets)
            {
                var melee = target.GetComponent<MeleeHit>();
                var player = target.GetComponent<CastBehaviour>();
                if (melee)
                {
                    melee.onAbilityAnimationStart.Invoke();
                }
                else
                {
                    player.onAbilityAnimationStart.Invoke();
                }
            }
        }
    
    
        // This will be called once the animator has transitioned out of the state.
        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // EventManager.TriggerEvent("OnAbilityAnimationEnd", null);
            // Melee == meleehit or ranged castbehaviour
            foreach (var target in targets)
            {
                var melee = target.GetComponent<MeleeHit>();
                var player = target.GetComponent<CastBehaviour>();
                if (melee)
                {
                    melee.onAbilityAnimationEnd.Invoke();
                }
                else
                {
                    player.onAbilityAnimationEnd.Invoke();
                }
            }
        }
    }
}