using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Game.Ability;
using Photon.Pun;
using UnityEngine;

public class RechargeMana : Ability
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
		// Caster.Item1.GetComponent<Animator>().SetTrigger("Attack2Trigger");
        
		transform.parent = caster.transform;
		// For some reason the position is random ?
		transform.position = new Vector3(caster.transform.position.x,
			caster.transform.position.y + 1f,
			caster.transform.position.z); 
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
		var element = caster.GetComponent<CastBehaviour>().characterData.element;
		if (hitColliders.Any(c => (element == Element.Fire && c.CompareTag("FireSource")) 
		                          || (element == Element.Ice && c.CompareTag("IceSource"))))
		{
			if (Time.frameCount % 10 == 0)
			{
				///// This could win the Guinness world record for DIRTIEST CODE EVER
				stream.Add(Instantiate(PowerStream, hitColliders.First(c => (element == Element.Fire && c.CompareTag("FireSource")) 
					|| (element == Element.Ice && c.CompareTag("IceSource")))
					.transform.position, Quaternion.identity,
					transform.parent));
				stream.Last().AddComponent<Rigidbody>().useGravity = false;
				stream.Last().transform.LookAt(caster.transform);
				stream.Last().GetComponent<Rigidbody>().AddForce(transform.forward * 400, ForceMode.Acceleration);
				/////
				Destroy(stream.Last(), 5f);
				caster.gameObject.GetComponent<Mana>().RechargeMana(10);
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
		stream?.ForEach(Destroy);
	}
}
