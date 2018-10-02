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
            
            //transform.position = Vector3.MoveTowards(transform.position, bestCollider.transform.position, Speed * Time.deltaTime);
            if(vectorObs[0] == 1.0f)
                return new float[2]{1, 0};
            return new float[2]{0, 2};
        }

        public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
        {
            return new List<float>();
        }
    }
}