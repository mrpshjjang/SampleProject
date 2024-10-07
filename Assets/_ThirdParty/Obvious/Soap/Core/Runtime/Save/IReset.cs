#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Obvious.Soap
{
    public interface IReset
    {
        void ResetToInitialValue();
#if UNITY_EDITOR
        void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange);
#endif
    }
}