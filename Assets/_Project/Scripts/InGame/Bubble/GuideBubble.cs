using System;
using System.Collections.Generic;
using UnityEngine;


public class GuideBubble : MonoBehaviour, IAddressablePoolHandler
{
    // 슈팅될 각도
    private Vector3 _directRotation;

    private bool isCheck;

    private int _collisionCount;

    private List<IngameCollider> _penetrateBubbleList = new List<IngameCollider>();

    public void Init()
    {
        ResetGuide();
    }

    public void Dispose()
    {

    }

    public eShootBubbleDirType GetShootDirection()
    {
        eShootBubbleDirType result = eShootBubbleDirType.NONE;

        if (_directRotation.y > 0)
        {
            if (_directRotation.x == 0)
                result = eShootBubbleDirType.UP;
            else if (_directRotation.x > 0)
                result = eShootBubbleDirType.UP_RIGHT;
            else if (_directRotation.x < 0)
                result = eShootBubbleDirType.UP_LEFT;
        }
        else if (_directRotation.y < 0)
        {
            if (_directRotation.x == 0)
                result = eShootBubbleDirType.DOWN;
            else if (_directRotation.x > 0)
                result = eShootBubbleDirType.DOWN_RIGHT;
            else if (_directRotation.x < 0)
                result = eShootBubbleDirType.DOWN_LEFT;
        }

        return result;
    }

    public void StartCheck()
    {
        _collisionCount = 0;
        isCheck = true;

        _penetrateBubbleList.Clear();

        // 슈터에 세팅된 각도 저장
        float shootAngle = ShootControlManager.instance.ShootAngleValue;

        // 슈터각도로 버블 슈팅각도 세팅
        _directRotation = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * (shootAngle)), Mathf.Cos(Mathf.Deg2Rad * (shootAngle)), 0f);

        // 슛버블의 부모트랜스폼을 슈터->맵 으로 변경
        transform.parent = IngameViewManager.instance.BubbleMapCanvas.transform;

        while (isCheck)
            Shooting();
    }

    public void ResetGuide()
    {
        isCheck = false;

        transform.parent = IngameViewManager.instance.BubbleShooter.transform;
        transform.localPosition = BubbleShooter.SHOOT_POSITION_VECTOR;
    }

    private void Shooting()
    {
        ShootLogic.BounceViewLogic(transform, (bounceType)=>
        {
            float collisionWidth = Common.SHOOT_BUBBLE_DIAMETER;

            switch (bounceType)
            {
                case ShootLogic.eBounceType.LEFT_BOUNCE:
                    {
                        // 가이드라인
                        _collisionCount++;
                        GuideLineManager.instance.AddGuideLinePosition(_collisionCount, transform.position);

                        // 앵글 변환
                        if (_directRotation.x < 0)
                            _directRotation.x *= -1;

                        ShootLogic.MoveLogic(false, transform, _directRotation, collisionWidth, (colliderList, realPos) =>
                        {
                            if (colliderList.Count > 0)
                            {
                                ShootEnd(colliderList[0]);
                            }

                            _collisionCount++;
                            GuideLineManager.instance.AddGuideLinePosition(_collisionCount, realPos, true);
                        });
                    }
                    break;
                case ShootLogic.eBounceType.RIGHT_BOUNCE:
                    {
                        _collisionCount++;
                        GuideLineManager.instance.AddGuideLinePosition(_collisionCount, transform.position);

                        // 앵글 변환
                        if (_directRotation.x > 0)
                            _directRotation.x *= -1;

                        ShootLogic.MoveLogic(false, transform, _directRotation, collisionWidth, (colliderList, realPos) =>
                        {
                            if (colliderList.Count > 0)
                            {
                                ShootEnd(colliderList[0]);
                            }

                            _collisionCount++;
                            GuideLineManager.instance.AddGuideLinePosition(_collisionCount, realPos, true);
                        });
                    }
                    break;
                case ShootLogic.eBounceType.TOP_OUT:
                    {
                        ResetGuide();
                    }
                    break;
                case ShootLogic.eBounceType.NONE:
                    {
                        ShootLogic.MoveLogic(false, transform, _directRotation, collisionWidth, (colliderList, realPos) =>
                        {
                            if (colliderList.Count > 0)
                            {
                                ShootEnd(colliderList[0]);
                            }

                            _collisionCount++;
                            GuideLineManager.instance.AddGuideLinePosition(_collisionCount, realPos, true);
                        });
                    }
                    break;
            }
        });
    }

    private void ShootEnd(IngameCollider collisionObject)
    {
        CollisionManager.instance.CheckShootBubbleCollision(null, this, collisionObject, null);
        ResetGuide();
    }

    private void AddPenetrateBubble(List<IngameCollider> collisionList)
    {
        for (int i = 0; i < collisionList.Count; i++)
        {
            if (!_penetrateBubbleList.Contains(collisionList[i]))
                _penetrateBubbleList.Add(collisionList[i]);
        }
    }

    #region PoolHandler

    public void ReturnToPool()
    {
    }

    public void DestroyToPool()
    {

    }

    public GameObject poolObject => gameObject;

    #endregion
}
