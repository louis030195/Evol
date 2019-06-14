using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

		[Tooltip("Size of the area")]
		public int size = 100;
		
		[Range(0.0f, 1.0f)]
		public float spacingBetweenArea = 0.1f;
		
		[Range(0.0f, 1.0f)]
		public float areaSizeGrowth = 0.1f;
		
		[Header("Forest area parameters")]
		[Tooltip("Space between trees (For example area size 100, spacing = 5, = 20 trees)")] 
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
				forestArea.Last().transform.LookAt(target.transform);
				var position = target.transform.position;
				forestArea.Last().transform.position = origin.transform.position - spacingBetweenArea * (position * (-i + 1));
				var forestAreaComponent = forestArea.Last().GetComponent<ForestArea>();
				forestAreaComponent.size = ((int)areaSizeGrowth * i + 1) * size;
				forestAreaComponent.target = position;
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
