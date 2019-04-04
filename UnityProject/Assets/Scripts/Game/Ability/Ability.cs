using System;
using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Evol.Game.Ability;
using Evol.Game.Item;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
	public abstract class Ability : MonoBehaviour
	{

		protected float initializationTime;
		protected Rune[] runes;
		[HideInInspector] public GameObject caster;

		protected virtual void Start()
		{
			initializationTime = Time.timeSinceLevelLoad;
		}
	}
}