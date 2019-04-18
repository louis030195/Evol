using System.Linq;
using Evol.Game.Item;
using Evol.Heuristic.StateMachine;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public abstract class Summon : Ability
    {
        public GameObject effect;
        public GameObject summon;
        protected GameObject instance;
        
        protected abstract override void Initialize();

        protected abstract override void TriggerAbility();

        protected abstract override void UpdateAbility();

        protected abstract override void StopAbility();

        protected void SummonNow(Vector3 positionToSpawn)
        {
            // Has duplicate rune ? Will proc again for each duplicate rune
            foreach (var i in Enumerable.Range(1, runes.Count(r => r.effect == RuneEffect.Duplicate) + 1))
            {
                instance = PhotonNetwork.Instantiate(summon.name, positionToSpawn, Quaternion.identity);
                // Has empower rune ?
                instance.transform.localScale *= (1 + runes.Count(r => r.effect == RuneEffect.Empower));
                // Required to set as a "network" child of the caster or it will be destroyed as a net child of the flask
                instance.GetPhotonView().TransferOwnership(caster.GetPhotonView().Owner);
                instance.GetComponent<StateController>().SetupAi(true);
            }
        }
    }
}