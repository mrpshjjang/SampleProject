using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]

public class UCTab_Text : UCTab
{

    #region public

    public void SetText(string txtTab)
    {
        _txtTab = txtTab;
        txtActive.text = txtTab;
        txtDeActive.text = txtTab;
    }

    public void SetTextColor(Color color)
    {
        txtActive.color = color;
        txtDeActive.color = color;
    }

    public string GetText()
    {
        return _txtTab;
    }

    #endregion


    #region protected

    #endregion


    #region private

    private string _txtTab;

    [SerializeField] private Text txtActive;
    [SerializeField] private Text txtDeActive;

    #endregion


    #region lifecycle

    #endregion
}
