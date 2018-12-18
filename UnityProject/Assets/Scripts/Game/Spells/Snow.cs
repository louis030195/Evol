using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : SpellBase
{
	// We could imagine that the wizard start flying and some blocks of hard snow fall down, slowing everyone ...

	public GameObject SnowPrefab;
	
	// Use this for initialization
	private void Start () {
		base.Start();
		
		// For some reason the position is random ?
		transform.position = new Vector3(Caster.transform.position.x,
			Caster.transform.position.y + 0.1f,
			Caster.transform.position.z); 
		transform.Rotate(-90, 0, 0);


		for (int i = 0; i < Random.Range(5, 15); i++)
		{
			var randomPositionAround = transform.position;
			randomPositionAround.y += 0.5f;
			randomPositionAround.x += Random.Range(-5, 5);
			randomPositionAround.z += Random.Range(-5, 5);

			StartCoroutine(SlowlyDisappear(Instantiate(
				SnowPrefab.transform.GetChild(Random.Range(0, SnowPrefab.transform.childCount - 1)),
				randomPositionAround, Quaternion.identity).gameObject));
		}
	}


	IEnumerator SlowlyDisappear(GameObject go)
	{
		
		yield return new WaitForSeconds(5f);
		Destroy(go);
		
		
		//Destroy(go.GetComponent<Rigidbody>());
		//Vector3.Slerp(go.transform.position, Vector3.down * 10, Time.deltaTime * 10);

	}
	

	
	// Update is called once per frame
	private void Update () {
		/*
		if(Time.timeSinceLevelLoad - initializationTime > 1 && GetComponent<ParticleSystem>().particleCount == 0)
			Destroy(gameObject);*/
	}
}
