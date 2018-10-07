using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEditor;


namespace Heuristic
{
    public class BasicHeuristic : MonoBehaviour, Decision
    {
        private bool isHeuristic = false;
        
        public string Tag;
        public float Speed;



        public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
        {
            if(!isHeuristic) isHeuristic = true;

            return new float[2]{0, 0};
        }

        public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
        {
            return new List<float>();
        }
    }
}