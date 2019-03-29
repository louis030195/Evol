using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Evol.Utils
{
	public static class Position
	{
		/// <summary>
		/// Will adjust the position above ground relatively from the prefab size
		/// Global position
		/// </summary>
		/// <param name="position"></param>
		/// <param name="prefabHeight">Prefab height needed in order to place well on top of ground</param>
		/// <param name="flyFix">Little tweak to fix flying object</param>
		/// <param name="transform">Transform parent</param>
		/// <returns></returns>
		public static Vector3 AboveGround(Vector3 position, float prefabHeight, float flyFix = 0.8f, Transform transform = null)
		{
			if (transform)
				position += transform.position;

			var below = false;
			
			// Below ground
			if (Physics.Raycast(position, Vector3.up, out var hit, Mathf.Infinity))
			{
				// Debug.WriteLine($"aboveground { position.y } + { (hit.distance + prefabHeight * 0.5f) * flyFix } - { hit.distance }");
				position.y += hit.distance + prefabHeight * 0.5f;
				below = true;
			}

			if (!below) // No need to raycast again
			{
				// Above ground
				if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
				{
					position.y -= hit.distance - prefabHeight * 0.5f;
				}
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


			return pos;
		}

	}
}