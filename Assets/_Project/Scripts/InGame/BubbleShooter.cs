using UnityEngine;
using DG.Tweening;
using System;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;

public class BubbleShooter : MonoBehaviour
{
    public static readonly Vector3 SHOOT_POSITION_VECTOR = new Vector3(0f, 0f, 1f);         // 슛버블 위치
    private static readonly Vector3 READY_POSITION_VECTOR = new Vector3(70f, -43f, 1f);     // 대기버블 위치
    private static readonly Vector3 SKILL_POSITION_VECTOR = new Vector3(38f, -111f, 1f);   // 스킬버블 위치
    private static readonly Vector3 BUFF_POSITION_VECTOR = new Vector3(-26f, -117f, 1f);     // 스킬버블 위치

    private static readonly Vector3[] ARR_POSITION = new[]
        { SHOOT_POSITION_VECTOR, READY_POSITION_VECTOR, SKILL_POSITION_VECTOR, BUFF_POSITION_VECTOR }; 

    // 가이드라인
    [SerializeField] public GuideLine InGameGuideLine;
    // 남은 슈팅카운트 텍스트
    [SerializeField] private TextMeshPro _bubbleCount;
    // 슈팅버블 리스트
    public List<ShootBubble> ReloadBubbleList { get; private set; } = new List<ShootBubble>();

    // 아이템버블
    public ShootBubble ItemBubble { get; private set; }

    // 슛할준비 체크용 변수
    public bool isShootReady { get; private set; }

    private bool isItemShoot;
    private bool _bubbleHide = false;

    public int EnableEquipCount { get; private set; }

    public eBubbleType LastShootType { get; private set; }

    public void Init()
    {
        UpdateBubbleCount(IngameDataManager.instance.CurrentBubbleCount);

        // 준비체크변수 초기화
        _bubbleHide = false;
        isShootReady = false;
        isItemShoot = false;

        EnableEquipCount = 2;

        // 버블 준비
        InitBubbles(() => GuideLineManager.instance.Init());

        IngameDataManager.instance.UpdateCurrentBubbleCount += UpdateBubbleCount;
    }

    public void Dispose()
    {
        gameObject.SetActive(false);
        IngameDataManager.instance.UpdateCurrentBubbleCount -= UpdateBubbleCount;
    }

    private void UpdateBubbleCount(int count)
    {
        _bubbleCount.text = count.ToString();
    }


    public eBubbleType ShootReadyBubbleType ()
    {
        if (isItemShoot)
            return ItemBubble.GetBubbleType();
        else if (ReloadBubbleList.Count > 0)
            return ReloadBubbleList[0].GetBubbleType();
        else
            return eBubbleType.NONE;
    }

    public Vector3 ShootReadyBubblePosition ()
    {
        if (ReloadBubbleList.Count > 0)
            return ReloadBubbleList[0].transform.position;
        else
            return Common.VECTOR3_ZERO;
    }

    public void InitBubbles(Action completeCallback)
    {
        CreateNextBubble(()=>
        {
            // 슈팅준비완료 체크
            isShootReady = true;

            if (completeCallback != null)
                completeCallback();
        });
    }

    #region "버블 슈팅관련"
    public void BubbleShooting(float angle)
    {
        // 슈팅준비가 되었다면
        if (isShootReady)
        {
            isShootReady = false;
            GuideLineManager.instance.ResetGuideLine();

            if (isItemShoot)
            {
                LastShootType = ItemBubble.GetBubbleType();

                ItemBubble.StartShoot(angle);
                ItemBubble = null;
            }
            else
            {
                if (ReloadBubbleList.Count > 0)
                {
                    IngameDataManager.instance.SetBubbleAddCount(-1);

                    // 슛버블 슈팅
                    ReloadBubbleList.First().StartShoot(angle);

                    LastShootType = ReloadBubbleList.First().GetBubbleType();

                    ReloadBubbleList.RemoveAt(0); 

                    // 실제로 슈팅했을 때, 콜백함수 호출
                    IngameManager.instance.StartShooting();
                }
            }
        }
    }

    public void ResetShootingBubble()
    {
        for (int i = 0; i < ReloadBubbleList.Count; ++i)
        {
            Destroy(ReloadBubbleList[i]);
            ReloadBubbleList[i] = null;
        }
        
        ReloadBubbleList.Clear();
    }
    #endregion

