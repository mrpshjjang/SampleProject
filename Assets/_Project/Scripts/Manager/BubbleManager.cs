using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public static class BubbleManager
{
    public static Vector3 GetBubbleMapPos(int posX, int posY)
    {
        // 반환 포지션 값
        Vector3 resultPosition;

        // Y좌표에 따라서 X좌표 시작점에 변화를 줄 옵션값
        float optionValue = 0f;
        if (posY % 2 != 0)
            optionValue = Common.BUBBLE_DIAMETER * 0.5f;

        // 시작 X좌표 세팅
        float firstPosX = Common.BUBBLE_DIAMETER * 0.5f - Common.BUBBLE_DIAMETER * Common.BUBBLE_WIDTH_COUNT * 0.5f + optionValue;

        // 최종 포지션 세팅
        resultPosition = new Vector3(firstPosX + (Common.BUBBLE_DIAMETER * posX), -Common.BUBBLE_DIAMETER * 0.5f - (Common.BUBBLE_HEIGHT_GAP_SIZE * posY), 0f);

        return resultPosition;
    }

    public static BaseBubble CreateBubble(SpecBubble spec, int idxX, int idxY, Transform parentTrans = null)
    {
        ObjectPoolManager.Instance.LoadObjectPool(eObjPoolType.Ingame, spec.prefab_name);
        BaseBubble bubble = null;

        switch (spec.type)
        {
            case eBubbleType.NORMAL:
                bubble = ObjectPoolManager.Instance.GetPoolObject<NormalBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingBubble(bubble, idxX, idxY, parentTrans);
                ((NormalBubble)bubble).Init(spec, idxX, idxY);
                break;

            case eBubbleType.BOMB:
                bubble = ObjectPoolManager.Instance.GetPoolObject<BombBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingBubble(bubble, idxX, idxY, parentTrans);
                ((BombBubble)bubble).Init(spec, idxX, idxY);
                break;

            case eBubbleType.BOSS:
                bubble = ObjectPoolManager.Instance.GetPoolObject<BossBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingBubble(bubble, idxX, idxY, parentTrans);
                ((BossBubble)bubble).Init(spec, idxX, idxY);
                break;

            case eBubbleType.BUBBLE_GENERATOR:
                bubble = ObjectPoolManager.Instance.GetPoolObject<BubbleGeneratorBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingBubble(bubble, idxX, idxY, parentTrans);
                ((BubbleGeneratorBubble)bubble).Init(spec, idxX, idxY);
                break;
        }

        return bubble;
    }

    public static void ReturnBubble(SpecBubble spec, BaseBubble bubble, Transform parentTrans = null)
    {
        switch (spec.type)
        {
            case eBubbleType.NORMAL:
                ObjectPoolManager.Instance.ReturnPoolObject<NormalBubble>(eObjPoolType.Ingame, spec.prefab_name, (NormalBubble)bubble);
                break;

            case eBubbleType.BOMB:
                ObjectPoolManager.Instance.ReturnPoolObject<BombBubble>(eObjPoolType.Ingame, spec.prefab_name, (BombBubble)bubble);
                break;

            case eBubbleType.BOSS:
                ObjectPoolManager.Instance.ReturnPoolObject<BossBubble>(eObjPoolType.Ingame, spec.prefab_name, (BossBubble)bubble);
                break;

            case eBubbleType.BUBBLE_GENERATOR:
                ObjectPoolManager.Instance.ReturnPoolObject<BubbleGeneratorBubble>(eObjPoolType.Ingame, spec.prefab_name, (BubbleGeneratorBubble)bubble);
                break;
        }
    }

    public static BaseBubble CreateShootBubble(SpecBubble spec, Vector3 pos, Transform parentTrans = null)
    {
        ObjectPoolManager.Instance.LoadObjectPool(eObjPoolType.Ingame, spec.prefab_name);
        BaseBubble bubble = null;

        switch (spec.type)
        {
            case eBubbleType.NORMAL:
                bubble = ObjectPoolManager.Instance.GetPoolObject<NormalBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingShootBubble(bubble, pos, parentTrans);
                ((NormalBubble)bubble).Init(spec);
                break;

            case eBubbleType.BOMB:
                bubble = ObjectPoolManager.Instance.GetPoolObject<BombBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingShootBubble(bubble, pos, parentTrans);
                ((BombBubble)bubble).Init(spec);
                break;

            case eBubbleType.BOSS:
                bubble = ObjectPoolManager.Instance.GetPoolObject<BossBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingShootBubble(bubble, pos, parentTrans);
                ((BossBubble)bubble).Init(spec);
                break;

            case eBubbleType.BUBBLE_GENERATOR:
                bubble = ObjectPoolManager.Instance.GetPoolObject<BubbleGeneratorBubble>(eObjPoolType.Ingame, spec.prefab_name);
                SettingShootBubble(bubble, pos, parentTrans);
                ((BubbleGeneratorBubble)bubble).Init(spec);
                break;
        }

        return bubble;
    }

    public static void SettingBubble(BaseBubble bubble, int idxX, int idxY, Transform parentTrans = null)
    {
        bubble.SetActive(true);
        if(parentTrans != null)
            bubble.transform.SetParent(parentTrans);
        bubble.transform.localPosition = BubbleManager.GetBubbleMapPos(idxX, idxY);
        bubble.transform.localScale = Common.VECTOR3_ONE;
    }

    public static void MoveBubble(BaseBubble bubble, int idxX, int idxY)
    {
        bubble.MoveBubble(GetBubbleMapPos(idxX, idxY));
        //bubble.transform.localPosition = GetBubbleMapPos(idxX, idxY);
    }

    public static void SettingShootBubble(BaseBubble bubble, Vector3 pos, Transform parentTrans = null)
    {
        bubble.SetActive(true);
        if(parentTrans != null)
            bubble.transform.SetParent(parentTrans);
        bubble.transform.localPosition = pos;
        bubble.transform.localScale = Common.VECTOR3_ONE;
    }

    public static BaseBubble GetPositionBubble(int indexX, int indexY)
    {
        if (MapManager.instance.IsIndexInMap(indexX, indexY) &&
            MapManager.instance.BubbleMapDataField[indexY][indexX] != null)
        {
            return MapManager.instance.BubbleMapDataField[indexY][indexX];
        }

        return null;
    }

    public static List<BaseBubble> GetNearBubbleList(int checkPosX, int checkPosY, bool getEmpty = false)
    {
        List<BaseBubble> resultList = new List<BaseBubble>();

        BaseBubble leftBubble = GetPositionBubble(checkPosX -1, checkPosY);
        if (leftBubble != null)
        {
            resultList.Add(leftBubble);
        }

        BaseBubble rightBubble = GetPositionBubble(checkPosX + 1, checkPosY);
        if (rightBubble != null)
        {
            resultList.Add(rightBubble);
        }

        BaseBubble upperBubble = GetPositionBubble(checkPosX, checkPosY + 1);
        if (upperBubble != null)
        {
            resultList.Add(upperBubble);
        }

        BaseBubble underBubble = GetPositionBubble(checkPosX, checkPosY - 1);
        if (underBubble != null)
        {
            resultList.Add(underBubble);
        }

        if (checkPosY % 2 == 0)
        {
            BaseBubble upperLeftBubble = GetPositionBubble(checkPosX - 1, checkPosY + 1);
            if (upperLeftBubble != null)
            {
                resultList.Add(upperLeftBubble);
            }

            BaseBubble underLeftBubble = GetPositionBubble(checkPosX - 1, checkPosY - 1);
            if (underLeftBubble != null)
            {
                resultList.Add(underLeftBubble);
            }
        }
        else
        {
            BaseBubble upperRightBubble = GetPositionBubble(checkPosX + 1, checkPosY + 1);
            if (upperRightBubble != null)
            {
                resultList.Add(upperRightBubble);
            }

            BaseBubble underRightBubble = GetPositionBubble(checkPosX + 1, checkPosY - 1);
            if (underRightBubble != null)
            {
                resultList.Add(underRightBubble);
            }
        }

        return resultList;
    }

    public static SpecBubble GetMapShootBubble(int level)
    {
        var specShootBubble = Facade.Spec.Data.SpecMapShootBubble.Find(e => e.level == level);
        var picBubble = Utils.RandomPick(specShootBubble.generator_list.ToList(), specShootBubble.generator_prob_list.ToList(), specShootBubble.generator_prob_list.Sum());
        var specBubble = Facade.Spec.Data.SpecBubble.Find(e => e.id == picBubble);

        return specBubble;
    }
}
