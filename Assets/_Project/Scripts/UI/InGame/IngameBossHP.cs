using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class IngameBossHP : MonoBehaviour
{
    private float _maxGaugeWidth;
    private Tweener _gaugeTweener;
    private float _maxHP;
    private float _currentHP;

    [SerializeField] private RectTransform Gauge;

    private void Start()
    {
        _maxGaugeWidth = 520f;
    }

    public void Dispose()
    {
        IngameDataManager.instance.UpdateCurrentBossHP -= UpdateHP;
    }

    public void SetGauge()
    {
        _maxHP = IngameDataManager.instance.MaxBossHP;
        Gauge.sizeDelta = new Vector2 (_maxGaugeWidth, 32f);

        IngameDataManager.instance.UpdateCurrentBossHP += UpdateHP;
    }

    public void UpdateHP(int currentHP)
    {
        if (Gauge != null)
        {
            float percentage = currentHP / _maxHP;
            if (percentage > 1f)
                percentage = 1f;

            float gaugeWidth = _maxGaugeWidth * percentage;
            if (gaugeWidth < 0f)
                gaugeWidth = 0f;

            if (_gaugeTweener != null)
            {
                _gaugeTweener.Kill();
                _gaugeTweener = null;
            }

            _gaugeTweener = Gauge.DOSizeDelta(new Vector2(gaugeWidth, Gauge.sizeDelta.y), 0.4f).SetEase(Ease.OutQuart);
        }
    }

}