    private void ResetBubblePosition(bool moveReset, Action moveComplete = null)
    {
        if (ReloadBubbleList.Count > 0)
        {
            SetNormalShooter(true);

            for (int i = 0; i < ReloadBubbleList.Count; i++)
            {
                Vector3 position = position = ARR_POSITION[i];
                
                if (moveReset)
                {
                    if (i == ReloadBubbleList.Count - 1)
                    {
                        Sequence action = DOTween.Sequence();
                        action.Append(ReloadBubbleList[i].transform.DOLocalMove(position, 0.2f));

                        action.Play().SetEase(Ease.OutQuart).OnComplete(() =>
                        {
                            moveComplete?.Invoke();
                        });
                    }
                    else
                    {
                        Sequence action = DOTween.Sequence();
                        action.Append(ReloadBubbleList[i].transform.DOLocalMove(position, 0.2f));

                        action.Play().SetEase(Ease.OutQuart);
                    }
                }
                else
                {
                    ReloadBubbleList[i].transform.localPosition = position;

                    if (i == ReloadBubbleList.Count - 1)
                    {
                        moveComplete?.Invoke();
                    }
                }
            }
        }
        else
        {
            moveComplete?.Invoke();
        }
    }

    private void CreateNextBubble(Action createComplete = null)
    {
        if (ReloadBubbleList.Count >= 2)
        {
            createComplete?.Invoke();
            return;
        }
        ShootBubble newBubble = null;
        Vector3 position = position = ARR_POSITION[ReloadBubbleList.Count];
        newBubble = BubbleManager.CreateShootBubble(BubbleManager.GetMapShootBubble(1), position, transform).ShootBubbleComponent;
        newBubble.Init();

        ReloadBubbleList.Add(newBubble);

        newBubble.CreateAction(() =>
        {
            ResetBubblePosition(true, () =>
            {
                createComplete?.Invoke();
            });
        });
    }

    #region "버블 재장전"
    public void ReloadBubble(Action reloadCompleteCallback)
    {
        // 재장전해주는 함수
        if (isItemShoot)
        {
            isItemShoot = false;
            isShootReady = true;

            ResetBubblePosition(false);

            if (reloadCompleteCallback != null)
                reloadCompleteCallback();
        }
        else
        {
            // 슈팅준비 해제
            isShootReady = false;

            if (ReloadBubbleList.Count > 0)
            {
                ResetBubblePosition(true, () =>
                {
                    CreateNextBubble(() =>
                    {
                        isShootReady = true;
                        reloadCompleteCallback?.Invoke();
                    });
                });
            }
            else
            {
                isShootReady = true;
                reloadCompleteCallback?.Invoke();
            }
        }
    }
    #endregion

    #region "버블 스왑"
    public void SwapBubble(Action completeCallback)
    {
        // 슈팅준비 해제
        isShootReady = false;

        // 리스트맨앞의 버블을 빼서, 맨뒤에 다시 넣는다
        ShootBubble gotoBubble = ReloadBubbleList[0];
        ReloadBubbleList.RemoveAt(0);
        ReloadBubbleList.Add(gotoBubble);

        // 재구성된 리스트로 포지셔닝
        ResetBubblePosition(true, () =>
        {
            isShootReady = true;
            if (completeCallback != null)
                completeCallback();
        });
    }
    
    #endregion

    private void SetNormalShooter(bool on)
    {
        if (on)
        {
            for (int i = 0; i < ReloadBubbleList.Count; i++)
            {
                ReloadBubbleList[i].gameObject.SetActive(true);
            }

            _bubbleHide = false;
        }
        else
        {
            for (int i = 0; i < ReloadBubbleList.Count; i++)
            {
                if (_bubbleHide)
                    ReloadBubbleList[i].transform.localScale = Common.VECTOR3_ZERO;
                else
                {
                    ReloadBubbleList[i].transform.DOScale(0f, 0.3f).SetEase(Ease.OutQuint).OnComplete(() =>
                    {
                        ReloadBubbleList[i].gameObject.SetActive(false);
                    });
                }
            }

            _bubbleHide = true;
        }
    }

}
