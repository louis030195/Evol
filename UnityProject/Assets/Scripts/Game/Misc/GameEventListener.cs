using UnityEngine;
using UnityEngine.Events;

namespace Evol.Game.Misc
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent; 
        [SerializeField] private UnityEvent response; 

        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            response.Invoke();
        }
    }
}