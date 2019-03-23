using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Utils;
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
	[HideInInspector] public Vector3 Target;

	private void Start()
	{
		CheckFullDelay *= 60;
		StartCoroutine(SpawnTree()); // Starting once at start
	}

	// Update is called once per frame
	private void Update () {
		if (Time.frameCount % CheckFullDelay == 0)
		{
			StartCoroutine(SpawnTree()); // To fill the destroyed trees
		}
	}
	
	
	private IEnumerator SpawnTree()
	{
		while (!Full)
		{
			yield return new WaitForSeconds(SpawnTreeDelay);
			var prefab = PrefabTree[Random.Range(0, PrefabTree.Count - 1)];
			var pos = FindPosition();
			if (pos != Vector3.zero)
			{
				
				Instantiate(prefab, pos,
					new Quaternion(0, Random.Range(0, 360), 0, Random.Range(0, 360)), transform);
			}
		}
	}

	/// <summary>
	/// This function will find a position to spawn a tree above ground and far enough from other trees
	/// TODO: find a better function name
	/// </summary>
	/// <param name="prefabSize"></param>
	/// <returns></returns>
	private Vector3 FindPosition()
	{
		var position = Vector3.zero;
		var tries = 0;
		
		// While we didn't find a suitable position
		while (tries < 10)
		{
			// We pick a random position above ground
			position = Position.AboveGround(transform.position + Target - new Vector3(Random.Range(-100, Size), 0,
				Random.Range(-100, Size)), 0);
			
			// Then we throw an overlap sphere
			var hitColliders = Physics.OverlapSphere(transform.position + position, SpacingBetweenTrees);
			
			// Which checks if there is already a tree around
			if (!hitColliders.Any(c => c.gameObject.name.Contains("tree")))
			{
				return position;
			}

			tries++;
		}
		return Vector3.zero;
	}
}
