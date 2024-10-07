using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ShootBubble : MonoBehaviour
{
    // 충돌체
    public CircleCollider2D Collider { get; private set; }

    // 슈팅될 각도
    private Vector3 _directRotation;
    public Vector3 DirectRotation { get { return _directRotation; } }

    // 슛버블 상태
    public eShootBubbleStatusType _shootBubbleState { get; private set; }

    public BaseBubble BubbleComponent { get; private set; } = null;

    public void Init()
    {
        if (BubbleComponent == null)
            BubbleComponent = GetComponent<BaseBubble>();

        if (Collider == null)
            Collider = gameObject.GetComponent<CircleCollider2D>();

        // 상태초기화
        _shootBubbleState = eShootBubbleStatusType.NONE;

        Collider.enabled = false;
    }

    public void Dispose()
    {
    }

    private void FixedUpdate()
    {
        if (_shootBubbleState == eShootBubbleStatusType.MOVING)
            Shooting();
    }

    public eBubbleType GetBubbleType()
    {
        if (BubbleComponent != null)
            return BubbleComponent.Spec.type;
        else
            return eBubbleType.NONE;
    }

    public void StartShoot(float shootAngle)
    {
        //ShooterIdleGO.SetActive(false);

        // 슈터각도로 버블 슈팅각도 세팅
        _directRotation = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle), 0f);

        // 슛버블의 부모트랜스폼을 슈터->맵 으로 변경
        transform.parent = IngameViewManager.instance.BubbleMapCanvas.transform;

        // 슈팅상태로 상태변경
        _shootBubbleState = eShootBubbleStatusType.MOVING;
    }

    private void Shooting()
    {
        ShootLogic.BounceViewLogic(transform, (ShootLogic.eBounceType bounceType) =>
        {
            float collisionWidth = Common.SHOOT_BUBBLE_DIAMETER;

            switch (bounceType)
            {
                case ShootLogic.eBounceType.LEFT_BOUNCE:
                    {
                        if (DirectRotation.x < 0)
                        {
                            // 앵글 변환
                            _directRotation.x *= -1;
                        }

                        ShootLogic.MoveLogic(false, transform, DirectRotation, collisionWidth, (colliderList, realPos) =>
                        {
                            if (colliderList.Count > 0)
                            {
                                ShootEnd(colliderList[0]);
                            }
                        });
                    }
                    break;
                case ShootLogic.eBounceType.RIGHT_BOUNCE:
                    {
                        if (DirectRotation.x > 0)
                        {
                            // 앵글 변환
                            _directRotation.x *= -1;
                        }

                        ShootLogic.MoveLogic(false, transform, DirectRotation, collisionWidth, (colliderList, realPos) =>
                        {
                            if (colliderList.Count > 0)
                            {
                                ShootEnd(colliderList[0]);
                            }
                        });
                    }
                    break;
                case ShootLogic.eBounceType.TOP_OUT:
                    {
                        _shootBubbleState = eShootBubbleStatusType.MOVE_END;
                        //BubbleComponenet.Dispose();
                        //IngameManager.instance.StartBubbleTurn();
                    }
                    break;
                case ShootLogic.eBounceType.NONE:
                    {
                        ShootLogic.MoveLogic(false, transform, DirectRotation, collisionWidth, (colliderList, realPos) =>
                        {
                            if (colliderList.Count > 0)
                            {
                                ShootEnd(colliderList[0]);
                            }
                        });
                    }
                    break;
            }
        });
    }

    private void ShootEnd(IngameCollider collisionObject)
    {
        // 슈팅버블상태 변경:슈팅종료
        _shootBubbleState = eShootBubbleStatusType.MOVE_END;
        // 충돌처리에 따른 로직구현
        IngameManager.instance.StartCheckShootResult(BubbleComponent, collisionObject);
    }

    public eShootBubbleDirType GetShootDirection()
    {
        eShootBubbleDirType result = eShootBubbleDirType.NONE;

        if (DirectRotation.y > 0)
        {
            if (DirectRotation.x == 0)
                result = eShootBubbleDirType.UP;
            else if (DirectRotation.x > 0)
                result = eShootBubbleDirType.UP_RIGHT;
            else if (DirectRotation.x < 0)
                result = eShootBubbleDirType.UP_LEFT;
        }
        else if (DirectRotation.y < 0)
        {
            if (DirectRotation.x == 0)
                result = eShootBubbleDirType.DOWN;
            else if (DirectRotation.x > 0)
                result = eShootBubbleDirType.DOWN_RIGHT;
            else if (DirectRotation.x < 0)
                result = eShootBubbleDirType.DOWN_LEFT;
        }

        return result;
    }

    public void CreateAction(Action createComplete)
    {
        var localScale = transform.localScale;
        transform.localScale = Common.VECTOR3_ZERO;
        transform.DOScale(localScale, 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            //ShooterIdleGO.SetActive(true);

            createComplete?.Invoke();
        });
    }
}
