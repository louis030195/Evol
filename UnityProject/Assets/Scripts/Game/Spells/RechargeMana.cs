using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Spell;
using UnityEngine;

public class RechargeMana : SpellBase
{


	public GameObject PowerStream;

	private GameObject stream;
	
	// Use this for initialization
	private void Start () 
	{
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
		
		var hitColliders = Physics.OverlapSphere(transform.position, 10f);
		if (hitColliders.Any(collider => collider.GetComponentInChildren<BurningSteps>() != null))
		{
			// Instanciate the power stream between the recharge spell and the source of power
			if(transform.Find(PowerStream.name + "(Clone)") == null)
				stream = Instantiate(PowerStream, transform);
			
			var particles = new ParticleSystem.Particle[stream.GetComponent<ParticleSystem>().particleCount];
			
			for (var t = 0f; t < 1f; t += 0.1f) {
				var count = stream.GetComponent<ParticleSystem>().GetParticles(particles);
				for (var i=0; i < count; i++) {
					particles[i].position = Vector3.Lerp(particles[i].position, hitColliders.First(collider =>
						collider.GetComponentInChildren<BurningSteps>() != null).transform.position, t);
				}
				stream.GetComponent<ParticleSystem>().SetParticles(particles, count);
			}
			
			if (Time.frameCount % 100 == 0)
				Caster.gameObject.GetComponent<PlayerController>().CurrentMana += 10;
		}
	}
}
