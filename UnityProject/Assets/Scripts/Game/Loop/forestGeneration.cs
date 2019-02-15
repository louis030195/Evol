using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Loop
{
	public class forestGeneration : MonoBehaviour
	{
		private List<GameObject> forestArea;
		public GameObject areaPrefab;

		// Use this for initialization
		private void Start () {
			forestArea = new List<GameObject>();
			StartCoroutine(SpawnAreas());
			
		}

		private IEnumerator SpawnAreas()
		{
			var origin = 1;
			while (true)
			{
				// Instanciate area
				forestArea.Add(Instantiate(areaPrefab, new Vector3(origin * 100, 0, 0), Quaternion.identity));
				yield return new WaitForSeconds(10f);
				origin++;
			}
		}

		

	}
}
