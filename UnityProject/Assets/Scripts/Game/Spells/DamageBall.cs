using System.Collections;
using System.Collections.Generic;
using Evol.Agents;
using Evol.Game.Player;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Evol.Game.Spell
{
    public class DamageBall : SpellBase
    {
        private Vector3 moveDirection;
        private float speed = 2000;
        protected override void Start()
        {
            if (!gameObject.GetPhotonView().IsMine)
                return;
            base.Start();
            
            // Throw forward       
            var camera = Caster.GetComponentInChildren<Camera>();
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
            print($"Hit {other.gameObject.name}");
            
            // The hitbox is on the mesh which is sometimes on a child
            var health = other.gameObject.GetComponent<Health>() ?? other.transform.parent.gameObject.GetComponent<Health>();
            if(health != null)
                health.TakeDamage(100);
                
            DestroyAfter();
        }
    }
}