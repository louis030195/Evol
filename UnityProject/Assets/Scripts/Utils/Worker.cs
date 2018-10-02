using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Utils
{
    [CreateAssetMenu(menuName = "Evol/Worker")]
    public class Worker : ScriptableObject
    {

        public GameObject WorkerPrefab;
        public int AmountOfWorkers = 1;
        public int AmountOfAgentsToAdd;

    }
}