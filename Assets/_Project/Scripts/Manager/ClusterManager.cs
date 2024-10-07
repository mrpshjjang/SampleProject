using System;
using System.Collections.Generic;
using TAGUNI;

public class ClusterManager : TSingleton<ClusterManager>
{
    // 최하단 버블의 인텍스Y 값
    private int _lastViewPos;

    public BaseBubble MainCheckBubble { get; private set; }
    public int ClusteringID { get; private set; }

    private List<BaseBubble> _finalProcessingBubbleList = new List<BaseBubble>();

    private bool isClustering;

    public override void Init()
    {
        base.Init();
    }

    public void Dispose ()
    {
    }

    public void StartShootClusterCheck(int posX, int posY, Action endCallback)
    {
        MainCheckBubble = BubbleManager.GetPositionBubble(posX, posY);

        if (MainCheckBubble == null)
            return;

        ClusteringID = MainCheckBubble.Spec.id;

        // 콤보체크 초기화
        isClustering = false;

        _finalProcessingBubbleList.Clear();

        List<BaseBubble> firstProcessingBubbleList = new List<BaseBubble>();
        List<BaseBubble> nearBubbleList = BubbleManager.GetNearBubbleList(posX, posY);

        StartShootBubbleCluster(posX, posY, endCallback);
    }

    private void StartShootBubbleCluster(int posX, int posY, Action endCallback)
    {
        List<BaseBubble> clusterBubbleList = new List<BaseBubble>();

        if (!CheckCluster(clusterBubbleList, posX, posY))
            clusterBubbleList.Clear();

        ClusterCheckEnd(clusterBubbleList, true, endCallback);
    }

    private void ClusterCheckEnd(List<BaseBubble> clusterBubbleList, bool isDelay, Action endCallback)
    {
        if (clusterBubbleList.Count > 0)
        {
            int mainTargetPosX = -1;
            int mainTargetPosY = -1;
            if (isDelay)
            {
                mainTargetPosX = clusterBubbleList[0].GetComponent<MapBubble>().IndexX;
                mainTargetPosY = clusterBubbleList[0].GetComponent<MapBubble>().IndexY;
            }

            DestroyBubbleManager.instance.DestroyBubbles(clusterBubbleList, mainTargetPosX, mainTargetPosY, (bool sResult) =>
            {
                endCallback?.Invoke();
            });
        }
        else
        {
            endCallback?.Invoke();
        }
    }

    private bool CheckCluster(List<BaseBubble> clusteringList, int posX, int posY)
    {
        // 현재 최하단 버블의 Y인덱스 저장
        _lastViewPos = MapManager.instance.GetViewTopBubblePosY();

        clusteringList.Add(BubbleManager.GetPositionBubble(posX, posY));

        // 클러스터 체크 시작
        LoopClusterCheck(clusteringList, posX, posY);

        // 클러스터링체크가 끝나면 클러스터링된 버블의 갯수에 따라 처리함
        if (clusteringList.Count >= Common.BUBBLE_COLLEPSE_COUNT)
        {
            // 클러스터링갯수가 기준갯수를 넘어갔을 경우
            return true;
        }
        else
        {
            return false;
        }
    }

    private void LoopClusterCheck(List<BaseBubble> clusteringList, int posX, int posY)
    {
        BaseBubble targetBubble = BubbleManager.GetPositionBubble(posX, posY);

        if (targetBubble != null)
            CheckClusterPosition(clusteringList, posX, posY);
    }

    // 타입별로 클러스터링 시작 함수
    private void CheckClusterPosition(List<BaseBubble> clusteringList, int posX, int posY)
    {
        // 해당버블의 주변(6방향) 버블들의 타입을 체크

        if ((posY % 2) == 0)
        {
            // 짝수라인
            FindCluster(clusteringList, posX - 1, posY);
            FindCluster(clusteringList, posX + 1, posY);
            FindCluster(clusteringList, posX - 1, posY + 1);
            FindCluster(clusteringList, posX, posY + 1);

            if (_lastViewPos <= (posY - 1))
            {
                FindCluster(clusteringList, posX - 1, posY - 1);
                FindCluster(clusteringList, posX, posY - 1);
            }
        }
        else
        {
            // 홀수라인
            FindCluster(clusteringList, posX - 1, posY);
            FindCluster(clusteringList, posX + 1, posY);
            FindCluster(clusteringList, posX + 1, posY + 1);
            FindCluster(clusteringList, posX, posY + 1);

            if (_lastViewPos <= (posY - 1))
            {
                FindCluster(clusteringList, posX + 1, posY - 1);
                FindCluster(clusteringList, posX, posY - 1);
            }
        }
    }

    // 클러스터링타입에 따라서 클러스터링 리스트에 버블을 추가해주는 함수
    private void FindCluster(List<BaseBubble> clusteringList, int posX, int posY)
    {
        // 해당 타겟버블이 타일맵에 있고, 버블이 있고, 타입이 같은지 확인 = 클러스터버블인지 확인
        BaseBubble targetBubble = BubbleManager.GetPositionBubble(posX, posY);

        if (targetBubble != null)
        {
            if (targetBubble.Spec.id == ClusteringID)
            {
                // 타입이 같은 경우
                // 해당 타겟버블이 클러스터링 리스트에 이미추가되어있는지 체크
                BaseBubble result = clusteringList.Find(delegate (BaseBubble bubble)
                {
                    if ((bubble.MapBubbleComponent.IndexX == targetBubble.MapBubbleComponent.IndexX) &&
                        (bubble.MapBubbleComponent.IndexY == targetBubble.MapBubbleComponent.IndexY))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

                if (result == null)
                {
                    // 리스트에 없는 버블이라면
                    // 리스트에 버블을 추가
                    clusteringList.Add(targetBubble);
                    // 탐색버블을 기준으로 다시 클러스터링 체크 시작
                    LoopClusterCheck(clusteringList, posX, posY);
                }
            }
        }
    }
}
