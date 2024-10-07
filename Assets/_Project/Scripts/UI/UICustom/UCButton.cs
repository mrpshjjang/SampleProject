using DG.Tweening;
using Sigtrap.Relays;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

public class UCButton : Button
{
    #region Button override

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        Animate();

        _isPress = true;
        _isCancel = false;
        onStartClick?.Dispatch();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        BackToNormal();
        ResetClick();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        BackToNormal();
        ResetClick();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_keepCount == 0 && !_isCancel)
            base.OnPointerClick(eventData);

        ResetClick();

        //SoundManager.PlayClick();
    }

    #endregion


    #region public

    public eButtonPressType PressType => pressType;
    public readonly Relay onStartClick = new();
    public readonly Relay onResetClick = new();
    public bool IsLongPressing { get; private set; } = false;

    public void Cancel()
    {
        _isCancel = true;
        ResetClick();
    }

    #endregion


    #region protected

    #endregion


    #region private

    private const float KEEP_PRESS_CLICK_TIME = 0.05f;
    private const float KEEP_DELAY_TIME = 0.3f;
    private const float JELLY_GAP_DURATION = 0.1f;

    private bool _isPress = false;
    private bool _isCancel = false;

    private int _keepCount = 0;
    private float _checkDelayKeepTime = 0;
    private float _checkPressTime = 0f;

    private Sequence _sequence;
    private Transform _trButton;
    private Vector3 _targetScale;
    private Vector3 _originScale;

    [SerializeField] private bool isKeepClick = false;
    [SerializeField] private eButtonPressType pressType = eButtonPressType.Scale;
    [SerializeField] private float pressScaleX = 0.9f;
    [SerializeField] private float pressScaleY = 1.1f;
    [SerializeField] private float pressJellyMin = 0.9f;
    [SerializeField] private float pressJellyMax = 1.1f;

    private Transform GetTransform()
    {
        if (_trButton == null)
            _trButton = gameObject.transform;

        return _trButton;
    }

    private void PlayScaleAnimation()
    {
        KillAnimation();

        _targetScale = new Vector3(_originScale.x * pressScaleX, _originScale.y * pressScaleY, 1f);
        _sequence = DOTween.Sequence(GetTransform().DOScale(_targetScale, 0.2f).SetUpdate(true).SetEase(Ease.OutBack)).SetUpdate(true);
    }

    private void PlayJellyAnimation()
    {
        KillAnimation();

        _sequence = DOTween.Sequence().SetUpdate(true);

        _targetScale = new Vector3(_originScale.x * 0.9f, _originScale.y * 1.1f, 1f);
        _sequence.Append(GetTransform().DOScale(_targetScale, JELLY_GAP_DURATION).SetEase(Ease.OutBack).SetUpdate(true));

        _targetScale = new Vector3(_originScale.x * 1.1f, _originScale.y * 0.9f, 1f);
        _sequence.Append(GetTransform().DOScale(_targetScale, JELLY_GAP_DURATION).SetEase(Ease.OutBack).SetUpdate(true));

        _targetScale = _originScale;
        _sequence.Append(GetTransform().DOScale(_targetScale, JELLY_GAP_DURATION).SetEase(Ease.OutBack).SetUpdate(true));
    }

    private void BackToNormal()
    {
        KillAnimation();

        _targetScale = _originScale;
        _sequence = DOTween.Sequence().SetUpdate(true);

        GetTransform().DOScale(_originScale, 0.2f).SetUpdate(true);
    }

    private void KillAnimation()
    {
        _sequence.Kill();
        GetTransform().DOKill();
    }

    private bool CanFirstDelayKeep()
    {
        if (_checkDelayKeepTime < KEEP_DELAY_TIME)
        {
            _checkDelayKeepTime += Time.unscaledDeltaTime;
            return false;
        }

        return true;
    }

    private void UpdateKeepClick()
    {
        if (_isPress && isKeepClick && !_isCancel)
        {
            if (!CanFirstDelayKeep()) return;

            IsLongPressing = true;

            if (_checkPressTime < KEEP_PRESS_CLICK_TIME)
            {
                _checkPressTime += Time.unscaledDeltaTime;
            }
            else
            {
                _keepCount++;
                _checkPressTime = 0f;

                if (_keepCount <= 30) _keepCount++;
                if (_keepCount <= 60) _keepCount++;
                if (_keepCount <= 120) _keepCount++;
                if (_keepCount <= 150) _keepCount++;
                if (_keepCount <= 180) _keepCount++;
                if (_keepCount <= 240) _keepCount++;
                if (_keepCount <= 500) _keepCount += _keepCount / 10;

                for (var it = 0; it <= _keepCount / 60; it++)
                {
                    onClick?.Invoke();
                }

                KillAnimation();
                GetTransform().localScale = _originScale;
                Animate();
            }
        }
        else
        {
            IsLongPressing = false;
        }
    }

    private void ResetClick()
    {
        _isPress = false;
        _keepCount = 0;
        _checkDelayKeepTime = 0f;
        _checkPressTime = 0f;

        IsLongPressing = false;

        onResetClick?.Dispatch();
    }

    private void Animate()
    {
        switch (pressType)
        {
            case eButtonPressType.Scale: PlayScaleAnimation(); break;
            case eButtonPressType.Jelly: PlayJellyAnimation(); break;
        }
    }

    #endregion


    #region lifecycle

    protected override void Awake()
    {
        base.Awake();

        if (Application.isPlaying)
        {
            _originScale = GetTransform().localScale;
        }
    }

    protected override void Start()
    {
        base.Start();
        transition = Transition.None;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetClick();

        if (Application.isPlaying)
        {
            BackToNormal();
        }
    }

    protected override void OnDisable()
    {
        ResetClick();

        base.OnDisable();
    }

    protected virtual void Update()
    {
        UpdateKeepClick();
    }

    protected override void OnDestroy()
    {
        onStartClick?.RemoveAll();
        onResetClick?.RemoveAll();

        base.OnDestroy();
    }

    #endregion
}

public enum eButtonPressType
{
    Scale,
    Jelly,
    None
}
