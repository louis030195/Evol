using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Ability;
using Evol.Game.Misc;
using Evol.Game.Player;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script works with AnimationCallback state machine (so it is required to be attached on the attack anims
/// This script is supposed to be attached on damage objects (weapons, fists ...) with collider with IsTrigger
/// </summary>
public class MeleeHit : Ability
{
    [HideInInspector] public UnityEvent onAbilityAnimationStart;
    [HideInInspector] public UnityEvent onAbilityAnimationEnd;

    private bool isAnimationPlaying;
    private bool hitOnce;

    private void Start()
    {
        caster = GetComponentInParent<Animator>().gameObject;
        
        // Getcomponentinparent because we usually put this script on hitting childs (fists, weapons ...)
        caster.GetComponent<Animator>().GetBehaviours<AnimationCallback>().ToList().ForEach(a => a.targets.Add(gameObject));
        onAbilityAnimationStart.AddListener(() => isAnimationPlaying = true);
        onAbilityAnimationEnd.AddListener(() => isAnimationPlaying = false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAnimationPlaying)
        {
            hitOnce = ApplyDamage(other.gameObject);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (isAnimationPlaying && !hitOnce) // Checking also if we already hit because OnTriggerStay proc multiple times during attack
        {
            hitOnce = ApplyDamage(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // TODO: think about this
        // hitOnce = false;
    }

    protected override void Initialize()
    {
    }

    protected override void TriggerAbility()
    {
    }

    protected override void UpdateAbility()
    {
    }

    protected override void StopAbility()
    {
    }
}
