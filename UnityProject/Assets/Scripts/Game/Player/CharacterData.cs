using System.Collections;
using System.Collections.Generic;
using Evol.Game.Spell;
using UnityEngine;

namespace Evol.Game.Player
{
    [CreateAssetMenu(menuName = "Evol/Character")]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private int id;
        [SerializeField] private string characterName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;
        [SerializeField] private SpellObject[] spells;

        
        // Getters
        public int Id => id;

        public string CharacterName => characterName;

        public string Description => description;

        public Sprite Icon => icon;
        public GameObject Prefab => prefab;
        public SpellObject[] Spells => spells;

        // More stats ?
    }
}