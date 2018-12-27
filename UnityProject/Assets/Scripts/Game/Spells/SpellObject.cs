using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Spell
{
	[CreateAssetMenu(menuName = "Evol/Spell")]
	public class SpellObject : ScriptableObject
	{
		public GameObject SpellPrefab;
		public float Cooldown;
		public int ManaCost;
	}
}