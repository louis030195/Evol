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
		
		/// <summary>
		/// Return a random position around above ground
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Radius of the circle</param>
		/// <returns></returns>
		public static Vector3 RandomPositionAround (Vector3 center , float radius){
			var ang = Random.value * 360;
			Vector3 pos;
			pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
			pos.y = center.y;
			pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            
            
			return AboveGround(pos, 0);
		}

	}
}