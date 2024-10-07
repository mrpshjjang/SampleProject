using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class BubbleGeneratorBubble : BaseBubble
{
    #region public

    public override void Init(SpecBubble spec, int idxX, int idxY)
    {
        base.Init(spec, idxX, idxY);

        _specGenerator = Facade.Spec.Data.SpecBubbleGenerator.FindAll(e => e.group_id == MainAttribute.value);
    }

    public async void CheckGenerator()
    {
        //StartCoroutine(GeneratedDelay(0.2f));
        await Generated(0.2f);
    }

    #endregion

    #region protected

    #endregion

    #region private

    private List<SpecBubbleGenerator> _specGenerator;
    private List<BaseBubble> _makeList = new List<BaseBubble>();

    private IEnumerator GeneratedDelay(float delay)
    {
        int makeCnt = CheckGeneratedCnt();
        _makeList.Clear();
        CheckMakeBubble();

        for (int i = 0; i < makeCnt; ++i)
        {
            var spec = _specGenerator.First();
            var posX = _idxX + spec.pos[0];
            var posY = _idxY + spec.pos[1];

            var picBubble = Utils.RandomPick(spec.generator_list.ToList(), spec.generator_prob_list.ToList(), spec.generator_prob_list.Sum());
            var makeBubble = MapManager.instance.MakeBubble(picBubble, posX, posY);

            MoveBubbles(_makeList);

            yield return new WaitForSeconds(delay);

            _makeList.Insert(0, makeBubble);
        }
    }
    
    private async UniTask Generated(float delay)
    {
        int makeCnt = CheckGeneratedCnt();
        _makeList.Clear();
        CheckMakeBubble();
        _action = true;

        for (int i = 0; i < makeCnt; ++i)
        {
            var spec = _specGenerator.First();
            var posX = _idxX + spec.pos[0];
            var posY = _idxY + spec.pos[1];

            var picBubble = Utils.RandomPick(spec.generator_list.ToList(), spec.generator_prob_list.ToList(), spec.generator_prob_list.Sum());
            var makeBubble = MapManager.instance.MakeBubble(picBubble, posX, posY);

            MoveBubbles(_makeList);

            await UniTask.WaitForSeconds(delay);

            _makeList.Insert(0, makeBubble);
        }

        _action = false;
    }

    private void MoveBubbles(List<BaseBubble> lstBubble)
    {
        for(int i = 0; i < lstBubble.Count(); ++i)
        {
            if (_specGenerator.Count() > i + 1)
            {
                var posX = _idxX + _specGenerator[i+1].pos[0];
                var posY = _idxY + _specGenerator[i+1].pos[1];
                MapManager.instance.MoveBubble(lstBubble[i], posX, posY);
            }
        }
    }

    private void CheckMakeBubble()
    {
        foreach (var spec in _specGenerator)
        {
            var posX = _idxX + spec.pos[0];
            var posY = _idxY + spec.pos[1];

            if (MapManager.instance.BubbleMapDataField[posY][posX] == null)
                break;

            _makeList.Add(MapManager.instance.BubbleMapDataField[posY][posX]);
        }
    }
    private int CheckGeneratedCnt()
    {
        int cnt = 0;
        bool checkStart = false;

        foreach (var spec in _specGenerator)
        {
            var posX = _idxX + spec.pos[0];
            var posY = _idxY + spec.pos[1];

            if (MapManager.instance.BubbleMapDataField.Count <= posY ||
                MapManager.instance.BubbleMapDataField[posY].Count() <= posX)
            {
                var check = false;
            }
            else
            {
                if (!checkStart && MapManager.instance.BubbleMapDataField[posY][posX] == null)
                    checkStart = true;

                if (MapManager.instance.BubbleMapDataField[posY][posX] != null && checkStart)
                {
                    break;
                }

                if (checkStart)
                {
                    cnt += 1;
                }
            }
        }

        return cnt;
    }

    #endregion

    #region lifecycle

    #endregion
}
