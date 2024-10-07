using UnityEngine;
using TAGUNI;
using DG.Tweening;
using System.Collections;
using System;
using System.Collections.Generic;

public class IngameViewManager : TSingletonWithMono<IngameViewManager>
{
    public readonly Vector3 VECTOR3_CAMERA_ORIGINAL_POSITION = new Vector3(0f, 0f, -10f);

    public BubbleCanvas BubbleCanvas => BubbleMapCanvas;

    [SerializeField] public Camera IngameCamera;
    [SerializeField] public BubbleShooter BubbleShooter;
    [SerializeField] public Transform LeftLineTrans;
    [SerializeField] public Transform RightLineTrans;
    [SerializeField] public BubbleCanvas BubbleMapCanvas;
    [SerializeField] public Transform BubbleDropLineTrans;
    [SerializeField] public IngameBossHP UI_IngameBossHP;

    private SpecMapConfig _specMapConfig;


    private void Start()
    {
        IngameManager.instance.Init();
        Init();
    }

    public override void Init()
    {
        // 해상도 대응함수
        SetScreen();

        // 카메라 위치 초기화
        IngameCamera.transform.localPosition = VECTOR3_CAMERA_ORIGINAL_POSITION;

        LoadMap(1);
    }

    public void LoadMap(int level)
    {
        IngameDataManager.instance.InitData(level);
        MapManager.instance.SetMapBubble(level);
        SetLevel(level);
        BubbleShooter.Init();
        IngameManager.instance.StartPlayerTurn();
    }

    public void SetLevel(int level)
    {
        _specMapConfig = Facade.Spec.Data.SpecMapConfig.Find(e => e.level == level);

        UI_IngameBossHP.gameObject.SetActive(_specMapConfig.clear_type == eClearType.BOSS_HP);
        UI_IngameBossHP.SetGauge();
    }

    #region 해상도 세팅 setScreen()
    private void SetScreen()
    {
        float mainResolution = 750f / 1334f;
        float targetResolution = (float)Screen.width / (float)Screen.height;

        float resolutionValue = 1f;

        if (mainResolution >= targetResolution)
            resolutionValue = mainResolution / targetResolution;

        IngameCamera.orthographicSize = 667f * resolutionValue;
    }
    #endregion
}
