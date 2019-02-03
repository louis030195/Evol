using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Game.Spell;
using Photon.Pun;
using UnityEngine;

public class RechargeMana : SpellBase
{


	public GameObject PowerStream;

	private List<GameObject> stream;
	
	// Use this for initialization
	protected override void Start () 
	{
		if (!gameObject.GetPhotonView().IsMine)
			return;
		base.Start();
		stream = new List<GameObject>();
		Caster.Item1.GetComponent<Animator>().SetTrigger("Attack2Trigger");
        
		transform.parent = Caster.Item1.transform;
		// For some reason the position is random ?
		transform.position = new Vector3(Caster.Item1.transform.position.x,
			Caster.Item1.transform.position.y + 1f,
			Caster.Item1.transform.position.z); 
		Destroy(gameObject, 10f);
		

	}
	
	// Update is called once per frame
	private void Update () {
		// TODO: Balance this
		// print("ok");
		// If there is a power source close enough
		if (!gameObject.GetPhotonView().IsMine)
			return;
		var hitColliders = Physics.OverlapSphere(transform.position, 10f);
		if (hitColliders.Any(c => (Caster.Item2 == Element.Fire && c.CompareTag("FireSource")) 
		                          || (Caster.Item2 == Element.Ice && c.CompareTag("IceSource"))))
		{
			// print("ok");
			if (Time.frameCount % 10 == 0)
			{
				///// This could win the Guinness world record for DIRTIEST CODE EVER
				stream.Add(Instantiate(PowerStream, hitColliders.First(c => (Caster.Item2 == Element.Fire && c.CompareTag("FireSource")) 
					|| (Caster.Item2 == Element.Ice && c.CompareTag("IceSource")))
					.transform.position, Quaternion.identity,
					transform.parent));
				stream.Last().AddComponent<Rigidbody>().useGravity = false;
				stream.Last().transform.LookAt(Caster.Item1.transform);
				stream.Last().GetComponent<Rigidbody>().AddForce(transform.forward * 400, ForceMode.Acceleration);
				/////
				Destroy(stream.Last(), 5f);
				Caster.Item1.gameObject.GetComponent<Mana>().RechargeMana(10);
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
