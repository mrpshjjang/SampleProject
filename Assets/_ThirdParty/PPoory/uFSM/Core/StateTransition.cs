using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPoory
{
    [System.Serializable]
    public class StateTransition
    {
        [System.Serializable]
        public class Tansition
        {
            public string eventName;
            public BaseState targetState;
        }

        public BaseState state;
        public List<Tansition> tansitionList = new List<Tansition>();
    }
}