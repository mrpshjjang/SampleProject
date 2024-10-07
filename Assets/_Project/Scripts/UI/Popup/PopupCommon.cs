using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupCommon : PopupBase
{
    #region public

    public override void Show()
    {
        base.Show();

        StopAllCoroutines();
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
    }

    public override void Hide()
    {
        InputManager.Instance.RemoveEscapeStack(gameObject);
        StopAllCoroutines();
        base.Hide();
    }

    public void SetData(
        string title, string desc,
        (bool, string, UnityAction) btnN,
        (bool, string, UnityAction) btnY,
        UnityAction escapeAction)
    {
        txtTitle.text = title;
        txtDesc.text = desc;

        button1.gameObject.SetActive(btnN.Item1);
        if (btnN.Item1)
        {
            txtButton1.text = btnN.Item2;
            button1.onClick.RemoveAllListeners();
            button1.onClick.AddListener(btnN.Item3);
        }

        button2.gameObject.SetActive(btnY.Item1);
        if (btnY.Item1)
        {
            txtButton2.text = btnY.Item2;
            button2.onClick.RemoveAllListeners();
            button2.onClick.AddListener(btnY.Item3);
        }

        _escapeAction = escapeAction;

        if (escapeAction == null)
        {
            closeByEscapeType = ePopupEscapeType.BLOCK_ESCAPE;
            InputManager.Instance.RemoveEscapeStack(gameObject);
        }
        else
        {
            closeByEscapeType = ePopupEscapeType.IGNORE;
            InputManager.Instance.AddEscapeStack(gameObject, InvokeEscapeAction);
        }
    }
    
    public void SetButtonCallBackTimer(int timeSec)
    {
        _timeSec = timeSec;

        StartCoroutine(nameof(CheckButtonCallBack));
    }

    #endregion


    #region private

    private UnityAction _escapeAction = null;
    private int _timeSec = 0;

    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtDesc;
    [SerializeField] private UCButton button1;
    [SerializeField] private Text txtButton1;
    [SerializeField] private UCButton button2;
    [SerializeField] private Text txtButton2;

    private void InvokeEscapeAction()
    {
        _escapeAction?.Invoke();
    }
    
    private IEnumerator CheckButtonCallBack()
    {
        while (_timeSec > 0)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            _timeSec--;
        }

        button2.onClick?.Invoke();
    }
    
    #endregion


    #region lifecycle

    protected override void OnDisable()
    {
        StopAllCoroutines();
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }

    #endregion
}
