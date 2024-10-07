using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using TAGUNI;

public class MapManager : TSingleton<MapManager>
{
    // 버블필드 2차원배열 데이터
    public List<BaseBubble[]> BubbleMapDataField { get; private set; }
    public int Level { get; private set; }

    public void Init()
    {
        if (BubbleMapDataField == null)
        {
            BubbleMapDataField = new List<BaseBubble[]>();
        }
        else
        {
            ResetMapDataField();
        }
    }

    public void Dispose()
    {
        ResetMapDataField();
    }

    private void ResetMapDataField()
    {
        for (int i = 0; i < BubbleMapDataField.Count; i++)
        {
            for (int w = 0; w < BubbleMapDataField[i].Length; w++)
            {
                if (BubbleMapDataField[i][w] != null)
                    BubbleMapDataField[i][w].DestroyBubble();
            }
        }
        BubbleMapDataField.Clear();
    }

    private int GetMapDataWidthCount(int indexY)
    {
        return (indexY % 2 != 0 ? Common.BUBBLE_WIDTH_COUNT - 1 : Common.BUBBLE_WIDTH_COUNT);
    }

    public void SetMapBubble(int level)
    {
        Level = level;
        for (int dataCreateIndex = 0; dataCreateIndex < 10; ++dataCreateIndex)
            BubbleMapDataField.Add(new BaseBubble[GetMapDataWidthCount(dataCreateIndex)]);

        var bubbles = Facade.Spec.Data.SpecMap.FindAll(e => e.level == Level);
        foreach (SpecMap specMap in bubbles)
        {
            MakeBubble(specMap.bubble, specMap.pos[0], specMap.pos[1]);
        }
    }

    public BaseBubble MakeBubble(int bubbleID, int posX, int posY)
    {
        var specBubble = Facade.Spec.Data.SpecBubble.Find(e => e.id == bubbleID);
        BaseBubble createBubble = BubbleManager.CreateBubble(specBubble, posX, posY, IngameViewManager.instance.BubbleCanvas.transform);
        createBubble.MapBubbleComponent.UpdateMapBubbleIndex(posX, posY);

        if (createBubble != null)
            BubbleMapDataField[posY][posX] = createBubble;

        return createBubble;
    }

    public void MoveBubble(BaseBubble bubble, int posX, int posY)
    {
        BubbleManager.MoveBubble(bubble, posX, posY);
        bubble.MapBubbleComponent.UpdateMapBubbleIndex(posX, posY);
        BubbleMapDataField[posY][posX] = bubble;
    }

    public int GetViewTopBubblePosY()
    {
        return 0;
    }

    public int GetLastBubbleIndexY()
    {
        // 최하단 버블의 Y인덱스를 구하는 함수
        for (int heightIndex = BubbleMapDataField.Count -1; heightIndex >= 0; heightIndex--)
        {
            for (int widthIndex = 0; widthIndex < BubbleMapDataField[heightIndex].Length; widthIndex++)
            {
                // 맨 아래서부터 버블 한개라도 있는 Y위치를 리턴
                if (BubbleMapDataField[heightIndex][widthIndex] != null)
                    return heightIndex;
            }
        }

        // 맵에 버블이 한개도 없으면 0 반환
        return 0;
    }

    public bool IsIndexInMap(int posX, int posY)
    {
        // 해당 인덱스가 맵필드데이터에 존재하는지 확인하는 함수
        return (posY >= 0 && posX >= 0 && posY < BubbleMapDataField.Count && posX < BubbleMapDataField[posY].Length);
    }

    public bool AddBubbleFieldData (int posX, int posY, BaseBubble addBubble)
    {
        // 맵 필드데이터에 해당인덱스의 버블을 추가하는 함수
        if (IsIndexInMap(posX, posY) && BubbleMapDataField[posY][posX] == null)
        {
            BubbleMapDataField[posY][posX] = addBubble;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveBubbleFieldData(int posX, int posY)
    {
        // 맵 필드데이터에 해당인덱스의 버블을 삭제하는 함수
        if (IsIndexInMap(posX, posY) && BubbleMapDataField[posY][posX] != null)
        {
            BubbleMapDataField[posY][posX] = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    public async UniTask BubbleAction()
    {
        for (int heightIndex = BubbleMapDataField.Count -1; heightIndex >= 0; heightIndex--)
        {
            for (int widthIndex = 0; widthIndex < BubbleMapDataField[heightIndex].Length; widthIndex++)
            {
                var bubble = BubbleMapDataField[heightIndex][widthIndex];
                if (bubble != null)
                {
                    switch (bubble.Spec.type)
                    {
                        case eBubbleType.BUBBLE_GENERATOR:
                            ((BubbleGeneratorBubble)bubble).CheckGenerator();
                            break;
                    }
                }
            }
        }
        
        await UniTask.WaitUntil(AllEndAction);
    }

    private bool AllEndAction()
    {
        bool end = true;
        for (int heightIndex = BubbleMapDataField.Count - 1; heightIndex >= 0; heightIndex--)
        {
            for (int widthIndex = 0; widthIndex < BubbleMapDataField[heightIndex].Length; widthIndex++)
            {
                var bubble = BubbleMapDataField[heightIndex][widthIndex];
                if (bubble != null && bubble.ActionIng)
                {
                    end = false;
                    break;
                }
            }
        }

        return end;
    }
}
