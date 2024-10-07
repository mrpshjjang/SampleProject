using System;
using System.Collections.Generic;
using UnityEngine;

public static class ShootLogic
{
    public enum eBounceType
    {
        NONE,
        LEFT_BOUNCE,
        RIGHT_BOUNCE,
        TOP_OUT,
    }

    public enum eCollisionType
    {
        NONE,
        MOVE,
        INGAME_COLLISION,
    }

    private static Vector3 _movePower;
    private static Vector3 _beforePosition;

    public static void BounceViewLogic (Transform shootObjectTrans, Action<eBounceType> bounceCallback)
    {
        float leftPosition = IngameViewManager.instance.LeftLineTrans.position.x + Common.BUBBLE_DIAMETER / 4;
        float rightPosition = IngameViewManager.instance.RightLineTrans.position.x - Common.BUBBLE_DIAMETER / 4;

        if (shootObjectTrans.localPosition.x <= leftPosition)
        {
            shootObjectTrans.localPosition = new Vector3(leftPosition, shootObjectTrans.localPosition.y);

            if (bounceCallback != null)
                bounceCallback(eBounceType.LEFT_BOUNCE);
        }
        else if (shootObjectTrans.localPosition.x >= rightPosition)
        {
            shootObjectTrans.localPosition = new Vector3(rightPosition, shootObjectTrans.localPosition.y);

            if (bounceCallback != null)
                bounceCallback(eBounceType.RIGHT_BOUNCE);
        }
        else if (shootObjectTrans.localPosition.y >= -(MapManager.instance.GetViewTopBubblePosY() - 1) * Common.BUBBLE_HEIGHT_GAP_SIZE)
        {
            if (bounceCallback != null)
                bounceCallback(eBounceType.TOP_OUT);
        }
        else
        {
            if (bounceCallback != null)
                bounceCallback(eBounceType.NONE);
        }
    }

    public static void MoveLogic(bool isPenetrate, Transform shootObjectTrans, Vector3 directRotation, float collisionWidth, Action<List<IngameCollider>, Vector3> moveCallback)
    {
        // 충돌체크를 위해서 이전 위치 저장하기
        _beforePosition = shootObjectTrans.position;

        // 이동력(update 한번에 움직이는 양)
        _movePower = directRotation * 45f;

        RaycastHit2D[] hits = Physics2D.CapsuleCastAll(Vector2.Lerp(_beforePosition, _beforePosition + _movePower, 0.5f),
                                                       new Vector2(collisionWidth, Vector2.Distance(_beforePosition, _beforePosition + _movePower)),
                                                       CapsuleDirection2D.Vertical, directRotation.z, Vector2.zero);

        Vector3 realPos = shootObjectTrans.position;
        List<IngameCollider> colliderList = new List<IngameCollider>();

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                var collider = hits[i].collider.gameObject.GetComponent<IngameCollider>();
                if (collider != null)
                    colliderList.Add(collider);
            }
        }

        if (isPenetrate)
            shootObjectTrans.Translate(_movePower);

        if (colliderList.Count > 0)
        {
            // 슛날린 버블과 충돌체 사이의 80% 거리만큼 이동
            if (!isPenetrate && colliderList[0].ThisTransform != null && shootObjectTrans != null)
            {
                Vector2 movePos = Vector2.Lerp(shootObjectTrans.position, colliderList[0].ThisTransform.position, 0.8f);
                shootObjectTrans.position = new Vector3(movePos.x, movePos.y, shootObjectTrans.position.z);
            }

            if (moveCallback != null)
                moveCallback(colliderList, realPos);
        }
        else if (!isPenetrate)
            shootObjectTrans.Translate(_movePower);
    }
}
