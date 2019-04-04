using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Item
{
    [CreateAssetMenu(fileName = "New Stat", menuName = "Evol/Item/Rune/Effect")]
    public class Effect : ScriptableObject
    {
        // An effect is like "Propagation" which makes the ability propagates to mobs
        public string effectName = "New Effect";
        public string description = "New Description";
    }
}