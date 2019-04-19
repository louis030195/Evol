using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public class MeleeHit : MonoBehaviour
{
    [HideInInspector] public UnityEvent onAbilityAnimationStart;
    [HideInInspector] public UnityEvent onAbilityAnimationEnd;

    private bool isAnimationPlaying;
    private string parentTag;

    private void Start()
    {
        // Getcomponentinparent because we usually put this script on hitting childs (fists, weapons ...)
        GetComponentInParent<Animator>().GetBehaviours<AnimationCallback>().ToList().ForEach(a => a.targets.Add(gameObject));
        parentTag = GetComponentInParent<Animator>().gameObject.tag; // Little hack to get the player / AI tag
        onAbilityAnimationStart.AddListener(() => isAnimationPlaying = true);
        onAbilityAnimationEnd.AddListener(() => isAnimationPlaying = false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // The hitbox is on the mesh which is sometimes on a child
        var parent = other.transform.parent; // Not all object have a parent
        var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
            parent ? parent.gameObject.GetComponent<Health>() : null;
        if (health != null && !other.CompareTag(parentTag) && isAnimationPlaying) // To avoid suiciding ? // TODO: check if animating
            health.TakeDamage(100, gameObject.GetComponentInParent<PhotonView>().Owner);
    }
}
