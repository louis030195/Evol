﻿using System.Collections;
using System.Collections.Generic;
using Evol.Agents;
using Evol.Game.Player;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class DamageBall : Ability
    {
        private Vector3 moveDirection;
        private float speed = 2000;
        protected override void Start()
        {
            if (!gameObject.GetPhotonView().IsMine)
                return;
            base.Start();
            
            // Throw forward       
            var camera = caster.GetComponentInChildren<Camera>();
            var pos = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.nearClipPlane));
            transform.LookAt(pos); 
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * speed);

            // Destroy the bullet after 5 seconds
            Invoke(nameof(DestroyAfter), 5);
        }

        private void DestroyAfter()
        {
            PhotonNetwork.Destroy(gameObject.GetPhotonView());
        }


        private void OnTriggerEnter(Collider other)
        {
            // The hitbox is on the mesh which is sometimes on a child
            var parent = other.transform.parent; // Not all object have a parent
            var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
                parent ? parent.gameObject.GetComponent<Health>() : null;
            if(health != null)
                health.TakeDamage(100);
                
            DestroyAfter();
        }
    }
}