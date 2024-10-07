using TMPro;
using UnityEngine;

namespace Obvious.Soap
{
    [AddComponentMenu("Soap/Bindings/BindInputField")]
    [RequireComponent(typeof(TMP_InputField))]
    public class BindInputField : CacheComponent<TMP_InputField>
    {
        [SerializeField] private StringVariable _stringVariable = null;

        protected override void Awake()
        {
            base.Awake();
            _component.text = _stringVariable;
            _component.onValueChanged.AddListener(SetBoundVariable);
        }

        private void OnDestroy() => _component.onValueChanged.RemoveListener(SetBoundVariable);

        private void SetBoundVariable(string value) => _stringVariable.Value = value;
        
    }
}