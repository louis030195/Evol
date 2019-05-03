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
		public int updateRate = 8;
		
		public int depth = 10;
		public int mapSize = 100;
		[Range(1,40)] 
		public float noiseScale;
		public int octaves = 10;
		[Range(0,1)]
		public float persistence = 0.5f;
		public float lacunarity = 0.5f;
		public int seed;
		public Vector2 offset;

		private int frames = 0;

		private void FixedUpdate()
		{
			frames++;
			if (frames % Mathf.Pow(10, updateRate) == 0)
			{
				frames = 0;
				var terrain = GetComponent<Terrain>();
				terrain.terrainData = GenerateTerrain(terrain.terrainData);
				seed++;
			}

		}

		private TerrainData GenerateTerrain(TerrainData terrainData)
		{
			terrainData.heightmapResolution = mapSize + 1;
			terrainData.size = new Vector3(mapSize, depth, mapSize);
			
			terrainData.SetHeights(0, 0, GenerateNoiseMap());
			return terrainData;
		}


		private float[,] GenerateNoiseMap() {
			var noiseMap = new float[mapSize,mapSize];

			var prng = new System.Random (seed);
			var octaveOffsets = new Vector2[octaves];
			for (var i = 0; i < octaves; i++) {
				var offsetX = prng.Next (-100000, 100000) + offset.x;
				var offsetY = prng.Next (-100000, 100000) + offset.y;
				octaveOffsets [i] = new Vector2 (offsetX, offsetY);
			}

			if (noiseScale <= 0) {
				noiseScale = 0.0001f;
			}

			var maxNoiseHeight = float.MinValue;
			var minNoiseHeight = float.MaxValue;

			var halfWidth = mapSize / 2f;
			var halfHeight = mapSize / 2f;


			for (var y = 0; y < mapSize; y++) {
				for (var x = 0; x < mapSize; x++) {
		
					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;

					for (var i = 0; i < octaves; i++) {
						var sampleX = (x-halfWidth) / noiseScale * frequency + octaveOffsets[i].x;
						var sampleY = (y-halfHeight) / noiseScale * frequency + octaveOffsets[i].y;

						var perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						amplitude *= persistence;
						frequency *= lacunarity;
					}

					if (noiseHeight > maxNoiseHeight) {
						maxNoiseHeight = noiseHeight;
					} else if (noiseHeight < minNoiseHeight) {
						minNoiseHeight = noiseHeight;
					}
					noiseMap [x, y] = noiseHeight;
				}
			}

			for (var y = 0; y < mapSize; y++) {
				for (var x = 0; x < mapSize; x++) {
					noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
				}
			}

			return noiseMap;
		}
	}
}