using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class NormalBubble : BaseBubble
{
    #region public

    public override void DestroyBubble(float delayTime = 0f, Action destroyComplete = null, Action destroyStart = null)
    {
        base.DestroyBubble(delayTime, destroyComplete, destroyStart);

        if (SubAttribute != null && SubAttribute.attribute == eBubbleAttribute.SUB_BOSS_ATTACK)
        {
            IngameDataManager.instance.AttackBoss(10);
        }
    }


    #endregion

    #region protected

    #endregion

    #region private

    #endregion

    #region lifecycle

    #endregion
}
