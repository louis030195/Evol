using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;


namespace Evol.Agents
{
	public class LivingBeingHeuristic : Decision
	{
		public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
		{
			//throw new System.NotImplementedException();
			return new[] {0f, 0f};
		}

		public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
		{
			//throw new System.NotImplementedException();
			return null;
		}
	}

}