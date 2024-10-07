using System;
using UnityEngine;

namespace Obvious.Soap
{
    [Serializable]
    public abstract class ScriptableBase : ScriptableObject
    {
        public virtual void Reset() { }
        public Action RepaintRequest;
    }
}