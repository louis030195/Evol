using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Evol.Utils
{
	public class RandomTerrain : MonoBehaviour
	{


		[Tooltip("Frequency of terrain randomization follow formula. \nUpdate every 10^UpdateRate frames")]
		public int UpdateRate = 8;
		
		public int Depth = 10;
		public int Width = 100;
		public int Height = 100;
		public float Scale = 20f;
		public float OffsetX = 100f;
		public float OffsetY = 100f;

		public int NumberOfBumps = 20;
		public int NeighboringBumps = 10;
		

		private int frames = 0;

		private void FixedUpdate()
		{
			frames++;
			if (frames % Mathf.Pow(10, UpdateRate) == 0)
			{
				frames = 0;
				var terrain = GetComponent<Terrain>();
				terrain.terrainData = GenerateTerrain(terrain.terrainData);
				OffsetX = Random.Range(0, 10000);
				//Depth = Random.Range(1, 5);;
			}

		}

		private TerrainData GenerateTerrain(TerrainData terrainData)
		{
			terrainData.heightmapResolution = Width + 1;
			terrainData.size = new Vector3(Width, Depth, Height);
			
			terrainData.SetHeights(0, 0, GenerateHeights(true));
			return terrainData;
		}


		private float[,] GenerateHeights(bool walls = false)
		{

			
			var heights = new float[Width, Height];
			foreach (var i in Enumerable.Range(0, NumberOfBumps))
			{
				var x = Random.Range(NeighboringBumps, Width - NeighboringBumps);
				var y = Random.Range(NeighboringBumps, Height - NeighboringBumps);
				var z = Random.Range(0, NeighboringBumps);
				foreach (var j in Enumerable.Range(-z, z))
				{
					var a = Random.Range(-x / NeighboringBumps, x / NeighboringBumps);
					var b = Random.Range(-y / NeighboringBumps, y / NeighboringBumps);
					heights[x+a, y+b] = CalculateHeight(x + a, y + b);
				}
			}

			if (walls)
			{
				foreach (var i in Enumerable.Range(0, Width))
					heights[i, 0] = 10000;

				foreach (var i in Enumerable.Range(0, Width))
					heights[0, i] = 10000;

				foreach (var i in Enumerable.Range(0, Width))
					heights[i, Width - 1] = 10000;

				foreach (var i in Enumerable.Range(0, Width))
					heights[Width - 1, i] = 10000;
			}

			return heights;
		}


		private float CalculateHeight(int x, int y)
		{
			var xCoord = (float) x / Width * Scale + OffsetX;
			var yCoord = (float) y / Width * Scale + OffsetY;

			return Mathf.PerlinNoise(xCoord, yCoord);
		}
	}
}