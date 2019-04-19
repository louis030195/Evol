using UnityEngine;

namespace Evol.Utils
{
    public class MonoBehaviourFromScriptableObject : MonoBehaviour
    {
        public static MonoBehaviourFromScriptableObject instance;

        private void Awake() {
            instance = this;
        }
    }
}