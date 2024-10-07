using System;
using System.Collections.Generic;
using UnityEngine;
using TAGUNI;


public class PlaceInMapManager : TSingleton<PlaceInMapManager>
{
    public override void Init()
    {
        base.Init();
    }

    public void Dispose ()
    {

    }

    public void CheckEnablePlace(Transform shootTrans, eShootBubbleDirType shootDirect, IngameCollider targetCollider, ref int placedPosX, ref int placedPosY)
    {
        // 충돌체의 위치 인덱스
        int targetPosX = -1;
        int targetPosY = -1;

        // 충돌체가 맵버블일 경우, 인덱스 세팅
        MapBubble mapbubble = targetCollider.GetComponent<MapBubble>();
        if (mapbubble != null)
        {
            targetPosX = mapbubble.IndexX;
            targetPosY = mapbubble.IndexY;
        }

        // 근처에 버블이 있는지 확인을 위한 변수
        bool isAroundBubble;

        // 비교대상의 거리
        float distance_A = float.MaxValue;

        // 비교할 거리
        float distance_B;

        for (int hIndex = 0; hIndex <= MapManager.instance.GetLastBubbleIndexY() + 1; hIndex++)
        {
            for (int wIndex = 0; wIndex < MapManager.instance.BubbleMapDataField[hIndex].Length; wIndex++)
            {
                isAroundBubble = false;

                // 높이가 버블타일데이터 안에 있고, 버블데이터가 없는(혹은 올리기버블이 있는) 공간에서만 체크
                if (MapManager.instance.IsIndexInMap(wIndex, hIndex) &&
                    (MapManager.instance.BubbleMapDataField[hIndex][wIndex] == null))
                {
                    if (hIndex == 0)
                    {
                        // 맨 꼭대기 버블은 무조건 붙을 수 있다.
                        isAroundBubble = true;
                    }
                    else
                    {
                        // 주변에 붙을 수 있는 버블이 있는지 체크한다. (6방향)
                        if ((hIndex % 2) == 0)
                        {
                            // 짝수라인 체크 (11개)
                            if (BubbleManager.GetPositionBubble(wIndex - 1, hIndex))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex + 1, hIndex))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex, hIndex + 1))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex - 1, hIndex + 1))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex, hIndex - 1))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex - 1, hIndex - 1))
                                isAroundBubble = true;
                        }
                        else
                        {
                            // 홀수라인 체크 (10개)
                            if (BubbleManager.GetPositionBubble(wIndex - 1, hIndex))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex + 1, hIndex))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex, hIndex + 1))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex, hIndex - 1))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex + 1, hIndex + 1))
                                isAroundBubble = true;
                            if (BubbleManager.GetPositionBubble(wIndex + 1, hIndex - 1))
                                isAroundBubble = true;
                        }
                    }

                    if (isAroundBubble)
                    {
                        switch (shootDirect)
                        {
                            case eShootBubbleDirType.UP:
                                {
                                    //  맞닿은 버블보다는 같거나 아래 배치 되어야 한다.
                                    if (hIndex > targetPosY)
                                    {
                                        distance_B = Vector2.Distance(shootTrans.position, BubbleManager.GetBubbleMapPos(wIndex, hIndex));
                                        if (distance_A > distance_B)
                                        {
                                            distance_A = distance_B;
                                            placedPosX = wIndex;
                                            placedPosY = hIndex;
                                        }
                                    }
                                }
                                break;
                            case eShootBubbleDirType.UP_RIGHT:
                                {
                                    //  맞닿은 버블보다는 같거나 아래 배치 되어야 한다.
                                    if (hIndex >= targetPosY)
                                    {
                                        // 같은 라인에서 반대방향으로 껴들어갈 수 없다.
                                        if (!(hIndex == targetPosY && wIndex > targetPosX))
                                        {
                                            distance_B = Vector2.Distance(shootTrans.localPosition, BubbleManager.GetBubbleMapPos(wIndex, hIndex));

                                            if (distance_A > distance_B)
                                            {
                                                distance_A = distance_B;
                                                placedPosX = wIndex;
                                                placedPosY = hIndex;
                                            }
                                        }
                                    }
                                }
                                break;
                            case eShootBubbleDirType.UP_LEFT:
                                {
                                    //  맞닿은 버블보다는 같거나 아래 배치 되어야 한다.
                                    if (hIndex >= targetPosY)
                                    {
                                        // 같은 라인에서 반대방향으로 껴들어갈 수 없다.
                                        if (!(hIndex == targetPosY && wIndex < targetPosX))
                                        {
                                            distance_B = Vector2.Distance(shootTrans.localPosition, BubbleManager.GetBubbleMapPos(wIndex, hIndex));
                                            if (distance_A > distance_B)
                                            {
                                                distance_A = distance_B;
                                                placedPosX = wIndex;
                                                placedPosY = hIndex;
                                            }
                                        }
                                    }
                                }
                                break;
                            case eShootBubbleDirType.DOWN:
                                {
                                    //  맞닿은 버블보다는 같거나 위에 배치 되어야 한다.
                                    if (hIndex <= targetPosY)
                                    {
                                        distance_B = Vector2.Distance(shootTrans.position, BubbleManager.GetBubbleMapPos(wIndex, hIndex));
                                        if (distance_A > distance_B)
                                        {
                                            distance_A = distance_B;
                                            placedPosX = wIndex;
                                            placedPosY = hIndex;
                                        }
                                    }
                                }
                                break;
                            case eShootBubbleDirType.DOWN_RIGHT:
                                {
                                    //  맞닿은 버블보다는 같거나 위에 배치 되어야 한다.
                                    if (hIndex <= targetPosY)
                                    {
                                        distance_B = Vector2.Distance(shootTrans.position, BubbleManager.GetBubbleMapPos(wIndex, hIndex));
                                        if (distance_A > distance_B)
                                        {
                                            distance_A = distance_B;
                                            placedPosX = wIndex;
                                            placedPosY = hIndex;
                                        }
                                    }
                                }
                                break;
                            case eShootBubbleDirType.DOWN_LEFT:
                                {
                                    //  맞닿은 버블보다는 같거나 위에 배치 되어야 한다.
                                    if (hIndex <= targetPosY)
                                    {
                                        distance_B = Vector2.Distance(shootTrans.position, BubbleManager.GetBubbleMapPos(wIndex, hIndex));
                                        if (distance_A > distance_B)
                                        {
                                            distance_A = distance_B;
                                            placedPosX = wIndex;
                                            placedPosY = hIndex;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    public void GetPlacedPosition (BaseBubble shootBubble, IngameCollider targetCollider, ref int indexX, ref int indexY)
    {
        CheckEnablePlace(shootBubble.transform, shootBubble.ShootBubbleComponent.GetShootDirection(), targetCollider, ref indexX, ref indexY);
    }

    public void GetPlacedPosition(GuideBubble guideBubble, IngameCollider targetCollider, ref int indexX, ref int indexY)
    {
        CheckEnablePlace(guideBubble.transform, guideBubble.GetShootDirection(), targetCollider, ref indexX, ref indexY);
    }

    public void CheckPlacedBubble(BaseBubble shootBubble, IngameCollider targetCollider, Action<int, int> endCallback)
    {
        // 버블 배치해주는 함수
        int placedPosX = -1;
        int placedPosY = -1;

        GetPlacedPosition(shootBubble, targetCollider, ref placedPosX, ref placedPosY);

        // 충돌한 버블 근처에 들어갈 공간이 있다면
        if ((placedPosX != -1) && (placedPosY != -1))
        {
            PlaceBubble(shootBubble, placedPosX, placedPosY, endCallback);
        }
    }

    private void PlaceBubble(BaseBubble shootBubble, int indexX, int indexY, Action<int, int> endCallback)
    {
        // 슈팅버블 맵버블로 교체
        shootBubble.transform.parent = IngameViewManager.instance.BubbleMapCanvas.transform;
        shootBubble.transform.localPosition = BubbleManager.GetBubbleMapPos(indexX, indexY);
        shootBubble.MapBubbleComponent.UpdateMapBubbleIndex(indexX, indexY);

        MapManager.instance.AddBubbleFieldData(indexX, indexY, shootBubble);

        // 배치 연출 시작
        CheckPlacedEffectAction(shootBubble.ShootBubbleComponent.DirectRotation, indexX, indexY, () =>
        {
            if (endCallback != null)
                endCallback(indexX, indexY);
        });
    }

    public void CheckPlacedBubble(GuideBubble guideBubble, IngameCollider targetCollider)
    {
        int placedPosX = -1;
        int placedPosY = -1;

        GetPlacedPosition(guideBubble, targetCollider, ref placedPosX, ref placedPosY);

        // 충돌한 버블 근처에 들어갈 공간이 있다면
        if ((placedPosX != -1) && (placedPosY != -1))
        {
            GuideLineManager.instance.CreatePrevBubble(placedPosX, placedPosY);
        }
    }

    // 버블이 배치됐을 때, 주변버블에 작용하는 버블 탐색 및 연출호출
    private void CheckPlacedEffectAction(Vector3 shootAngle, int PosX, int PosY, Action endCallback)
    {
        BaseBubble shootBubble = MapManager.instance.BubbleMapDataField[PosY][PosX];

        if (MapManager.instance.IsIndexInMap(PosX, PosY))
        {
            int checkStartYPos = PosY - 2;
            if (checkStartYPos < MapManager.instance.GetViewTopBubblePosY())
                checkStartYPos = MapManager.instance.GetViewTopBubblePosY();

            int checkEndYPos = PosY + 2;
            if (checkEndYPos < MapManager.instance.GetLastBubbleIndexY())
                checkEndYPos = MapManager.instance.GetLastBubbleIndexY();

            int checkDeffStartXPos = -1;
            int checkDeffEndXPos = 0;
            if ((PosY % 2) != 0)
            {
                checkDeffStartXPos = 0;
                checkDeffEndXPos = 1;
            }

            for (int width = PosX - 2; width < PosX + 3; width++)
            {
                for (int height = checkStartYPos; height < checkEndYPos; height++)
                {
                    if ((//(Mathf.Abs(PosY - height) == 2 && Mathf.Abs(PosX - width) <= 1) ||                                              // 2칸 위아랫줄
                        (Mathf.Abs(PosY - height) == 1 && width >= PosX + checkDeffStartXPos && width <= PosX + checkDeffEndXPos) ||    // 1칸 위아랫줄
                        (height == PosY && Mathf.Abs(PosX - width) <= 1)))                                                               // 자기라인
                    {
                        if (MapManager.instance.IsIndexInMap(width, height) && MapManager.instance.BubbleMapDataField[height][width] != null && MapManager.instance.BubbleMapDataField[height][width] != shootBubble)
                            PlacedEffectAction(BubbleManager.GetBubbleMapPos(PosX, PosY), MapManager.instance.BubbleMapDataField[height][width]);
                    }
                }
            }

            // 슈팅버블(교체된 맵버블) 액션
            if (shootBubble != null)
                PlacedEffectAction(shootBubble, shootAngle, endCallback);
        }
        else
        {
            if (endCallback != null)
                endCallback();
        }
    }

    // 해당위치 버블에 배치연출
    private void PlacedEffectAction(Vector3 shootBubblePosition, BaseBubble targetBubble)
    {
        // 찌그러질 각도 세팅
        Vector3 forceVec = (targetBubble.transform.localPosition - shootBubblePosition);

        targetBubble.MapBubbleComponent.PlaceBubble(true, forceVec);
    }

    // 해당위치 버블에 배치연출
    private void PlacedEffectAction(BaseBubble shootBubble, Vector3 forceVec, Action callback = null)
    {
        shootBubble.MapBubbleComponent.PlaceBubble(false, forceVec, () =>
        {
            // 배치연출 끝나면 함수호출
            if (shootBubble && callback != null)
                callback();
        });
    }
}

