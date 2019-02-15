using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ForestArea : MonoBehaviour
{
	[Tooltip("Space between trees")] public int SpacingBetweenTrees = 500;
	[Tooltip("Delay to spawn tree in seconds")] public float SpawnTreeDelay = 1f;
	[Tooltip("Delay to check if the area is full in seconds")] public int CheckFullDelay = 60;
	public List<GameObject> PrefabTree;
	[HideInInspector] public bool Full;
	[HideInInspector] public int Size = 100;

	private void Start()
	{
		CheckFullDelay *= 60;
	}

	// Update is called once per frame
	private void Update () {
		if (Time.frameCount % CheckFullDelay == 0)
		{
			StartCoroutine(SpawnTree());
		}
	}
	
	
	private IEnumerator SpawnTree()
	{
		while (!Full)
		{
			yield return new WaitForSeconds(SpawnTreeDelay);
			var prefab = PrefabTree[Random.Range(0, PrefabTree.Count - 1)];
			var pos = FindPosition(prefab.GetComponent<MeshRenderer>().bounds.size.y);
			if (pos != Vector3.zero)
			{
				
				Instantiate(prefab, pos,
					new Quaternion(0, Random.Range(0, 360), 0, Random.Range(0, 360)), transform);
			}
		}
	}

	private Vector3 FindPosition(float prefabSize)
	{
		var position = Vector3.zero;
		var tries = 0;
		while (tries < 10)
		{
			position = new Vector3(Random.Range(-100, Size), 5,
				Random.Range(-100, Size));
			var hitColliders = Physics.OverlapSphere(transform.position + position, SpacingBetweenTrees);
			if (!hitColliders.Any(c => c.gameObject.name.Contains("tree")))
			{
				return AboveGround(transform.position + position, prefabSize);
			}

			tries++;
		}
		return Vector3.zero;
	}

	private Vector3 AboveGround(Vector3 position, float prefabSize)
	{
		RaycastHit hit;
		if (Physics.Raycast(position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
		{
			position.y += hit.distance + prefabSize / 2;
		}

		hit = new RaycastHit();
		if (Physics.Raycast(position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
		{
			position.y -= hit.distance - prefabSize / 2;
		}

		return position;
	}
}
