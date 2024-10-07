using System;
using UnityEngine;
using DG.Tweening;


public class BubbleCanvas : MonoBehaviour
{
    private Vector3 _originPosition;

    private Tweener _moveTweener = null;

    // 이전에 포지션 저장변수
    private int _prevLastTopPosIndexY = 0;



    private void Awake()
    {
        _originPosition = transform.localPosition;
    }

    public void Start()
    {
    }

    public void SetMapCanvas(bool isGameStartView = false, Action moveCompleteCallback = null)
    {
        // 게임도입 연출부 이면 모든 버블을 활성화 한다.
        // 그외에 경우에는 화면 (혹은 화면 n칸위) 까지의 버블만 활성화 한다.
        if (isGameStartView)
            _prevLastTopPosIndexY = 0;

        //MapManager.instance.Init();
        //MapManager.instance.SetMapBubble();

        if (moveCompleteCallback != null)
            moveCompleteCallback();
    }
}
