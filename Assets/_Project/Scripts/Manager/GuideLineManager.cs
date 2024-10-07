using System.Collections.Generic;
using UnityEngine;
using TAGUNI;


public class GuideLineManager : TSingletonWithMono<GuideLineManager>
{
    [SerializeField] private GameObject previewBubble;

    private GuideLine _guideLine;

    // 가이드버블
    public GuideBubble GuideBubble { get; private set; }


    public override void Init()
    {
        base.Init();

        if (_guideLine == null)
            _guideLine = IngameViewManager.instance.BubbleShooter.InGameGuideLine;

        _guideLine.Init();

        ObjectPoolManager.Instance.LoadObjectPool(eObjPoolType.Ingame, "GuideBubble");
        GuideBubble = ObjectPoolManager.Instance.GetPoolObject<GuideBubble>(eObjPoolType.Ingame, "GuideBubble");

        GuideBubble.Init();
    }

    public void Dispose()
    {
        if (GuideBubble != null)
        {
            GuideBubble.Dispose();
            GuideBubble = null;
        }

        UpdateGuideLine();
    }

    public void UpdateGuideLine()
    {
        previewBubble.SetActive(true);

        if (GuideBubble != null)
            GuideBubble.StartCheck();
    }

    public void ResetGuideLine()
    {
        previewBubble.SetActive(false);

        if (_guideLine != null)
            _guideLine.ResetGuideLine();

        if (GuideBubble != null)
            GuideBubble.ResetGuide();
    }

    public void CreatePrevBubble(int posX, int posY)
    {
        previewBubble.transform.SetParent(IngameViewManager.instance.BubbleMapCanvas.transform);
        previewBubble.transform.localScale = Common.VECTOR3_ONE;
        previewBubble.transform.localPosition = BubbleManager.GetBubbleMapPos(posX, posY);
    }

    public void AddGuideLinePosition(int collisionCount, Vector3 position, bool isTarget = false)
    {
        _guideLine.AddGuideLinePosition(collisionCount, position, isTarget);
    }

    public int GetGuideLinePositionCount()
    {
        return _guideLine.GetGuideLinePositionCount();
    }

    public void SetGuideLine()
    {
        _guideLine.SetGuideLine();
    }
}
