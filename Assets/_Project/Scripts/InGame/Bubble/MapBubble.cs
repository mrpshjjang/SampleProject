using UnityEngine;
using DG.Tweening;
using System;

public class MapBubble : MonoBehaviour
{
    // 충돌체
    public CircleCollider2D Collider { get; private set; }

    // 버블의 위치인덱스
    public int IndexX { get; private set; }
    public int IndexY { get; private set; }

    private BaseBubble _bubble = null;

    // 고유세팅
    private bool _placeMove;
    private bool _placeScale;

    // 액션 트위너
    private Sequence _floatingAction = null;
    protected Sequence _placedAction = null;

    // 드랍효과 적용트리거
    private bool isDrop;

    public void Init()
    {
        if (_bubble == null)
            _bubble = GetComponent<BaseBubble>();

        // 추락액션 살아있으면 초기화
        if (_floatingAction != null)
        {
            _floatingAction.Kill();
            _floatingAction = null;
        }

        if (_placedAction != null)
        {
            _placedAction.Kill();
            _placedAction = null;
        }

        if (Collider == null)
            Collider = gameObject.GetComponent<CircleCollider2D>();

        // 위치데이터 초기화
        IndexX = 0;
        IndexY = 0;

        isDrop = false;
    }

    public void Dispose()
    {
        if (_floatingAction != null)
        {
            _floatingAction.Kill();
            _floatingAction = null;
        }

        if (_placedAction != null)
        {
            _placedAction.Kill();
            _placedAction = null;
        }

        isDrop = false;
    }

    public void SetPlaceOption (bool placeMove = true, bool placeScale = true)
    {
        _placeMove = placeMove;
        _placeScale = placeScale;
    }

    public void PlaceBubble(bool setScale, Vector3 actionForce, Action placeComplete = null)
    {
        // 맵 배치연출 호출
        // Default 연출
        if (_placedAction != null)
        {
            _placedAction.Kill();
            _placedAction = null;
        }

        _placedAction = DOTween.Sequence();

        if (!_placeMove && (!_placeScale || !setScale))
        {
            // 아무런 연출도 세팅되지 않았을 경우, 바로 콜백
            if (placeComplete != null)
                placeComplete();
        }
        else
        {
            // 연출이 세팅되었을 경우, 재생 후 콜백
            _placedAction.Play().OnComplete(() => {
                if (placeComplete != null)
                    placeComplete();
            });
        }
    }

    private void DropLogic()
    {
        if (transform.position.y <= IngameViewManager.instance.BubbleDropLineTrans.position.y)
        {
            isDrop = false;

            transform.SetParent(IngameViewManager.instance.BubbleDropLineTrans);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

            Vector3 targetPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-100, 100), transform.position.y + UnityEngine.Random.Range(0, -50));
            float targetPositionDistance = Vector3.Distance(transform.position, targetPosition);

            transform.DOJump(targetPosition, UnityEngine.Random.Range(40f, 80f), 1, targetPositionDistance/270f + UnityEngine.Random.Range(0f, 0.1f)).SetEase(Ease.Linear).OnComplete(() =>
            {
                //_bubble.FloatingBubble(this);
            });
        }
    }

    private void FixedUpdate()
    {
        if (isDrop)
            DropLogic();
    }

    public void UpdateMapBubbleIndex(int idxX, int idxY, bool isCollider = true)
    {
        // 위치인덱스 세팅
        IndexX = idxX;
        IndexY = idxY;

        Collider.enabled = true;
    }

    public void DestroyBubble()
    {
        // 필드맵데이터에서 버블 삭제
        Collider.enabled = false;

        MapManager.instance.RemoveBubbleFieldData(IndexX, IndexY);
        transform.SetParent(IngameViewManager.instance.BubbleDropLineTrans);
    }

    public void SetBottomDestroyReady()
    {
        MapManager.instance.RemoveBubbleFieldData(IndexX, IndexY);
        Collider.enabled = false;

        isDrop = true;
    }

    // 버블 떨구기
    public void FloatingBubble()
    {
        // 필드맵데이터에서 버블 삭제
        Collider.enabled = false;
        transform.SetParent(IngameViewManager.instance.BubbleDropLineTrans);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

        MapManager.instance.RemoveBubbleFieldData(IndexX, IndexY);

        Vector3 targetPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-150, 150), IngameViewManager.instance.BubbleDropLineTrans.position.y + UnityEngine.Random.Range(0, -100));
        float targetPositionDistance = Vector3.Distance(transform.position, targetPosition);

        transform.DOJump(targetPosition, transform.position.y + UnityEngine.Random.Range(20f, 50f), 1, targetPositionDistance / 360f + UnityEngine.Random.Range(0f, 0.1f)).SetEase(Ease.Linear).OnComplete(() =>
        {
            _bubble.FloatingBubble(this);
        });
    }

    public void CrashShootBubble(eBubbleType crashType, bool isGuide, Action<bool> endCallback)
    {
        if (endCallback != null)
            endCallback(true);
    }
}
