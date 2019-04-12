using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class SummonSkeleton : Summon
    {
        protected override void Initialize()
        {
        }

        protected override void TriggerAbility()
        {
            // Throw forward       
            var camera = caster.GetComponentInChildren<Camera>();
            var pos = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
            transform.LookAt(pos); 
            GetComponent<Rigidbody>().AddForce(-transform.forward * 1000);
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
            PhotonNetwork.Destroy(instance);
        }

        private bool collided = false; // tmp to remove after resolving the todo
        private void OnCollisionEnter(Collision other)
        {
            if (collided) return; // tmp to remove after resolving the todo
            collided = true; // tmp to remove after resolving the todo
            var position = transform.position;
            PhotonNetwork.Instantiate(effect.name, position, new Quaternion(90, 0, 0, 90));
            // TODO: instanciate as child view photonvie of caster or it dies after flakon death
            SummonNow(position); // TODO: async await this summon (drop coroutine shit) cuz we have to wait that it spawn and then destroy the flask
            StartCoroutine(DestroyAfter(2));
            // PhotonNetwork.Destroy(gameObject);
        }
    }
}