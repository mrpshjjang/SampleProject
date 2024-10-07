using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UCTabEvent : UnityEvent<int> { }

public class UCTab : MonoBehaviour, IPointerClickHandler
{
    public UCTabEvent OnTabClickEvent
    {
        get => onTabEvent;
        set => onTabEvent = value;
    }

    #region public

    public int Id => id;

    public void SetSelect(bool isSelect)
    {
        _isSelect = isSelect;

        if (goActive != null)
        {
            goActive.SetActive(_isSelect);
        }

        if (goDeActive != null)
        {
            goDeActive.SetActive(!_isSelect);
        }
    }

    #endregion


    #region protected

    #endregion


    #region private

    [SerializeField] private int id;
    [SerializeField] private GameObject goActive;
    [SerializeField] private GameObject goDeActive;
    [SerializeField] private UCTabEvent onTabEvent;
    [SerializeField] private bool isIgnoreSelectAgain = true;

    private bool _isSelect = false;

    #endregion


    #region lifecycle

    #endregion


    #region IPointerClickHandler override

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSelect || !isIgnoreSelectAgain)
        {
            SetSelect(!_isSelect);
            onTabEvent?.Invoke(id);
        }
    }

    #endregion
}
