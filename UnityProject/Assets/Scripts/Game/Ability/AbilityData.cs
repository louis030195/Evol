using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Game.Ability;
using Evol.Game.Item;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
	[CreateAssetMenu(menuName = "Evol/Ability")]
	public class AbilityData : ScriptableObject
	{
		public string abilityName;
		public string description;
		public Sprite icon;
		public Stat stat;

		public GameObject prefab; 
		// It is redundant to add a reference to the prefab but we need it in order to odo duplication etc (handle runes)
		// I don't see a cleaner way atm
	}
}