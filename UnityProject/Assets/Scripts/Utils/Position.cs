using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Utils
{
	public static class Position
	{
		/// <summary>
		///  Will adjust the position above ground relatively from the prefab size
		/// </summary>
		/// <param name="position"></param>
		/// <param name="prefabSize"></param>
		/// <returns></returns>
		public static Vector3 AboveGround(Vector3 position, float prefabSize)
		{
			RaycastHit hit;
			// Below ground
			if (Physics.Raycast(position, Vector3.up, out hit, Mathf.Infinity))
			{
				position.y += hit.distance + prefabSize * 0.5f;
			}

			hit = new RaycastHit();
			// Above ground
			if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
			{
				position.y -= hit.distance - prefabSize * 0.5f;
			}

			return position;
		}

	}
}