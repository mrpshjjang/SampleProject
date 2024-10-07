using System;
using System.Collections.Generic;
using System.Linq;
//
// public partial class SpecSkill
// {
//     public List<SpecSkillAbility> specSkillAbilities { get; private set; }
//
//     public void Initialize()
//     {
//         specSkillAbilities = Facade.Spec.SpecSkillAbility.GetBySkillId(id).ToList();
//     }
// }
//
// public partial class SpecItem
// {
//     public bool IsInstantRandomBox => main_type == eItemMainType.RANDOM_BOX && stack_type == eItemStackType.UNSTACK;
//     public bool IsInstantItemBox => main_type == eItemMainType.ITEM_BOX && stack_type == eItemStackType.UNSTACK;
// }
//
// public partial class SpecShop
// {
//     public SpecShopTemplate template { get; private set; }
//     public SpecShopCondition condition { get; private set; }
//     public List<eShopViewPlace> ViewPlaces { get; private set; } = new();
//
//     public void Initialize()
//     {
//         template = Facade.Spec.SpecShopTemplate.Get(id);
//         condition = Facade.Spec.SpecShopCondition.Get(id);
//
//         if (template == null)
//         {
//             Debug.LogError($"SpecShop.Initalize:: template spec is null!! Id={id}");
//             return;
//         }
//
//         if (condition == null)
//         {
//             Debug.LogError($"SpecShop.Initalize:: condition spec is null!! Id={id}");
//             return;
//         }
//
//         foreach (var str in condition.view_place)
//             ViewPlaces.Add((eShopViewPlace) Enum.Parse(typeof(eShopViewPlace), str));
//     }
//
//     public bool IsInViewPlace(eShopViewPlace type)
//     {
//         return ViewPlaces.Contains(type);
//     }
//
//     public string GetPrice()
//     {
//         switch (purchase_type)
//         {
//             case eShopPurchaseType.CURRENCY: return ConsumeInfos[0].item_count.ToString();
// #if !UNITY_EDITOR
//             case eShopPurchaseType.IAP: return IAPManager.GetPriceString(this);
// #endif
//             default: return $"${template.price_check}";
//         }
//     }
// }
//
// public partial class SpecShopCondition
// {
//     public int UnlockConditionValueInt { get; private set; }
//     public int SubUnlockConditionValueInt { get; private set; }
//
//     public void Initialize()
//     {
//         UnlockConditionValueInt = int.Parse(unlock_condition_value);
//         SubUnlockConditionValueInt = int.Parse(unlock_condition_sub_value);
//     }
// }
//
// public partial class SpecEventSchedule
// {
//     public DateTime StartDate { get; private set; }
//     public DateTime EndDate { get; private set; }
//     public DateTime FinalEndDate { get; private set; }
//
//     public void Initialize()
//     {
//         StartDate = Utils.ConvertStringToDateTime(start_date);
//         EndDate = Utils.ConvertStringToDateTime(end_date);
//         FinalEndDate = final_end_date.Equals("NONE") ? EndDate : Utils.ConvertStringToDateTime(final_end_date);
//     }
// }
//
// public partial class SpecHeroGradeUpEffect
// {
//     public Dictionary<eStat, double> GetBasicStatDic()
//     {
//         var result = new Dictionary<eStat, double>();
//         if (basic_stat_key_1 != eStat.NONE)
//         {
//             if (result.ContainsKey(basic_stat_key_1))
//                 result.Add(basic_stat_key_1, basic_stat_value_1);
//             else
//                 result[basic_stat_key_1] += basic_stat_value_1;
//         }
//
//         if (basic_stat_key_2 != eStat.NONE)
//         {
//             if (result.ContainsKey(basic_stat_key_2))
//                 result.Add(basic_stat_key_2, basic_stat_value_2);
//             else
//                 result[basic_stat_key_2] += basic_stat_value_2;
//         }
//
//         if (basic_stat_key_3 != eStat.NONE)
//         {
//             if (result.ContainsKey(basic_stat_key_3))
//                 result.Add(basic_stat_key_3, basic_stat_value_3);
//             else
//                 result[basic_stat_key_3] += basic_stat_value_3;
//         }
//
//         return result;
//     }
// }
//
// public partial class SpecHeroLevelUpEffect
// {
//     public Dictionary<eStat, double> GetBasicStatDic()
//     {
//         var result = new Dictionary<eStat, double>();
//         if (basic_stat_key_1 != eStat.NONE)
//         {
//             if (result.ContainsKey(basic_stat_key_1))
//                 result.Add(basic_stat_key_1, basic_stat_value_1);
//             else
//                 result[basic_stat_key_1] += basic_stat_value_1;
//         }
//
//         if (basic_stat_key_2 != eStat.NONE)
//         {
//             if (result.ContainsKey(basic_stat_key_2))
//                 result.Add(basic_stat_key_2, basic_stat_value_2);
//             else
//                 result[basic_stat_key_2] += basic_stat_value_2;
//         }
//
//         if (basic_stat_key_3 != eStat.NONE)
//         {
//             if (result.ContainsKey(basic_stat_key_3))
//                 result.Add(basic_stat_key_3, basic_stat_value_3);
//             else
//                 result[basic_stat_key_3] += basic_stat_value_3;
//         }
//
//         return result;
//     }
// }
//
// public partial class SpecCostumeLevelUpEffect
// {
//     public Dictionary<eStat, double> GetStatDic()
//     {
//         var result = new Dictionary<eStat, double>();
//         result.Add(eStat.ATK_RATE, atk_rate_value);
//         result.Add(eStat.DEF_RATE, def_rate_value);
//         result.Add(eStat.HP_RATE, hp_rate_value);
//         return result;
//     }
// }
//
// public partial class SpecTreasure
// {
//     public eTreasureActivateType ActivateType => cooltime > 0 ? eTreasureActivateType.ACTIVE : eTreasureActivateType.PASSIVE;
// }
//
// public partial class SpecTraining
// {
//     public int GetDisplayingLevel(int level)
//     {
//         var prevStep = Facade.Spec.SpecTraining.GetByStep(stat_key, step - 1);
//         return prevStep == null ? level : level - prevStep.end_level;
//     }
// }
//
// public partial class SpecEquipmentLevelUp
// {
//     public ObfuscatorLong TotalNeedMaterial { get; private set; }
//
//     public void Initialize()
//     {
//         TotalNeedMaterial = Facade.Spec.SpecEquipmentLevelUp.All.Where(x => x.group_id == group_id && x.level <= level).Sum(x => x.material_count);
//     }
// }
//
// public partial class SpecStage
// {
//     public SpecSpawn specSpawn { get; private set; }
//     public SpecTrialSpawn specTrialSpawn { get; private set; }
//     public SpecStageEnvironment specEnvironment { get; private set; }
//     public int MaxWave { get; private set; }
//     public double atkMultiplier { get; private set; }
//     public double defMultiplier { get; private set; }
//     public double hpMultiplier { get; private set; }
//     public bool IsInitialized { get; private set; }
//
//     public void Initialize()
//     {
//         specSpawn = Facade.Spec.SpecSpawn.GetByKeyId(type, spawn_key_id);
//         specTrialSpawn = Facade.Spec.SpecTrialSpawn.GetByKeyId(type, spawn_key_id);
//         specEnvironment = Facade.Spec.SpecStageEnvironment.GetByKeyId(type, environment_key_id);
//         var listStage = Facade.Spec.SpecStage.GetStages(eStageType.STAGE, stage);
//         MaxWave = listStage.Count();
//
//         atkMultiplier = (1.0 + 0.01 * atk_mult);
//         defMultiplier = (1.0 + 0.01 * def_mult);
//         hpMultiplier = (1.0 + 0.01 * hp_mult);
//
//         var prevSpec = Facade.Spec.SpecStage.GetByType(type).FirstOrDefault(x => x.order == order - 1);
//         if (prevSpec != null)
//         {
//             if (!prevSpec.IsInitialized) prevSpec.Initialize();
//
//             atkMultiplier *= prevSpec.atkMultiplier;
//             defMultiplier *= prevSpec.defMultiplier;
//             hpMultiplier *= prevSpec.hpMultiplier;
//         }
//
//         IsInitialized = true;
//     }
//
//     public double ApplyMultiplier(eStat statType, double input)
//     {
//         switch (statType)
//         {
//             case eStat.ATK: return input * atkMultiplier;
//             case eStat.DEF: return input * defMultiplier;
//             case eStat.HP:  return input * hpMultiplier;
//         }
//
//         return input;
//     }
// }
//
//
// public partial class SpecDungeonPattern
// {
//     public double atkMultiplier { get; private set; }
//     public double defMultiplier { get; private set; }
//     public double hpMultiplier { get; private set; }
//     public bool IsInitialized { get; private set; }
//
//     public void Initialize()
//     {
//         atkMultiplier = (1.0 + 0.01 * atk_mult);
//         defMultiplier = (1.0 + 0.01 * def_mult);
//         hpMultiplier = (1.0 + 0.01 * hp_mult);
//
//         var prevSpec = Facade.Spec.SpecDungeonPattern.All.FirstOrDefault(x => x.wave == wave - 1);
//         if (prevSpec != null)
//         {
//             if (!prevSpec.IsInitialized) prevSpec.Initialize();
//
//             atkMultiplier *= prevSpec.atkMultiplier;
//             defMultiplier *= prevSpec.defMultiplier;
//             hpMultiplier *= prevSpec.hpMultiplier;
//         }
//
//         IsInitialized = true;
//     }
//
//     public double ApplyMultiplier(eStat statType, double input)
//     {
//         switch (statType)
//         {
//             case eStat.ATK: return input * atkMultiplier;
//             case eStat.DEF: return input * defMultiplier;
//             case eStat.HP:  return input * hpMultiplier;
//         }
//
//         return input;
//     }
// }
//
// public partial class SpecStageTrialBuff
// {
//     public List<ConditionInfo> Conditions { get; private set; }
//
//     public void Initialize()
//     {
//         Conditions = new();
//         if (organize_condition_type_1 != eOrganizeCondition.NONE) Conditions.Add(new ConditionInfo(organize_condition_type_1, organize_condition_apply_type_1, organize_condition_value_1));
//         if (organize_condition_type_2 != eOrganizeCondition.NONE) Conditions.Add(new ConditionInfo(organize_condition_type_2, organize_condition_apply_type_2, organize_condition_value_2));
//         if (organize_condition_type_3 != eOrganizeCondition.NONE) Conditions.Add(new ConditionInfo(organize_condition_type_3, organize_condition_apply_type_3, organize_condition_value_3));
//     }
//
//     public class ConditionInfo
//     {
//         public eOrganizeCondition ConditionType { get; private set; }
//         public eOrganizeConditionApplyType OrganizeType { get; private set; }
//         public ObfuscatorInt Count { get; private set; }
//
//         public ConditionInfo(eOrganizeCondition conditionType, eOrganizeConditionApplyType organizeType, ObfuscatorInt count)
//         {
//             ConditionType = conditionType;
//             OrganizeType = organizeType;
//             Count = count;
//         }
//     }
// }
//
// public partial class SpecDungeonAbyss
// {
//     public SpecDungeonAbyssReward specRewardNormal { get; private set; }
//     public SpecDungeonAbyssReward specRewardTreasureNormal { get; private set; }
//     public SpecDungeonAbyssReward specRewardTreasureEpic { get; private set; }
//     public SpecDungeonAbyssReward specRewardMileStone { get; private set; }
//     public SpecDungeonAbyssBoss specBoss { get; private set; }
//
//     public void Initialize()
//     {
//         specRewardNormal = Facade.Spec.SpecDungeonAbyssReward.GetByGroupIdAndLevel((int) eAbyssBattleType.NORMAL, level);
//         specRewardTreasureNormal = Facade.Spec.SpecDungeonAbyssReward.GetByGroupIdAndLevel((int) eAbyssBattleType.TREASURE_NORMAL, level);
//         specRewardTreasureEpic = Facade.Spec.SpecDungeonAbyssReward.GetByGroupIdAndLevel((int) eAbyssBattleType.TREASURE_EPIC, level);
//         specRewardMileStone = Facade.Spec.SpecDungeonAbyssReward.GetByGroupIdAndLevel((int) eAbyssBattleType.MILESTONE, level);
//         specBoss = Facade.Spec.SpecDungeonAbyssBoss.All.FirstOrDefault(x => x.wave == 1);
//     }
// }
//
// public partial class SpecSummon
// {
//     public SpecSummonCost SpecMainCost { get; private set; }
//     public SpecSummonCost SpecSubCost { get; private set; }
//
//     public void Initialize()
//     {
//         if (cost_id != 0) SpecMainCost = Facade.Spec.SpecSummonCost.Get(cost_id);
//         if (sub_cost_id != 0) SpecSubCost = Facade.Spec.SpecSummonCost.Get(sub_cost_id);
//     }
// }
//
// public partial class SpecDungeonAbyssBoss
// {
//     public double atkMultiplier { get; private set; }
//     public double defMultiplier { get; private set; }
//     public double hpMultiplier { get; private set; }
//     public bool IsInitialized { get; private set; }
//
//     public void Initialize()
//     {
//         atkMultiplier = (1.0 + 0.01 * atk_mult);
//         defMultiplier = (1.0 + 0.01 * def_mult);
//         hpMultiplier = (1.0 + 0.01 * hp_mult);
//
//         var prevSpec = Facade.Spec.SpecDungeonAbyssBoss.All.FirstOrDefault(x => x.wave == wave - 1);
//         if (prevSpec != null)
//         {
//             if (!prevSpec.IsInitialized) prevSpec.Initialize();
//
//             atkMultiplier *= prevSpec.atkMultiplier;
//             defMultiplier *= prevSpec.defMultiplier;
//             hpMultiplier *= prevSpec.hpMultiplier;
//         }
//
//         IsInitialized = true;
//     }
//
//     public double ApplyMultiplier(eStat statType, double input)
//     {
//         switch (statType)
//         {
//             case eStat.ATK: return input * atkMultiplier;
//             case eStat.DEF: return input * defMultiplier;
//             case eStat.HP:  return input * hpMultiplier;
//         }
//
//         return input;
//     }
// }
//
// public partial class SpecQuest
// {
//     public SpecQuestReward SpecNormalReward { get; private set; }
//     public SpecQuestReward SpecAdReward { get; private set; }
//
//     public void Initialize()
//     {
//         if (reward_id != 0) SpecNormalReward = Facade.Spec.SpecQuestReward.Get(reward_id);
//         if (ad_reward_id != 0) SpecAdReward = Facade.Spec.SpecQuestReward.Get(ad_reward_id);
//     }
// }
//
// public partial class SpecOfflineReward
// {
//     public List<OfflineRewardInfo> Infos => _infos ?? MakeInfo();
//     private List<OfflineRewardInfo> _infos;
//
//     private List<OfflineRewardInfo> MakeInfo()
//     {
//         ObfuscatorLong[] counts = new[]
//         {
//             reward_count_1, reward_count_2, reward_count_3,
//             reward_count_4, reward_count_5, reward_count_6,
//             reward_count_7, reward_count_8, reward_count_9,
//             reward_count_10, reward_count_11, reward_count_12, reward_count_13
//         }; //새 리워드가 추가되면 여기에 추가
//
//         _infos = new();
//         foreach (var spec in Facade.Spec.SpecOfflineRewardInfo.All)
//         {
//             _infos.Add(new OfflineRewardInfo(spec.reward_id, counts[spec.id - 1], spec.min_threshold));
//         }
//
//         return _infos;
//     }
//
//     public class OfflineRewardInfo
//     {
//         public ObfuscatorInt id;
//         public ObfuscatorLong count;
//         public ObfuscatorInt timeThreshold;
//
//         public OfflineRewardInfo(int id, ObfuscatorLong count, int timeThreshold)
//         {
//             this.id = id;
//             this.count = count;
//             this.timeThreshold = timeThreshold;
//         }
//
//         public bool CanReward(long minute)
//         {
//             return minute >= timeThreshold;
//         }
//
//         public long GetCycle(long minute)
//         {
//             if (!CanReward(minute)) return 0;
//             return minute / timeThreshold;
//         }
//     }
// }
//
// public partial class SpecPass
// {
//     public SpecPassReward SpecNormalReward { get; private set; }
//     public SpecPassReward SpecVipReward { get; private set; }
//
//     public void Initialize()
//     {
//         if (normal_reward_id != 0) SpecNormalReward = Facade.Spec.SpecPassReward.Get(normal_reward_id);
//         if (vip_reward_id != 0) SpecVipReward = Facade.Spec.SpecPassReward.Get(vip_reward_id);
//     }
// }
