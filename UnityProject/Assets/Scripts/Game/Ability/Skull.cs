using System.Collections;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class Skull : MonoBehaviour
    {

        private void OnCollisionEnter(Collision other)
        {
            var parentAbility = GetComponentInParent<Ability>();
            parentAbility.ApplyDamage(other.gameObject);
            StartCoroutine(DestroyAfter(parentAbility.abilityData.stat.lifeLength));
        }

        private IEnumerator DestroyAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}