using System.Collections.Generic;
using TAGUNI;

public class FloatingBubbleManager : TSingleton<FloatingBubbleManager>
{
    // 최하단 버블의 인텍스Y 값
    private int _lastViewPos;


    public override void Init()
    {
        base.Init();
    }

    public void Dispose ()
    {

    }

    // 떨어지는 버블을 체크해주는 함수
    public void CheckFloatingBubble()
    {
        // 연결버블 탐색 시 사용할 변수
        bool isSameBubble = false;

        // 떨어지는버블 갯수 체크용 변수
        int floaingCount = 0;

        // 현재 최하단 버블의 Y인덱스 저장
        _lastViewPos = MapManager.instance.GetViewTopBubblePosY();

        // 연결되어있는 버블을 저장해놓을 리스트
        List<BaseBubble> nonFloatingList = new List<BaseBubble>();

        // 연결버블 체크 시작
        CheckFloatingBubbleStart(nonFloatingList);

        for (int hIndex = _lastViewPos; hIndex <= MapManager.instance.GetLastBubbleIndexY(); hIndex++)
        {
            for (int wIndex = 0; wIndex < MapManager.instance.BubbleMapDataField[hIndex].Length; wIndex++)
            {
                // 현재 보여지는 버블필드 체크
                // 그중 버블이 있는 필드 체크
                if (MapManager.instance.BubbleMapDataField[hIndex][wIndex] != null)
                {
                    // 탐색변수 초기화
                    isSameBubble = false;

                    for (int z = 0; z < nonFloatingList.Count; z++)
                    {
                        // 탐색버블이 연결되어있는 버블리스트 아이템중 하나라면
                        if ((nonFloatingList[z].MapBubbleComponent.IndexX == wIndex) &&
                            (nonFloatingList[z].MapBubbleComponent.IndexY == hIndex))
                        {
                            // 탐색변수 세팅 및 탐색 종료
                            isSameBubble = true;
                            break;
                        }
                    }

                    // 탐색버블이 연결리스트에 없다면 플로팅 시작
                    if (!isSameBubble)
                    {
                        floaingCount++;
                        MapManager.instance.BubbleMapDataField[hIndex][wIndex].MapBubbleComponent.FloatingBubble();
                    }
                }
            }
        }
    }

    // 연결되어있는 버블찾기 시작함수
    private void CheckFloatingBubbleStart(List<BaseBubble> nonFloatingList)
    {
        for (int wIndex = 0; wIndex < MapManager.instance.BubbleMapDataField[_lastViewPos].Length; wIndex++)
        {
            // 마지막 버블라인부터 가로로 탐색 시작
            if (MapManager.instance.BubbleMapDataField[_lastViewPos][wIndex] != null)
                CheckFloatingBubbles(nonFloatingList, wIndex, _lastViewPos);
        }
    }

    private void CheckFloatingBubbles(List<BaseBubble> floatingList, int posX, int posY)
    {
        // 주변버블(6방향) 탐색 시작
        if ((posY % 2) == 0)
        {
            // 짝수라인 버블
            FindFloatingBubble(floatingList, posX, posY);
            FindFloatingBubble(floatingList, posX - 1, posY);
            FindFloatingBubble(floatingList, posX + 1, posY);
            FindFloatingBubble(floatingList, posX - 1, posY + 1);
            FindFloatingBubble(floatingList, posX, posY + 1);

            if (_lastViewPos <= (posY - 1))
            {
                FindFloatingBubble(floatingList, posX - 1, posY - 1);
                FindFloatingBubble(floatingList, posX, posY - 1);
            }
        }
        else
        {
            // 홀수라인 버블
            FindFloatingBubble(floatingList, posX, posY);
            FindFloatingBubble(floatingList, posX - 1, posY);
            FindFloatingBubble(floatingList, posX + 1, posY);
            FindFloatingBubble(floatingList, posX + 1, posY + 1);
            FindFloatingBubble(floatingList, posX, posY + 1);

            if (_lastViewPos <= (posY - 1))
            {
                FindFloatingBubble(floatingList, posX + 1, posY - 1);
                FindFloatingBubble(floatingList, posX, posY - 1);
            }
        }
    }

    private void FindFloatingBubble(List<BaseBubble> floatingList, int posX, int posY)
    {
        // 연결된 버블이 타일데이터에있고, 버블이 있다면
        BaseBubble checkBubble = BubbleManager.GetPositionBubble(posX, posY);

        if (checkBubble != null)
        {
            // 연결버블리스트에 있는지 탐색
            BaseBubble result = floatingList.Find(delegate (BaseBubble bubble)
            {
                if ((bubble.MapBubbleComponent.IndexX == checkBubble.MapBubbleComponent.IndexX) &&
                    (bubble.MapBubbleComponent.IndexY == checkBubble.MapBubbleComponent.IndexY))
                    return true;
                else
                    return false;
            });

            if (result == null)
            {
                // 연결버블리스트에 없다면 추가하고, 기준버블로 다시 탐색 시작
                floatingList.Add(checkBubble);
                CheckFloatingBubbles(floatingList, posX, posY);
            }
        }
    }
}
