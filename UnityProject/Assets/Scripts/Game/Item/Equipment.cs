using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Item
{
    public abstract class Equipment : ItemData
    {
        // Each equipment has a list of stats, the value isn't set directly in the ScriptableObject because it's not fixed
        //I think all equipment have specific stats, for example an amulet can have stats that a ring can't have
    }
}