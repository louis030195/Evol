using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Item
{
    [Serializable]
    public class EffectDegree
    {
        public Effect effect;
        public int degree;// We can think of degree like a level of effect
    }
    
    /// <summary>
    /// A rune can be equipped on an ability to improve it
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Evol/Item/Rune")]
    public class Rune : ItemData
    {
        // A rune has a list of effects that will be applied to the ability
        // It is possible to think that a rune can bring positive effects and negative effects
        // For example double the radius of the spell but reduce it's damage
        public EffectDegree[] effects;
    }
}