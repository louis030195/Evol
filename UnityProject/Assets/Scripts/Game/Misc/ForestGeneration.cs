using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Game.Misc
{
	public class ForestGeneration : MonoBehaviour
	{
		private List<GameObject> forestArea = new List<GameObject>();
		public GameObject areaPrefab;

		[Tooltip("Maximum number of area on the map")]
		public int MaximumNumberOfArea = 5;

		[Tooltip("Origin where the areas start to propagate")]
		public GameObject origin;

		[Tooltip("Where to propagate")]
		public GameObject target;

		[Tooltip("Size of the area (exponential)(around 100 - 200 is nice)")]
		public int size = 100;
		
		[Header("Forest area parameters")]
		[Tooltip("Space between trees")] 
		public int spacingBetweenTrees = 5;

		[Tooltip("Delay to spawn tree in seconds")]
		public float spawnTreeDelay = 1f;

		[Tooltip("Delay to check if the area is full in seconds")]
		public int checkFullDelay = 60;

		public GameObject[] prefabTree;

		// Use this for initialization
		private void Start () {
			StartCoroutine(SpawnAreas());
		}

		private IEnumerator SpawnAreas()
		{
			var i = 1;
			while (i <= MaximumNumberOfArea)
			{
				// Instanciate area
				forestArea.Add(Instantiate(areaPrefab));
				forestArea.Last().transform.localPosition = origin.transform.position;
				var forestAreaComponent = forestArea.Last().GetComponent<ForestArea>();
				forestAreaComponent.size = i * size;
				forestAreaComponent.target = target.transform.position;
				forestAreaComponent.spacingBetweenTrees = spacingBetweenTrees;
				forestAreaComponent.spawnTreeDelay = spawnTreeDelay;
				forestAreaComponent.checkFullDelay = checkFullDelay;
				forestAreaComponent.prefabTree = prefabTree;
				yield return new WaitForSeconds(10f);
				i++;
			}
		}

		

	}
}
