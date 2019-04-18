using System.Collections;
using System.Linq;
using Evol.Game.Item;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class SkullRain : Ability
    {
        public GameObject skullPrefab;
        public int maxWaves = 3;
        public int maxSkullPerWave = 5;

        private int _maxWaves; // Fucking Unity serialization !!
        private int _maxSkullPerWave;
        
        protected override void Initialize()
        {
            StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength*2));
            
            // Has empower rune ?
            _maxWaves = maxSkullPerWave + maxSkullPerWave * runes.Count(r => r.effect == RuneEffect.Empower);
            
            // Has duplicate rune ?
             _maxSkullPerWave = maxWaves + (maxWaves * runes.Count(r => r.effect == RuneEffect.Duplicate));
        }

        protected override void TriggerAbility()
        {
            transform.localScale *= abilityData.stat.scale;
            
            // Cast forward
            var pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, pos.z + 5);
            StartCoroutine(SpawnSkullWaves());
        }

        private IEnumerator SpawnSkullWaves()
        {
            foreach (var i in Enumerable.Range(0, _maxWaves)) // Will spawn waves of kind of circle of skulls that will fall
            {
                foreach (var j in Enumerable.Range(0, _maxSkullPerWave))
                {
                    var go = PhotonNetwork.Instantiate(skullPrefab.name, 
                        Vector3.zero, // Assuming it's size around 1
                        Quaternion.identity);
                    go.transform.SetParent(transform);
                    go.transform.localPosition = new Vector3(j, 10, j); // TODO: make spawn in circle (full)
                }

                yield return new WaitForSeconds(1.5f);
            }
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
        }
    }
}