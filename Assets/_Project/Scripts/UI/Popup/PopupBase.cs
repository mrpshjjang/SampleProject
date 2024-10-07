using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Sigtrap.Relays;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    #region public

    public ePopupEscapeType CloseByEscapeType => closeByEscapeType;

    public enum eAnimType
    {
        None,
        Fade,
        Bubble,
        Slide_Bounce,
        Animator,
        Slide_Cubic
    }

    public void SetType(PopupData data)
    {
        popupData = data;
        popupType = data.Type;
    }

    public virtual void Show()
    {
        _isHiding = false;
        canvasGroup.DOKill();
        rtContent.DOKill();

        switch (animType)
        {
            case eAnimType.Fade:         ShowFadeIn(); break;
            case eAnimType.Bubble:       ShowBubble(); break;
            case eAnimType.Slide_Bounce: ShowSlideBounce(); break;
            case eAnimType.Animator:     ShowAnimator(); break;
            case eAnimType.Slide_Cubic:  ShowSlideCubic(); break;
            default:                     gameObject.SetActive(true); break;
        }

        if (closeByEscapeType == ePopupEscapeType.CAN_ESCAPE)
        {
            InputManager.Instance.AddEscapeStack(gameObject, Hide);
        }

        if (txtCloseTimer != null)
            txtCloseTimer.text = string.Empty;

        //SoundManager.PlaySFX("sfx_popup2");

        Debug.Log("Show :: " + popupType);

        if (useAutoHideTimerWhenOpen)
            SetHideTimer(autoHideTime);
    }

    public virtual void Hide()
    {
        PopupManager.HidePopupData(popupData);
    }

    public virtual void HideAnim()
    {
        if (_isHiding) return;

        _isHiding = true;
        canvasGroup.DOKill();
        rtContent.DOKill();

        switch (animType)
        {
            case eAnimType.Fade:         HideFadeOut(); break;
            case eAnimType.Bubble:       HideBubble(); break;
            case eAnimType.Slide_Bounce: HideSlideBounce(); break;
            case eAnimType.Animator:     HideAnimator(); break;
            case eAnimType.Slide_Cubic:  HideSlideCubic(); break;
            default:                     gameObject.SetActive(false); break;
        }

        if (closeByEscapeType == ePopupEscapeType.CAN_ESCAPE)
        {
            InputManager.Instance.RemoveEscapeStack(gameObject);
        }

        _instantCloseAction?.Dispatch();
        _instantCloseAction?.RemoveAll();
        _ctsHideTimer.Cancel();

        Debug.Log("Hide :: " + popupType);
    }

    public void CancelHideTimer()
    {
        _ctsHideTimer?.Cancel();
        if (txtCloseTimer != null)
            txtCloseTimer.text = string.Empty;
    }

    public async UniTask SetHideTimer(int second)
    {
        _ctsHideTimer?.Cancel();
        _ctsHideTimer = new CancellationTokenSource();

        var token = _ctsHideTimer;

        try
        {
            for (int iter = 0; iter < second; iter++)
            {
                if (_ctsHideTimer.IsCancellationRequested) break;

                if(txtCloseTimer != null)
                    txtCloseTimer.text = (second - iter).ToString();

                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token.Token, ignoreTimeScale:true);
            }

            if (txtCloseTimer != null)
                txtCloseTimer.text = string.Empty;

            Hide();
        }
        catch (Exception e)
        {
            if (token.IsCancellationRequested) return;
        }
    }

    public void ShowFadeIn()
    {
        gameObject.SetActive(true);

        canvasGroup.DOKill();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, duration).SetUpdate(true).OnComplete(ShowComplete);
    }

    public void HideFadeOut()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
        {
            HideComplete();
            // this.gameObject.transform.SetParent(PopupManager.Parent);
            canvasGroup.blocksRaycasts = true;

            // if(this.popupType != ePopupType.StartLoading && this.popupType != ePopupType.Reward)
            //     PopupManager.CheckLayer();

            // if(this as PopupCommon == null) this.gameObject.SetActive(false);
            // else                            PopupManager.DestroyPopup(this);
            gameObject.SetActive(false);
        });
    }

    public void AddInstantCloseAction(Action action)
    {
        _instantCloseAction.AddListener(action);
    }

    #endregion


    #region protected

    [Header("Common")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rtContent;
    [SerializeField] protected float duration = 0.3f;
    [SerializeField] protected ePopupEscapeType closeByEscapeType = ePopupEscapeType.CAN_ESCAPE;
    [SerializeField] protected eAnimType animType;
    [DrawIf("animType", eAnimType.Animator)] protected Animator animator;
    [DrawIf("animType", eAnimType.Animator)] protected string animatorCloseMessage = "Close";
    [DrawIf("animType", eAnimType.Animator)] protected string animatorOpenMessage = "Open";

    [FormerlySerializedAs("useAutoCloseTimerWhenOpen")]
    [Header("Optional")]
    [SerializeField] private bool useAutoHideTimerWhenOpen;
    [DrawIf("useAutoCloseTimerWhenOpen", true)] private int autoHideTime;
    [SerializeField] private Text txtCloseTimer;

    protected PopupData popupData;
    protected ePopupType popupType;

    protected virtual void ShowComplete() { }

    protected virtual void HideComplete() { }

    #endregion


    #region private

    private bool _isHiding = false;
    private readonly Relay _instantCloseAction = new();
    private CancellationTokenSource _ctsHideTimer = new();

    private void ShowBubble()
    {
        ShowFadeIn();

        var size = rtContent.localScale;
        size = Vector3.zero;
        rtContent.localScale = size;
        rtContent.DOKill();
        rtContent.DOScale(Vector3.one, duration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(ShowComplete);
    }

    private void HideBubble()
    {
        HideFadeOut();
        rtContent.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            HideComplete();
        });
    }

    private void ShowSlideBounce()
    {
        ShowFadeIn();
        rtContent.DOKill();
        rtContent.anchoredPosition = new Vector2(rtContent.anchoredPosition.x, -rtContent.rect.height);
        rtContent.DOAnchorPosY(0f, duration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(ShowComplete);
    }

    private void HideSlideBounce()
    {
        HideFadeOut();
        rtContent.DOKill();
        rtContent.DOAnchorPosY(-rtContent.rect.height, duration).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            HideComplete();
        });
    }

    private void ShowSlideCubic()
    {
        ShowFadeIn();
        rtContent.DOKill();
        rtContent.anchoredPosition = new Vector2(rtContent.anchoredPosition.x, -rtContent.rect.height);
        rtContent.DOAnchorPosY(0f, duration).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(ShowComplete);
    }

    private void HideSlideCubic()
    {
        HideFadeOut();
        rtContent.DOKill();
        rtContent.DOAnchorPosY(-rtContent.rect.height, duration).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            HideComplete();
        });
    }

    private void ShowAnimator()
    {
        var tweenId = "Animator" + GetInstanceID();
        DOTween.Kill(tweenId);

        gameObject.SetActive(false);
        gameObject.SetActive(true);
        DOTween.Sequence().SetId(tweenId).SetUpdate(true).AppendInterval(0.1f).OnComplete(() => { DOTween.Sequence().SetId(tweenId).SetUpdate(true).AppendInterval(duration - 0.1f).OnComplete(ShowComplete); });
    }

    private void HideAnimator()
    {
        var tweenId = "Animator" + GetInstanceID();
        DOTween.Kill(tweenId);

        animator.SetTrigger(animatorCloseMessage);

        DOTween.Sequence().SetId(tweenId).SetUpdate(true).AppendInterval(duration).OnComplete(() =>
        {
            HideComplete();
            gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = true;
        });
    }

    #endregion


    #region Click Event

    public virtual void OnClickClose()
    {
        PopupManager.Hide(popupType);
    }

    #endregion


    #region lifecycle

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    protected virtual void OnDisable()
    {
        _isHiding = false;
        _instantCloseAction?.Dispatch();
        _instantCloseAction?.RemoveAll();
        PopupManager.RemoveStack(popupType);
    }

    protected virtual void OnDestroy()
    {
        _instantCloseAction?.RemoveAll();
    }

    #endregion

    public enum ePopupEscapeType
    {
        CAN_ESCAPE,
        BLOCK_ESCAPE,
        IGNORE
    }
}
