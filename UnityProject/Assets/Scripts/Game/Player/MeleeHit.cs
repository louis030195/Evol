using System.Collections;
using System.Collections.Generic;
using Evol.Game.Misc;
using Evol.Game.Player;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class MeleeHit : MonoBehaviour
{
    private bool isAnimationPlaying;

    private void Start()
    {
        EventManager.StartListening("OnAnimationStart", arg0 =>
        {
            isAnimationPlaying = true;
        });
        EventManager.StartListening("OnAnimationEnd", arg0 =>
        {
            isAnimationPlaying = false;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        // The hitbox is on the mesh which is sometimes on a child
        var parent = other.transform.parent; // Not all object have a parent
        var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
            parent ? parent.gameObject.GetComponent<Health>() : null;
        if (health != null && !other.CompareTag("Player") && isAnimationPlaying) // To avoid suiciding ? // TODO: check if animating
            health.TakeDamage(100, gameObject.GetComponentInParent<PhotonView>().Owner);
    }
}
