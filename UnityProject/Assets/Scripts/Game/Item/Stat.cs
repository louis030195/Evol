using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Item
{
    
    [CreateAssetMenu(fileName = "New Stat", menuName = "Evol/Item/Equipment/Stat")]
    public class Stat : ScriptableObject
    {
        public string statName = "New Stat";
        public string description = "New Description";
    }
}