using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;

namespace Evol.Game.Misc
{



	public class ForestArea : MonoBehaviour
	{
		[HideInInspector] public int spacingBetweenTrees = 5;
		[HideInInspector] public float spawnTreeDelay = 1f;
		[HideInInspector] public int checkFullDelay = 60;
		[HideInInspector] public GameObject[] prefabTree;
		[HideInInspector] public int size = 100;
		[HideInInspector] public Vector3 target;

		private int spawnedTrees;
		
		private void Start()
		{
			checkFullDelay *= 60;
			StartCoroutine(SpawnTree()); // Starting once at start
		}

		// Update is called once per frame
		private void Update()
		{
			if (Time.frameCount % checkFullDelay == 0)
			{
				// print($"spawnedTrees { spawnedTrees }");
				spawnedTrees = transform.childCount;
				StartCoroutine(SpawnTree()); // To fill the destroyed trees
			}
		}


		private IEnumerator SpawnTree()
		{
			while (spawnedTrees < size / spacingBetweenTrees) // For example area size 100, spacing = 5, = 20 trees
			{
				yield return new WaitForSeconds(spawnTreeDelay);
				var prefab = prefabTree.PickRandom();
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
				position = Position.AboveGround(transform.position - new Vector3(Random.Range(-size, size), 0,
					                                0.75f * Random.Range(-size, size)), 0, layerMask: 1 << LayerMask.NameToLayer("Walkable"));

				// Then we throw an overlap sphere
				var hitColliders = Physics.OverlapSphere(position, spacingBetweenTrees, 1 << LayerMask.NameToLayer("Walkable"));
				// UnityEngine.Debug.DrawRay(position, transform.up * 10, Color.green);

				// Which checks if there is already a tree around
				if (!hitColliders.Any(c => c.CompareTag("Tree")))
				{
					return position;
				}

				tries++;
			}

			return Vector3.zero;
		}
	}
}