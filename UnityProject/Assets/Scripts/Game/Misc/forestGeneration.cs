using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Game.Misc
{
	public class forestGeneration : MonoBehaviour
	{
		private List<GameObject> forestArea;
		public GameObject areaPrefab;

		[Tooltip("Maximum number of area on the map")]
		public int MaximumNumberOfArea = 5;

		[Tooltip("Origin where the areas start to propagate")]
		public Vector3 Origin;

		[Tooltip("Where to propagate")]
		public Vector3 Target;

		[Tooltip("Size of the area (exponential)(around 100 - 200 is nice)")]
		public int Size = 100;

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
				forestArea.Add(Instantiate(areaPrefab, Origin, Quaternion.identity));
				forestArea[forestArea.Count - 1].GetComponent<ForestArea>().Size = i * Size;
				forestArea[forestArea.Count - 1].GetComponent<ForestArea>().Target = Target;
				yield return new WaitForSeconds(10f);
				i++;
			}
		}

		

	}
}
