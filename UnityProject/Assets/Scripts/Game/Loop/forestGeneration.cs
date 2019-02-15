using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Game.Loop
{
	public class forestGeneration : MonoBehaviour
	{
		private List<GameObject> forestArea;
		public GameObject areaPrefab;

		[Tooltip("Maximum number of area on the map")]
		public int MaximumNumberOfArea = 5;

		// Use this for initialization
		private void Start () {
			forestArea = new List<GameObject>();
			StartCoroutine(SpawnAreas());
			
		}

		private IEnumerator SpawnAreas()
		{
			var i = 1;
			while (i < MaximumNumberOfArea)
			{
				// Instanciate area
				forestArea.Add(Instantiate(areaPrefab));
				forestArea[forestArea.Count - 1].GetComponent<ForestArea>().Size = i * 100;
				print(forestArea[forestArea.Count - 1].GetComponent<ForestArea>().Size);
				yield return new WaitForSeconds(10f);
				i++;
			}
		}

		

	}
}
