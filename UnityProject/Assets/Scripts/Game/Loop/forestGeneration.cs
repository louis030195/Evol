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

		[Tooltip("Origin where the areas start to propagate")]
		public Vector3 Origin;

		[Tooltip("Where to propagate")]
		public Vector3 Target;

		// Use this for initialization
		private void Start () {
			forestArea = new List<GameObject>();
			StartCoroutine(SpawnAreas());
			
		}

		private IEnumerator SpawnAreas()
		{
			var i = 1;
			var targetDir = Target - Origin;
			var angle = Vector3.Angle(targetDir, Origin);
			print(angle);
			while (i < MaximumNumberOfArea)
			{
				// Instanciate area
				forestArea.Add(Instantiate(areaPrefab, Origin, Quaternion.identity/*new Quaternion(0, angle, 0, 0)*/));
				forestArea[forestArea.Count - 1].GetComponent<ForestArea>().Size = i * 100;
				print(forestArea[forestArea.Count - 1].GetComponent<ForestArea>().Size);
				yield return new WaitForSeconds(10f);
				i++;
			}
		}

		

	}
}
