using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Utils
{
    public class EventListenedList<T> : List<T> {

        public event EventHandler OnAdd;
        public event EventHandler OnRemove;
        // public event EventHandler OnUpdate;

        public void Add(T item) {
            base.Add(item);
            OnAdd?.Invoke(this, null); // should be renamed onaddafter
        }
        
        public bool Remove(T item) {
            var result = base.Remove(item);
            if(result) // We don't invoke the event if the item has failed to be removed
                OnRemove?.Invoke(this, null);
            return result;
        }

    }
}