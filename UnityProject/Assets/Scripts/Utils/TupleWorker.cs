using UnityEngine;

namespace Evol.Utils
{
    [System.Serializable]
    public class TupleWorker : Tuple<GameObject, int>
    {
        public static TupleWorker Zero
        {
            get { return new TupleWorker(new GameObject(), 0); }
        }

        public TupleWorker(GameObject a, int b)
            : base(a, b)
        {
        }
    }
}