using System.Collections;
using System.Collections.Generic;
using Evol.Game.Spell;
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
        [SerializeField] private int id;
        [SerializeField] private string characterName;
        [SerializeField] private string description;
        [SerializeField] private bool ranged;
        [SerializeField] private Element element;
        [SerializeField] private Sprite icon;
        [SerializeField] private SpellObject[] spells;
        [SerializeField] private GameObject placeholder;
        
        // Getters
        public int Id => id;

        public string CharacterName => characterName;

        public string Description => description;
        public Element Element => element;

        public Sprite Icon => icon;
        public SpellObject[] Spells => spells;
        public bool Ranged => ranged;
        public GameObject Placeholder => placeholder;

        // More stats ?
    }
}