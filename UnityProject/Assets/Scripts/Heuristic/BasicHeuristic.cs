using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using MLAgents;
using UnityEditor;


namespace Heuristic
{
    public class BasicHeuristic : Decision
    {
        private bool isHeuristic = false;
        private float[] rayAngles = {0f, 45f, 90f, 135f, 180f, 110f, 70f};
        
        public string Tag;
        public float Speed;


        private Collider[] hitColliders;
        
        public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
        {
            if(!isHeuristic) isHeuristic = true;

            
            for (int i = 0; i < 7; i++)
            {
                if (vectorObs[i] == 1.0f)
                {
                    return new float[2] {1, Vector3.Angle(new Vector3(0, rayAngles[i], 0), Vector3.forward)};
                }
            }

            return new float[2]{1f, Time.frameCount % 100 == 0 ? Random.Range(0.1f, 0.3f) : 0};
        }

        public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
        {
            return new List<float>();
        }
    }
}