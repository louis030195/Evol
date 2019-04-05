using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Ability
{
	[CreateAssetMenu(menuName = "Evol/Spell")]
	public class AbilityData : ScriptableObject
	{
		public new string name;
		public string description;
		public AudioClip[] sound;
		public string animation;
		public Sprite icon;
		public float cooldown;
		public int manaCost;
		public GameObject prefab;

		//public abstract void Initialize(GameObject obj);
		//public abstract void TriggerAbility();
	}
}