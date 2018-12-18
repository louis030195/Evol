using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// What all spells have in common ?
/// </summary>
public class SpellBase : MonoBehaviour {

	protected float initializationTime;
	
	public GameObject Caster { get; set; }

	protected virtual void Start()
	{
		initializationTime = Time.timeSinceLevelLoad;
	}
}
