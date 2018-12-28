using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Game.Spell;
using UnityEngine;

public class RechargeMana : SpellBase
{


	public GameObject PowerStream;

	private List<GameObject> stream;
	
	// Use this for initialization
	private void Start () 
	{
		stream = new List<GameObject>();
		Caster.GetComponent<Animator>().SetTrigger("Attack2Trigger");
            
		transform.parent = Caster.transform;
		// For some reason the position is random ?
		transform.position = new Vector3(Caster.transform.position.x,
			Caster.transform.position.y + 1f,
			Caster.transform.position.z); 
		Destroy(gameObject, 10f);
		

	}
	
	// Update is called once per frame
	private void Update () {
		// TODO: Balance this
		
		// If there is a power source close enough
		var hitColliders = Physics.OverlapSphere(transform.position, 10f);
		if (hitColliders.Any(collider => collider.GetComponentInChildren<BurningSteps>() != null))
		{



			if (Time.frameCount % 100 == 0)
			{
				///// This could win the Guinness world record for DIRTIEST CODE EVER
				stream.Add(Instantiate(PowerStream, hitColliders.First(collider
					=> collider.GetComponentInChildren<BurningSteps>() != null).transform.position, Quaternion.identity,
					transform.parent));
				stream.Last().AddComponent<Rigidbody>().useGravity = false;
				stream.Last().transform.LookAt(Caster.transform);
				stream.Last().GetComponent<Rigidbody>().AddForce(transform.forward * 100, ForceMode.Acceleration);
				/////
				Destroy(stream.Last(), 5f);
				Caster.gameObject.GetComponent<Mana>().RechargeMana(10);
			}
		}
		else
		{
			// Destroy the stream if we go out of range of the source
			stream.ForEach(Destroy);
		}
	}

	private void OnDestroy()
	{
		stream.ForEach(Destroy);
	}
}
