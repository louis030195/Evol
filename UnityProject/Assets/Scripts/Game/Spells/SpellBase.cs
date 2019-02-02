﻿using System;
using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using UnityEngine;

/// <summary>
/// What all spells have in common ?
/// </summary>
public class SpellBase : MonoBehaviour {

	protected float initializationTime;
	
	public Tuple<GameObject, Element> Caster { get; set; }

	protected virtual void Start()
	{
		initializationTime = Time.timeSinceLevelLoad;
	}
}
