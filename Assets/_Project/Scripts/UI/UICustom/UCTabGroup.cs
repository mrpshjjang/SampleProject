using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UCTabGroup : MonoBehaviour
{
    #region public

    public UnityAction<int> OnTabSelectEvent;

    public List<UCTab> Tabs => tabs;

    public bool SelectorFlag => selector != null;
    public int CurId => _id;

    public void Select(int id)
    {
        OnTabSelect(id);
    }

    public void ForceMoveSelector(int id)
    {
        _id = id;
    }

    public void Reset()
    {
        foreach (var tab in tabs)
        {
            tab.SetSelect(false);
        }
    }

    #endregion


    #region protected

    protected virtual void OnTabSelect(int id)
    {
        var tab = tabs.Find(d => d.Id == id);

        if (tab != null)
        {
            SelectTab(tab.Id);
            //this.MoveSelector(tab);
            OnTabSelectEvent?.Invoke(tab.Id);
            //SoundManager.PlaySFX("sfx_tap");
        }
    }

    #endregion


    #region private

    [SerializeField] private RectTransform selector;
    [SerializeField] private eTabSelectorDirection selectorDirection;
    [SerializeField] private float selectorMoveDuration = 0.3f;
    [SerializeField] private List<UCTab> tabs = new();

    private Vector3 _velocity;
    private int _id = -1;

    private void SelectTab(int id)
    {
        if (_id == id)
        {
            return;
        }

        _id = id;

        foreach (var tab in tabs)
        {
            tab.SetSelect(tab.Id == id);
        }
    }

    private void RegisterEvent()
    {
        foreach (var tab in tabs)
        {
            tab.OnTabClickEvent.AddListener(OnTabSelect);
        }
    }

    private void ClearEvent()
    {
        foreach (var tab in tabs)
        {
            tab.OnTabClickEvent.RemoveListener(OnTabSelect);
        }
    }

    // private void MoveSelector(UCTab tab)
    // {
    //     if (this.selector == null)
    //     {
    //         return;
    //     }
    //
    //     Vector3 targetPos = Vector3.one;
    //     RectTransform rtTab = tab.GetComponent<RectTransform>();
    //
    //     switch (this.selectorDirection)
    //     {
    //         case eTabSelectorDirection.HORIZONTAL:
    //
    //             this.selector.DOKill();
    //             targetPos = new Vector3(rtTab.anchoredPosition.x, this.selector.anchoredPosition.y, this.selector.anchoredPosition3D.z);
    //             this.selector.DOAnchorPosX(rtTab.anchoredPosition.x, this.selectorMoveDuration).SetUpdate(true);
    //
    //             break;
    //
    //         case eTabSelectorDirection.VERTICAL:
    //
    //             this.selector.DOKill();
    //             targetPos = new Vector3(this.selector.anchoredPosition.x, rtTab.anchoredPosition.y, this.selector.anchoredPosition3D.z);
    //             this.selector.DOAnchorPosY(rtTab.anchoredPosition.y, this.selectorMoveDuration).SetUpdate(true);
    //
    //             break;
    //
    //         default:
    //
    //             return;
    //     }
    // }

    #endregion


    #region lifecycle

    protected virtual void Awake()
    {
        RegisterEvent();
    }

    protected virtual void OnEnable()
    {
        // foreach (UCTab tab in this.tabs)
        // {
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(tab.GetComponent<RectTransform>());
        // }
        //
        // LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    protected virtual void Update()
    {
        if (_id == -1)
        {
            return;
        }

        if (selector == null)
        {
            return;
        }

        var tab = tabs.Find(d => d.Id == _id);
        var rtTab = tab.GetComponent<RectTransform>();

        switch (selectorDirection)
        {
            case eTabSelectorDirection.HORIZONTAL:
            {
                var curPos = selector.anchoredPosition;
                Vector2 goalPos = new(rtTab.anchoredPosition.x, selector.anchoredPosition.y);
                //this.selector.anchoredPosition = Vector3.Lerp(curPos, goalPos, Time.unscaledDeltaTime * 15f);
                selector.anchoredPosition = Vector3.SmoothDamp(curPos, goalPos, ref _velocity, 0.15f, float.MaxValue, Time.unscaledDeltaTime);
                break;
            }
            case eTabSelectorDirection.VERTICAL:
            {
                var curPos = selector.anchoredPosition;
                Vector2 goalPos = new(selector.anchoredPosition.x, rtTab.anchoredPosition.y);
                //this.selector.anchoredPosition = Vector3.Lerp(curPos, goalPos, Time.unscaledDeltaTime * 15f);
                selector.anchoredPosition = Vector3.SmoothDamp(curPos, goalPos, ref _velocity, 0.15f, float.MaxValue, Time.unscaledDeltaTime);
                break;
            }
            default:

                return;
        }
    }

    protected virtual void OnDestroy()
    {
        ClearEvent();
    }

    #endregion
}

public enum eTabSelectorDirection
{
    HORIZONTAL,
    VERTICAL
}
