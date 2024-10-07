using TAGUNI;
using System;
using UnityEngine;
using System.Collections.Generic;

public class CollisionManager : TSingleton<CollisionManager>
{
    public Vector3 ShootBubblePosition { get; private set; }
    public List<IngameCollider> PenetrateBubbles { get; private set; } = new List<IngameCollider>();

    public override void Init()
    {
        base.Init();

        ShootBubblePosition = Common.VECTOR3_ZERO;

        PenetrateBubbles.Clear();
    }

    public void Dispose ()
    {

    }

    public void CheckShootBubbleCollision(BaseBubble shootBubble, GuideBubble guideBubble, IngameCollider inGameCollider, Action endCallback)
    {
        // 버블슈팅 완료 및 체크시작
        bool isGuide = guideBubble != null;

        eBubbleType checkBubbleType = eBubbleType.NONE;
        if (isGuide)
            checkBubbleType = IngameViewManager.instance.BubbleShooter.ShootReadyBubbleType();
        else
            checkBubbleType = shootBubble.Spec.type;

        // 버블타입을 위해 충돌체를 맵버블로 캐스팅
        MapBubble mapBubble = inGameCollider.GetComponent<MapBubble>();

        if (checkBubbleType == eBubbleType.BOMB)
        {
            int posX = -1;
            int posY = -1;

            // 슛버블이 아니면 탑프레임
            if (mapBubble != null)
            {
                posX = mapBubble.IndexX;
                posY = mapBubble.IndexY;
            }
            else
            {
                if (isGuide)
                    PlaceInMapManager.instance.CheckEnablePlace(guideBubble.transform, guideBubble.GetShootDirection(), inGameCollider, ref posX, ref posY);
                else
                    PlaceInMapManager.instance.CheckEnablePlace(shootBubble.transform, shootBubble.ShootBubbleComponent.GetShootDirection(), inGameCollider, ref posX, ref posY);
            }

            if (shootBubble != null)
            {
                switch (shootBubble.Spec.type)
                {
                    case eBubbleType.BOMB:
                        //shootBubble.DestroyBubble(eDestroyType.BOMB_ITEM);
                        break;
                }
            }
        }
        else
        {
            if (mapBubble != null)
            {
                if (!isGuide)
                    shootBubble.gameObject.SetActive(false);

                mapBubble.CrashShootBubble(checkBubbleType, isGuide, (bool place) =>
                {
                    if (place)
                    {
                        if (isGuide)
                        {
                            PlaceInMapManager.instance.CheckPlacedBubble(guideBubble, inGameCollider);
                            if (endCallback != null)
                                endCallback();
                        }
                        else
                        {
                            if (!isGuide)
                                shootBubble.gameObject.SetActive(true);

                            CollisionShootBubblePlace(shootBubble, inGameCollider, () =>
                            {
                                if (endCallback != null)
                                    endCallback();
                            });
                        }
                    }
                    else
                    {
                        if (endCallback != null)
                            endCallback();
                    }
                });
            }
            else
            {
                if (isGuide)
                {
                    PlaceInMapManager.instance.CheckPlacedBubble(guideBubble, inGameCollider);
                    if (endCallback != null)
                        endCallback();
                }
                else
                {
                    CollisionShootBubblePlace(shootBubble, inGameCollider, () =>
                    {
                        if (endCallback != null)
                            endCallback();
                    });
                }
            }
        }
    }

    private void CollisionShootBubblePlace(BaseBubble shootBubble, IngameCollider ingameCollider, Action endCallback)
    {
        ShootBubblePosition = shootBubble.transform.position;

        // 슛버블타입을 맵에 배치
        PlaceInMapManager.instance.CheckPlacedBubble(shootBubble, ingameCollider, (plcePosX, placePosY) =>
        {
            // 배치연출이 끝나면 연결되어있는 버블 체크시작
            ClusterManager.instance.StartShootClusterCheck(plcePosX, placePosY, () =>
            {
                if (endCallback != null)
                    endCallback();
            });
        });
    }

}
