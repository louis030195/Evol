using Evol.Utils;
using UnityEngine;

namespace Evol.Game.Ability
{
    [System.Serializable]
    public class Stat
    {
        public float manaCost;
        public float cooldown;
        public float lifeLength;
        public float scale;
        public float damage;
        public float heal;
        public float shield;

        public override string ToString()
        {
            return $"ManaCost: {manaCost}\nCooldown: {cooldown}\nLife length: {lifeLength}\nScale: {scale}\n";
        }
    }
}