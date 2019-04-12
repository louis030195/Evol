using UnityEngine;

namespace Evol.Game.Item
{
    public enum RuneEffect
    {
        Propagate,
        Duplicate,
        Empower
    }
    [CreateAssetMenu(fileName = "New rune", menuName = "Evol/Item/Rune")]
    public class RuneData : ItemData
    {
        public RuneEffect effect;
    }
}