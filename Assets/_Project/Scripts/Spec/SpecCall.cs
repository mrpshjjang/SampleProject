using System;
using System.Collections.Generic;
using System.Linq;
using Sample.SpecData;

public static class SpecCall
{
    #region Option

    // public static readonly Dictionary<int, List<SpecEquipmentLevelUp>> DicEquipmentLevelUp = new();
    // public static readonly Dictionary<eShopViewPlace, List<SpecShop>> DicShopInViewPlace = new();

    // public static int Option(eOption option)
    // {
    //     return SpecDataManager.Instance.GetOption(option);
    // }
    //
    // public static string Option(ePvPOption option)
    // {
    //     return SpecDataManager.Instance.GetPvPOption(option);
    // }
    //
    // public static string GetRandomLocalize(string includeKey)
    // {
    //     var nominate = SpecDataManager.Instance.GetLocalizeTextInclude(includeKey);
    //     if (nominate is null || nominate.Count == 0)
    //     {
    //         return string.Empty;
    //     }
    //
    //     return Utils.RandomPick(nominate);
    // }

    #endregion


    #region public

    public static void Clear()
    {
        // DicEquipmentLevelUp.Clear();
        // DicShopInViewPlace.Clear();
    }

    public static void InitializeSpecs()
    {
        // //SpecShopCondition
        // foreach (var spec in Facade.Spec.SpecShopCondition.All) spec.Initialize();
        //
        // //SpecEventSchedule
        // foreach (var spec in Facade.Spec.SpecEventSchedule.All) spec.Initialize();
        //
        // //SpecEquipmentLevelUp
        // foreach (var spec in Facade.Spec.SpecEquipmentLevelUp.All)
        // {
        //     spec.Initialize();
        //
        //     if(!DicEquipmentLevelUp.ContainsKey(spec.group_id))
        //         DicEquipmentLevelUp.Add(spec.group_id, new List<SpecEquipmentLevelUp>());
        //
        //     DicEquipmentLevelUp[spec.group_id].Add(spec);
        // }
        //
        // //SpecSkill
        // foreach (var spec in Facade.Spec.SpecSkill.All) spec.Initialize();
        //
        // //SpecStage
        // foreach (var spec in Facade.Spec.SpecStage.All) spec.Initialize();
        //
        // //SpecDungeonPattern
        // foreach (var spec in Facade.Spec.SpecDungeonPattern.All) spec.Initialize();
        //
        // //SpecStageTrialBuff
        // foreach (var spec in Facade.Spec.SpecStageTrialBuff.All) spec.Initialize();
        //
        // //SpecSummon
        // foreach (var spec in Facade.Spec.SpecSummon.All) spec.Initialize();
        //
        // //SpecDungeonAbyss
        // foreach (var spec in Facade.Spec.SpecDungeonAbyss.All) spec.Initialize();
        //
        // //SpecDungeonAbyssBoss
        // foreach (var spec in Facade.Spec.SpecDungeonAbyssBoss.All) spec.Initialize();
        //
        // //SpecQuest
        // foreach (var spec in Facade.Spec.SpecQuest.All) spec.Initialize();
        //
        // //SpecPass
        // foreach (var spec in Facade.Spec.SpecPass.All) spec.Initialize();

#if UNITY_EDITOR
        VerifySpecs();
#endif
    }

    #endregion


    #region protected

    #endregion


    #region private

    private static void VerifySpecs()
    {
        //스킬 데이터 검사
        // foreach (var spec in Facade.Spec.SpecSkill.All)
        // {
        //     if (spec.prefab_key.Equals("NONE")) continue;
        //
        //     ObjectPoolManager.Instance.LoadObjectPool(eObjPoolType.SKILL, spec.prefab_key);
        //     var skill = ObjectPoolManager.Instance.GetPoolObject<SkillBase>(eObjPoolType.SKILL, spec.prefab_key);
        //     ObjectPoolManager.Instance.ReturnPoolObject(eObjPoolType.SKILL, spec.prefab_key, skill);
        // }
    }

    #endregion


    #region lifecycle

    #endregion
}
