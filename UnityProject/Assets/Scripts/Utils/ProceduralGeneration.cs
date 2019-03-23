using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace Evol.Utils
{
	public class ProceduralGeneration : MonoBehaviour
	{
		// Could be useful for more advanced shapes (3D ...)
		// http://wiki.unity3d.com/index.php/ProceduralPrimitives
		public enum Shape{ Rectangle, Triangle, Circle }
		[Tooltip("Shape of the generated thing")] public Shape GeneratedShape;
		[Tooltip("Object to spawn in the shape")] public GameObject ObjectToSpawn;
		[Tooltip("Size of the generated thing (side / radius)")] public int GeneratedSize;

		private void Start()
		{
			// Good old anti-pattern if, TODO: use dict => func or delegate or something else cleaner ...
			if (GeneratedShape == Shape.Rectangle)
			{
				var objectWidth =
					ObjectToSpawn.GetComponent<Renderer>().bounds.size.x; // Maybe depending on the object could be z, think about it
				var objectHeight = ObjectToSpawn.GetComponent<Renderer>().bounds.size.y;
				var rotation1 = new Quaternion(0, 0, 0, 0);
				var rotation2 = new Quaternion(0, 90, 0, 90);

				foreach (var i in Enumerable.Range(0, GeneratedSize))
				{
					Instantiate(ObjectToSpawn, Position.AboveGround(
							new Vector3(objectWidth * i, 0, 0),
							objectHeight, transform:transform), rotation1,
						transform);
				}

				foreach (var i in Enumerable.Range(0, GeneratedSize))
				{
					// Note the i + 1 (for side 2 and 4) which is here to fix the rotation, without it it looks like that
					//  _______
					//        
					// |       |
					// |       |
					// |_______|
					// |       |
					//
					// And with the i + 1
					//  _______
					// |       |
					// |       |
					// |       |
					// |_______|
					//        

					Instantiate(ObjectToSpawn, Position.AboveGround(
							new Vector3(0, 0, objectWidth * (i + 1)),
							objectHeight, transform:transform), rotation2,
						transform);
				}

				foreach (var i in Enumerable.Range(0, GeneratedSize))
				{
					Instantiate(ObjectToSpawn, Position.AboveGround(
							new Vector3(objectWidth * i, 0, objectWidth * GeneratedSize),
							objectHeight, transform:transform), rotation1,
						transform);
				}

				foreach (var i in Enumerable.Range(0, GeneratedSize))
				{
					Instantiate(ObjectToSpawn, Position.AboveGround(
							new Vector3(objectWidth * GeneratedSize, 0, objectWidth * (i + 1)),
							objectHeight, transform:transform), rotation2,
						transform);
				}
			}
		}
	}
}