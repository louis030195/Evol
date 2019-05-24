using System.Collections;
using System.Linq;
using Evol.Game.Player;
using Evol.Heuristic;
using Evol.ML;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Misc
{
    public class Tree : MonoBehaviour
    {
        [Tooltip("Tags considered as allies")] public string[] allies;
        [Tooltip("Tags considered as enemies")] public string[] enemies;
        [Tooltip("Maximum size the tree can grow")] public float MaxSize = 1;
        [Tooltip("At which rate the tree is growing")] public float GrowRate = 0.015f;
        [Tooltip("")] public float MinRandomness = 0.5f;
        [Tooltip("")] public float MaxRandomness = 1.5f;
        
        private float maxSize;
        private float growRate;
        private float scale;
        private bool aboveGround;

        private Attack attack;
        
        public void Awake()
        {
            attack = GetComponent<Attack>();
            attack.alliesTag = allies.ToList();
            attack.enemiesTag = enemies.ToList();
            maxSize = Random.Range(MaxSize * MinRandomness, MaxSize * MaxRandomness); // Add a bit of randomness
            growRate = Random.Range(GrowRate * MinRandomness, GrowRate * MaxRandomness);
            Grow(); // Set the initial size directly to avoid seeing the transition to a lower size ...
        }

        public void Update()
        {
            Grow();

            if (!aboveGround) // TODO: ???
            {
                transform.position = Position.AboveGround(transform.position, GetComponent<Collider>().bounds.size.y);
                aboveGround = true;
            }
        }

        private void Grow()
        {
            if (scale < maxSize)
            {
                transform.localScale = Vector3.one * scale;
                scale += growRate * Time.deltaTime;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // TODO: spawn the spell from the top of the tree or something like that (compare with renderer stuff)
            // var pos = new Vector3(other.transform.position.x, 20, other.transform.position.z);
            if(enemies.Any(other.CompareTag)) attack.AttackNow(other.transform.position);
        }
    }
}