using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForestArea : MonoBehaviour
{
	public int SpacingBetweenTrees = 5;
	public List<GameObject> prefabTree;
	[HideInInspector] public bool Full;
	
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 100 == 0)
		{
			StartCoroutine(SpawnTree());
		}
	}
	
	
	private IEnumerator SpawnTree()
	{
		while (!Full)
		{
			yield return new WaitForSeconds(0.1f);

			var pos = FindPosition();
			if(pos != Vector3.zero)
				Instantiate(prefabTree[Random.Range(0, prefabTree.Count - 1)], transform.position + pos, Quaternion.identity, transform);
		}
	}

	private Vector3 FindPosition()
	{
		var position = Vector3.zero;
		var tries = 0;
		while (tries < 100)
		{
			position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
			var hitColliders = Physics.OverlapSphere(position, SpacingBetweenTrees);
			if (!hitColliders.Any(c => c.gameObject.name.Contains("tree")))
			{
				return position;
			}

			tries++;
		}

		return Vector3.zero;
	}
}
