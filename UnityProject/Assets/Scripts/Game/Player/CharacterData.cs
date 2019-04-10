using System.Collections;
using System.Collections.Generic;
using Evol.Game.Ability;
using UnityEngine;

namespace Evol.Game.Player
{
    public enum Element
    {
        Fire,
        Ice
    }
    
    [CreateAssetMenu(menuName = "Evol/Character")]
    public class CharacterData : ScriptableObject
    {
        public int id;
        public string characterName;
        public string description;
        public bool ranged;
        public Element element;
        public Sprite icon;
        public GameObject placeholder;
        public GameObject[] abilities;
    }
}