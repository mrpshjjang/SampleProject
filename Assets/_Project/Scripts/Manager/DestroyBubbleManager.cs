using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAGUNI;
using UnityEngine;

public class DestroyBubbleManager : TSingleton<DestroyBubbleManager>
{
    // 순차적 삭제체크를 위한 기준 카운트
    private int _destroyCount;

    // 순차적 삭제체크를 위한 비교 카운트
    private int _currentDestroyStartCount;
    private int _currentDestroyCompleteCount;

    // 버스트버블에의해 파괴되는 버블리스트
    List<BaseBubble> _destroyByBurstBubbleList = new List<BaseBubble>();

    Action<bool> _destroyCompleteCallback;


    public override void Init()
    {
        base.Init();

        _destroyCount = 0;
        _currentDestroyStartCount = 0;
        _currentDestroyCompleteCount = 0;
    }

    public void Dispose()
    {
        _destroyCount = 0;
        _currentDestroyStartCount = 0;
        _currentDestroyCompleteCount = 0;
    }

    public void DestroyBubbles(List<BaseBubble> targetBubbleList, int mainTargetPosX, int mainTargetPosY, Action<bool> endCallback, eDestroyType destroyType = eDestroyType.NORMAL)
    {
        if (targetBubbleList.Count <= 0)
        {
            if (endCallback != null)
                endCallback(false);

            return;
        }

        _destroyByBurstBubbleList.Clear();

        // 기준카운트 세팅
        _destroyCount = 0;

        // 터트리기 종료 시, 호출될 액션함수 세팅
        _destroyCompleteCallback = endCallback;

        // 비교카운트 초기화
        _currentDestroyStartCount = 0;
        _currentDestroyCompleteCount = 0;

        _destroyCount = _destroyByBurstBubbleList.Count;

        for (int i = 0; i < _destroyByBurstBubbleList.Count; i++)
        {
            DestroyBubble(_destroyByBurstBubbleList[i], mainTargetPosX, mainTargetPosY, eDestroyType.BURST);

            if (targetBubbleList.Contains(_destroyByBurstBubbleList[i]))
                targetBubbleList.Remove(_destroyByBurstBubbleList[i]);
        }

        _destroyCount += targetBubbleList.Count;

        // 버블터트리기 시작 (딜레이)
        for (int i = 0; i < targetBubbleList.Count; i++)
            DestroyBubble(targetBubbleList[i], mainTargetPosX, mainTargetPosY, destroyType);
    }

    private void DestroyBubble(BaseBubble destroyBubble, int targetBubbleIndexX, int targetBubbleIndexY, eDestroyType destroyType)
    {
        float delayTime = 0f;

        if (targetBubbleIndexX != -1 && targetBubbleIndexY != -1)
        {
            float distance = Vector3.Distance(BubbleManager.GetBubbleMapPos(targetBubbleIndexX, targetBubbleIndexY),
                                              BubbleManager.GetBubbleMapPos(destroyBubble.MapBubbleComponent.IndexX, destroyBubble.MapBubbleComponent.IndexY));
            delayTime = distance / 1000f;
        }

        destroyBubble.MapBubbleComponent.DestroyBubble();
        destroyBubble.DestroyBubble(delayTime, DestroyCompleteCheck, DestroyStartCheck);
    }

    private void DestroyStartCheck()
    {
        _currentDestroyStartCount++;

        // 모든버블이 터진 시점에 바로 추락체크
        if (_destroyCount <= _currentDestroyStartCount)
            FloatingBubbleManager.instance.CheckFloatingBubble();
    }

    private async void DestroyCompleteCheck()
    {
        // 터트리기 버블이 터질때마다 비교카운트 +1
        _currentDestroyCompleteCount++;

        // 비교카운트가 기준카운트에 도달했을 때
        if (_destroyCount <= _currentDestroyCompleteCount)
        {
            // 모든 버블이 터졌다고 판단
            if (_destroyCompleteCallback != null)
                _destroyCompleteCallback(true);
        }
    }
}
