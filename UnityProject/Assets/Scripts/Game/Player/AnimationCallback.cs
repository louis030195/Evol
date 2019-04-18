using System.Collections;
using System.Collections.Generic;
using Evol.Game.Misc;
using UnityEngine;

namespace Evol.Game.Player
{
    /// <summary>
    /// https://unity3d.com/learn/tutorials/modules/beginner/5-pre-order-beta/state-machine-behaviours
    /// Useful to throw event for animations
    /// </summary>
    public class AnimationCallback : StateMachineBehaviour
    {
    
        // This will be called when the animator first transitions to this state.
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EventManager.TriggerEvent("OnAbilityAnimationStart", null);
        }
    
    
        // This will be called once the animator has transitioned out of the state.
        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EventManager.TriggerEvent("OnAbilityAnimationEnd", null);
        }
    }
}