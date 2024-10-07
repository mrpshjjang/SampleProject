using System;
using System.Collections.Generic;
using System.Linq;
using Sample.SpecData;

public static class SpecExtension
{
    // #region GetByGroupIdAndGrade
    //
    // public static SpecCommonRewardAndConsume GetByGroupIdAndGrade(this ISpecData<ObfuscatorInt, SpecCommonRewardAndConsume> data, eCommonRewardConsumeKey type)
    // {
    //     return data.All.FirstOrDefault(x => x.type == type);
    // }
    //
    // #endregion
    //
    //
    // #region SpecUnit
    //
    // public static IEnumerable<SpecUnit> GetUnitByGroup(this ISpecData<ObfuscatorInt, SpecUnit> data, bool isHeroGroup)
    // {
    //     return data.All.Where(x => x.is_hero_group == isHeroGroup);
    // }
    //
    // #endregion
    //
    // #region SpecHeroGradeUp
    //
    // public static SpecHeroGradeUp GetByGrade(this ISpecData<ObfuscatorInt, SpecHeroGradeUp> data, int rarity, int grade)
    // {
    //     return data.All.FirstOrDefault(x => x.rarity == rarity && x.grade == grade);
    // }
    //
    // public static int GetMaxGrade(this ISpecData<ObfuscatorInt, SpecHeroGradeUp> data, int rarity)
    // {
    //     return data.All.Where(x => x.rarity == rarity).Max(x=>x.grade);
    // }
    //
    // #endregion
    //
    // #region SpecHeroGradeUpEffect
    //
    // public static SpecHeroGradeUpEffect GetByGroupIdAndGrade(this ISpecData<ObfuscatorInt, SpecHeroGradeUpEffect> data, int groupId, int grade)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.grade == grade);
    // }
    //
    // #endregion
    //
    // #region SpecHeroLevelUp
    //
    // public static SpecHeroLevelUp GetByLevel(this ISpecData<ObfuscatorInt, SpecHeroLevelUp> data, int rarity, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.rarity == rarity && x.level == level);
    // }
    //
    // public static int GetMaxLevel(this ISpecData<ObfuscatorInt, SpecHeroLevelUp> data, int rarity)
    // {
    //     return data.All.Where(x => x.rarity == rarity).Max(x=>x.level);
    // }
    //
    // #endregion
    //
    // #region SpecHeroLevelUpEffect
    //
    // public static SpecHeroLevelUpEffect GetByGroupIdAndLevel(this ISpecData<ObfuscatorInt, SpecHeroLevelUpEffect> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecCostume
    //
    // public static IEnumerable<SpecCostume> GetByUnitId(this ISpecData<ObfuscatorInt, SpecCostume> data, int unitId)
    // {
    //     return data.All.Where(x => x.unit_id == unitId);
    // }
    //
    // #endregion
    //
    // #region SpecCostumeLevelUpMaterial
    //
    // public static IEnumerable<SpecCostumeLevelUpMaterial> GetByGroupId(this ISpecData<ObfuscatorInt, SpecCostumeLevelUpMaterial> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // public static SpecCostumeLevelUpMaterial GetByLevel(this ISpecData<ObfuscatorInt, SpecCostumeLevelUpMaterial> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecCostumeLevelUpEffect
    //
    // public static SpecCostumeLevelUpEffect GetByLevel(this ISpecData<ObfuscatorInt, SpecCostumeLevelUpEffect> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecEquipmentLevelUp
    //
    // public static SpecEquipmentLevelUp GetByLevel(this Dictionary<int, List<SpecEquipmentLevelUp>> data, int groupId, int level)
    // {
    //     return data[groupId].FirstOrDefault(x => x.level == level);
    // }
    //
    // public static int GetMaxLevel(this Dictionary<int, List<SpecEquipmentLevelUp>> data, int groupId)
    // {
    //     return data[groupId].Max(x=>x.level);
    // }
    //
    // #endregion
    //
    // #region SpecHeroTalent
    //
    // public static IEnumerable<SpecHeroTalent> GetByUnitId(this ISpecData<ObfuscatorInt, SpecHeroTalent> data, int unitId)
    // {
    //     return data.All.Where(x => x.unit_id == unitId);
    // }
    //
    // #endregion
    //
    // #region SpecTreasureLevelUp
    //
    // public static SpecTreasureLevelUp GetByLevel(this ISpecData<ObfuscatorInt, SpecTreasureLevelUp> data, int rarity, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.rarity == rarity && x.level == level);
    // }
    //
    // public static int GetMaxLevel(this ISpecData<ObfuscatorInt, SpecTreasureLevelUp> data, int rarity)
    // {
    //     return data.All.Where(x => x.rarity == rarity).Max(x=>x.level);
    // }
    //
    // #endregion
    //
    // #region SpecTreasureBasicStatLevelUp
    //
    // public static SpecTreasureBasicStatLevelUp GetByLevel(this ISpecData<ObfuscatorInt, SpecTreasureBasicStatLevelUp> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecTreasureGradeUp
    //
    // public static SpecTreasureGradeUp GetByGrade(this ISpecData<ObfuscatorInt, SpecTreasureGradeUp> data, int rarity, int grade)
    // {
    //     return data.All.FirstOrDefault(x => x.rarity == rarity && x.grade == grade);
    // }
    //
    // public static int GetMaxGrade(this ISpecData<ObfuscatorInt, SpecTreasureGradeUp> data, int rarity)
    // {
    //     return data.All.Where(x => x.rarity == rarity).Max(x=>x.grade);
    // }
    //
    // #endregion
    //
    // #region SpecTreasureBasicStatGradeUp
    //
    // public static SpecTreasureBasicStatGradeUp GetByGrade(this ISpecData<ObfuscatorInt, SpecTreasureBasicStatGradeUp> data, int groupId, int grade)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.grade == grade);
    // }
    //
    // #endregion
    //
    // #region SpecRelicLevelUp
    //
    // public static SpecRelicLevelUp GetByLevel(this ISpecData<ObfuscatorInt, SpecRelicLevelUp> data, int rarity, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.rarity == rarity && x.level == level);
    // }
    //
    // public static int GetMaxLevel(this ISpecData<ObfuscatorInt, SpecRelicLevelUp> data, int rarity)
    // {
    //     return data.All.Where(x => x.rarity == rarity).Max(x=>x.level);
    // }
    //
    // #endregion
    //
    // #region SpecAttendance
    //
    // public static IEnumerable<SpecAttendance> GetByType(this ISpecData<ObfuscatorInt, SpecAttendance> data, eAttendanceType type)
    // {
    //     return data.All.Where(x => x.type == type);
    // }
    //
    // public static SpecAttendance GetByDay(this ISpecData<ObfuscatorInt, SpecAttendance> data, eAttendanceType type, int day)
    // {
    //     return data.All.FirstOrDefault(x => x.type == type && x.day == day);
    // }
    //
    // #endregion
    //
    // #region SpecPatternStat
    //
    // public static IEnumerable<SpecPatternStat> GetByLevels(this ISpecData<ObfuscatorInt, SpecPatternStat> data, int groupId, int level)
    // {
    //     return data.All.Where(x => x.group_id == groupId && x.level <= level);
    // }
    //
    // public static IEnumerable<SpecPatternStat> GetBySpecificLevel(this ISpecData<ObfuscatorInt, SpecPatternStat> data, int groupId, int level)
    // {
    //     return data.All.Where(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecPatternLevelUp
    //
    // public static SpecPatternLevelUp GetByLevel(this ISpecData<ObfuscatorInt, SpecPatternLevelUp> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // public static int GetMaxLevel(this ISpecData<ObfuscatorInt, SpecPatternLevelUp> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId).Max(x=>x.level);
    // }
    //
    // #endregion
    //
    // #region SpecPatternStat
    //
    // public static SpecPatternCompose GetByCount(this ISpecData<ObfuscatorInt, SpecPatternCompose> data, long count)
    // {
    //     return data.All.FirstOrDefault(x => x.compose_count == count) ?? data.All.Last();
    // }
    //
    // #endregion
    //
    // #region SpecQuest
    //
    // public static IEnumerable<SpecQuest> GetByCategory(this ISpecData<ObfuscatorInt, SpecQuest> data, eQuestCategory category)
    // {
    //     return data.All.Where(x => x.category == category);
    // }
    //
    // #endregion
    //
    // #region SpecGuideMission
    //
    // public static SpecGuideMission GetByLevel(this ISpecData<ObfuscatorInt, SpecGuideMission> data, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecContentUnlock
    //
    // public static SpecContentUnlock GetByType(this ISpecData<ObfuscatorInt, SpecContentUnlock> data, eContentUnlockKey type)
    // {
    //     return data.All.FirstOrDefault(x => x.key == type);
    // }
    //
    // #endregion
    //
    // #region SpecStage
    //
    // public static IEnumerable<SpecStage> GetByType(this ISpecData<ObfuscatorInt, SpecStage> data, eStageType type)
    // {
    //     return data.All.Where(x => x.type == type);
    // }
    //
    // public static IEnumerable<SpecStage> GetStages(this ISpecData<ObfuscatorInt, SpecStage> data, eStageType type, int stage)
    // {
    //     return data.All.Where(x => x.type == type && x.stage == stage);
    // }
    //
    // public static SpecStage GetStage(this ISpecData<ObfuscatorInt, SpecStage> data, eStageType type, int stage, int wave)
    // {
    //     return data.All.FirstOrDefault(x => x.type == type && x.stage == stage && x.wave == wave);
    // }
    //
    // #endregion
    //
    // #region SpecStageInfo
    //
    // public static IEnumerable<SpecSpawn> GetByKeyType(this ISpecData<ObfuscatorInt, SpecSpawn> data, eStageType type)
    // {
    //     return data.All.Where(x => x.stage_type == type);
    // }
    //
    // public static SpecSpawn GetByKeyId(this ISpecData<ObfuscatorInt, SpecSpawn> data, eStageType type, int keyId)
    // {
    //     return data.All.FirstOrDefault(x => x.stage_type == type && x.key_id == keyId);
    // }
    //
    // public static SpecTrialSpawn GetByKeyId(this ISpecData<ObfuscatorInt, SpecTrialSpawn> data, eStageType type, int keyId)
    // {
    //     return data.All.FirstOrDefault(x => x.stage_type == type && x.key_id == keyId);
    // }
    //
    // #endregion
    //
    // #region SpecStageEnvironment
    //
    // public static SpecStageEnvironment GetByKeyId(this ISpecData<ObfuscatorInt, SpecStageEnvironment> data, eStageType type, int keyId)
    // {
    //     return data.All.FirstOrDefault(x => x.stage_type == type && x.key_id == keyId);
    // }
    //
    // #endregion
    //
    // #region SpecStageDrop
    //
    // public static IEnumerable<SpecStageDrop> GetByGroupId(this ISpecData<ObfuscatorInt, SpecStageDrop> data, int groupId, bool hasProb)
    // {
    //     var temp = data.All.Where(x => x.group_id == groupId);
    //     return hasProb ? temp.Where(x => x.item_prob > 0) : temp.Where(x => x.item_prob < 0);
    // }
    //
    // #endregion
    //
    // #region SpecStageClear
    //
    // public static SpecStageClear GetByStage(this ISpecData<ObfuscatorInt, SpecStageClear> data, eStageType type, int stage)
    // {
    //     return data.All.FirstOrDefault(x => x.type == type && x.stage == stage);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyss
    //
    // public static SpecDungeonAbyss GetByLevel(this ISpecData<ObfuscatorInt, SpecDungeonAbyss> data, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyssReward
    //
    // public static SpecDungeonAbyssReward GetByGroupIdAndLevel(this ISpecData<ObfuscatorInt, SpecDungeonAbyssReward> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyssBoss
    //
    // public static SpecDungeonAbyssBoss GetByWave(this ISpecData<ObfuscatorInt, SpecDungeonAbyssBoss> data, int wave)
    // {
    //     return data.All.FirstOrDefault(x => x.wave == wave) ?? data.All.Last();
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyssDispatch
    //
    // public static IEnumerable<SpecDungeonAbyssDispatch> GetByLevel(this ISpecData<ObfuscatorInt, SpecDungeonAbyssDispatch> data, int level)
    // {
    //     return data.All.Where(x => x.unlock_level <= level);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyssDispatchReward
    //
    // public static IEnumerable<SpecDungeonAbyssDispatchReward> GetByGroupId(this ISpecData<ObfuscatorInt, SpecDungeonAbyssDispatchReward> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyssMerchant
    //
    // public static IEnumerable<SpecDungeonAbyssMerchant> GetByGroupId(this ISpecData<ObfuscatorInt, SpecDungeonAbyssMerchant> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonAbyssBossStatBuff
    //
    // public static SpecDungeonAbyssBossStatBuff GetBySpawnId(this ISpecData<ObfuscatorInt, SpecDungeonAbyssBossStatBuff> data, int spawnId)
    // {
    //     return data.All.FirstOrDefault(x => x.spawn_id == spawnId);
    // }
    //
    // #endregion
    //
    // #region SpecDungeonPattern
    //
    // public static SpecDungeonPattern GetByWave(this ISpecData<ObfuscatorInt, SpecDungeonPattern> data, int wave)
    // {
    //     return data.All.FirstOrDefault(x => x.wave == wave) ?? data.All.Last();
    // }
    //
    // #endregion
    //
    // #region SpecSkillAbility
    //
    // public static IEnumerable<SpecSkillAbility> GetBySkillId(this ISpecData<ObfuscatorInt, SpecSkillAbility> data, int skillId)
    // {
    //     return data.All.Where(x => x.skill_id == skillId);
    // }
    //
    // #endregion
    //
    // #region SpecTraining
    //
    // public static IEnumerable<SpecTraining> GetByStatType(this ISpecData<ObfuscatorInt, SpecTraining> data, eStat type)
    // {
    //     return data.All.Where(x => x.stat_key == type);
    // }
    //
    // public static SpecTraining GetByStep(this ISpecData<ObfuscatorInt, SpecTraining> data, eStat type, int step)
    // {
    //     return data.All.FirstOrDefault(x => x.stat_key == type && x.step == step);
    // }
    //
    // public static SpecTraining GetStepByLevel(this ISpecData<ObfuscatorInt, SpecTraining> data, eStat type, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.stat_key == type && x.start_level <= level && x.end_level >= level);
    // }
    //
    // #endregion
    //
    // #region SpecAdBuff
    //
    // public static IEnumerable<SpecAdBuff> GetByFirstLevel(this ISpecData<ObfuscatorInt, SpecAdBuff> data)
    // {
    //     var lowestLevelBuffs = data.All
    //         .GroupBy(buff => buff.group_id)
    //         .Select(g => g.OrderBy(buff => buff.level).First())
    //         .ToList();
    //
    //     return lowestLevelBuffs;
    // }
    //
    // public static IEnumerable<SpecAdBuff> GetByGroupId(this ISpecData<ObfuscatorInt, SpecAdBuff> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // public static SpecAdBuff GetByLevel(this ISpecData<ObfuscatorInt, SpecAdBuff> data, int groupId, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId && x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecSummon
    //
    // public static IEnumerable<SpecSummon> GetBySummonType(this ISpecData<ObfuscatorInt, SpecSummon> data, eSummonType type)
    // {
    //     return data.All.Where(x => x.summon_type == type);
    // }
    //
    // public static SpecSummon GetByLevel(this ISpecData<ObfuscatorInt, SpecSummon> data, eSummonType type, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.summon_type == type && x.level == level);
    // }
    //
    // public static List<SpecSummon> GetByLevels(this ISpecData<ObfuscatorInt, SpecSummon> data, eSummonType type, int level)
    // {
    //     return data.All.Where(x => x.summon_type == type && x.level <= level).ToList();
    // }
    //
    // #endregion
    //
    // #region SpecPickUpSummon
    //
    // public static SpecPickUpSummon GetByGroupId(this ISpecData<ObfuscatorInt, SpecPickUpSummon> data, int groupId)
    // {
    //     return data.All.FirstOrDefault(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecPickUpSummonStep
    //
    // public static IEnumerable<SpecPickUpSummonStep> GetByGroupId(this ISpecData<ObfuscatorInt, SpecPickUpSummonStep> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecRandomBox
    //
    // public static IEnumerable<SpecRandomBox> GetByGroupId(this ISpecData<ObfuscatorInt, SpecRandomBox> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecTraining
    //
    // public static List<SpecTrainingReward> GetSpecTrainingRewardList(this ISpecData<ObfuscatorInt, SpecTrainingReward> data)
    // {
    //     return data.All.ToList();
    // }
    //
    // #endregion
    //
    // #region SpecTrait
    //
    // public static IEnumerable<SpecTraitTree> GetTraitTreeByGroupId(this ISpecData<ObfuscatorInt, SpecTraitTree> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecConstellation
    //
    // public static IEnumerable<SpecConstellationTree> GetConstellationTreeByGroupId(this ISpecData<ObfuscatorInt, SpecConstellationTree> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecEventSchedule
    //
    // public static SpecEventSchedule GetByEventId(this ISpecData<ObfuscatorInt, SpecEventSchedule> data, int eventId)
    // {
    //     return data.All.FirstOrDefault(x => x.event_id == eventId);
    // }
    //
    // #endregion
    //
    // #region SpecShop
    //
    // // public static SpecShop GetByProductId(this ISpecData<ObfuscatorInt, SpecShop> data, string productId)
    // // {
    // //     return data.All.FirstOrDefault(x => x.product_id.ToString().Equals(productId));
    // // }
    //
    // public static SpecShop GetByUniqueProductId(this ISpecData<ObfuscatorInt, SpecShop> data, eUniqueProductType uniqueProductType)
    // {
    //     return data.All.FirstOrDefault(x => x.unique_product_type == uniqueProductType);
    // }
    //
    // public static IEnumerable<SpecShop> GetUnlockCondition(this ISpecData<ObfuscatorInt, SpecShop> data, ePurchaseUnlockType condition)
    // {
    //     if (condition == ePurchaseUnlockType.NONE)
    //         return new List<SpecShop>();
    //
    //     return data.All.Where(x => x.condition.unlock_condition == condition || x.condition.unlock_condition_sub == condition);
    // }
    //
    // #endregion
    //
    // #region SpecBox
    //
    // public static SpecBox GetBySourceId(this ISpecData<ObfuscatorInt, SpecBox> data, int sourceItemId)
    // {
    //     return data.All.FirstOrDefault(x => x.source_item_id == sourceItemId);
    // }
    //
    // #endregion
    //
    // #region SpecDimensionalGrowth
    //
    // public static int GetMaxLevel(this ISpecData<ObfuscatorInt, SpecDimensionalGrowth> data)
    // {
    //     return data.All.Last().level;
    // }
    //
    // public static SpecDimensionalGrowth GetByLevel(this ISpecData<ObfuscatorInt, SpecDimensionalGrowth> data, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecTotemDrawRarity
    //
    // public static SpecTotemDrawRarity GetByUpgrade(this ISpecData<ObfuscatorInt, SpecTotemDrawRarity> data, int upgrade)
    // {
    //     return data.All.FirstOrDefault(x => x.upgrade == upgrade);
    // }
    //
    // public static List<int> GetRarity(this SpecTotemDrawRarity data)
    // {
    //     return new List<int>(
    //         new int[] {
    //             data.rarity_1,
    //             data.rarity_2,
    //             data.rarity_3,
    //             data.rarity_4,
    //             data.rarity_5,
    //             data.rarity_6,
    //             data.rarity_7,
    //             data.rarity_8,
    //             data.rarity_9,
    //             data.rarity_10,
    //         });
    // }
    //
    // #endregion
    //
    // #region SpecTotemDrawStat
    //
    // public static SpecTotemDrawStat Get(this ISpecData<ObfuscatorInt, SpecTotemDrawStat> data, eStat stat, int rarity)
    // {
    //     return data.All.FirstOrDefault(x => x.stat_type == stat && x.rarity == rarity);
    // }
    //
    // #endregion
    //
    // #region SpecTotemDrawLevelStat
    //
    // public static SpecTotemDrawLevelStat GetByLevel(this ISpecData<ObfuscatorInt, SpecTotemDrawLevelStat> data, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.level == level);
    // }
    //
    // public static List<SpecTotemDrawLevelStat> GetListByLevel(this ISpecData<ObfuscatorInt, SpecTotemDrawLevelStat> data, int level)
    // {
    //     return data.All.ToList().FindAll(e=>e.level == level);
    // }
    //
    // #endregion
    //
    // #region SpecTotemDrawRarityCondition
    //
    // public static SpecTotemDrawRarityCondition GetByUpgradeStep(this ISpecData<ObfuscatorInt, SpecTotemDrawRarityCondition> data, int upgrade, int step)
    // {
    //     return data.All.FirstOrDefault(x => x.upgrade == upgrade && x.step == step);
    // }
    //
    // public static int GetByUpgradeMax(this ISpecData<ObfuscatorInt, SpecTotemDrawRarityCondition> data, int upgrade)
    // {
    //     return data.All.ToList().FindAll(x => x.upgrade == upgrade).Count;
    // }
    //
    // #endregion
    //
    // #region SpecTotemRarityReward
    //
    // public static SpecTotemRarityReward GetByRarity(this ISpecData<ObfuscatorInt, SpecTotemRarityReward> data, int rarity)
    // {
    //     return data.All.FirstOrDefault(x => x.rarity == rarity);
    // }
    //
    // #endregion
    //
    // #region SpecLab
    //
    // public static IEnumerable<SpecLabTree> GetLabTreeByGroupId(this ISpecData<ObfuscatorInt, SpecLabTree> data, int groupId)
    // {
    //     return data.All.Where(x => x.group_id == groupId);
    // }
    //
    // #endregion
    //
    // #region SpecBenefits
    //
    // public static SpecBenefits GetByItemId(this ISpecData<ObfuscatorInt, SpecBenefits> data, int itemId)
    // {
    //     return data.All.FirstOrDefault(x => x.item_id == itemId);
    // }
    //
    // public static SpecBenefits GetByTypeAndValue(this ISpecData<ObfuscatorInt, SpecBenefits> data, eBenefitsType type, double value)
    // {
    //     return data.All.FirstOrDefault(x => x.benefits_type == type && x.benefits_value == value);
    // }
    //
    // public static SpecBenefits GetByType(this ISpecData<ObfuscatorInt, SpecBenefits> data, eBenefitsType type)
    // {
    //     return data.All.FirstOrDefault(x => x.benefits_type == type);
    // }
    //
    // #endregion
    //
    // #region #SpecVIP
    //
    // public static SpecVIP GetByLevel(this ISpecData<ObfuscatorInt, SpecVIP> data, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.level == level);
    // }
    //
    // public static int GetMaxLevel(this ISpecData<ObfuscatorInt, SpecVIP> data)
    // {
    //     return data.All.Last().level;
    // }
    //
    // public static int GetLevel(this ISpecData<ObfuscatorInt, SpecVIP> data, long exp)
    // {
    //     long amount = 0;
    //     int level = 0;
    //     foreach (SpecVIP specVip in data.All.ToList())
    //     {
    //         amount = specVip.ConsumeInfos[0].item_count;
    //         if (exp < amount)
    //         {
    //             level = specVip.level;
    //             break;
    //         }
    //     }
    //     if(level == 0)
    //         level = Facade.Spec.SpecVIP.GetMaxLevel();
    //
    //     level = Math.Min(level, Facade.Spec.SpecVIP.GetMaxLevel());
    //
    //     return level;
    // }
    //
    // public static long GetCalcExp(this ISpecData<ObfuscatorInt, SpecVIP> data, long exp)
    // {
    //     long amount = exp;
    //     foreach (SpecVIP specVip in data.All.ToList())
    //     {
    //         if (amount - specVip.ConsumeInfos[0].item_count > 0)
    //         {
    //             amount -= specVip.ConsumeInfos[0].item_count;
    //         }
    //         else
    //         {
    //             break;
    //         }
    //     }
    //
    //     return amount;
    // }
    //
    //
    // #endregion
    //
    // #region #SpecVIPDaily
    //
    // public static SpecVIPDaily GetByLevel(this ISpecData<ObfuscatorInt, SpecVIPDaily> data, int level)
    // {
    //     return data.All.FirstOrDefault(x => x.level == level);
    // }
    // #endregion
    //
    //
    // #region #SpecVIP_Effect
    //
    // public static void GetStatsByLevel(this ISpecData<ObfuscatorInt, SpecVIP_Effect> data, int startLevel, int endLevel, ref Dictionary<eStat, double> dicStat)
    // {
    //     dicStat.Clear();
    //
    //     List<SpecVIP_Effect> findAll = data.All.ToList().FindAll(e => e.level >= startLevel && e.level <= endLevel);
    //     foreach (SpecVIP_Effect effect in findAll)
    //     {
    //         if (!dicStat.ContainsKey(effect.upgrade_type))
    //         {
    //             dicStat.Add(effect.upgrade_type, effect.upgrade_value);
    //         }
    //         else
    //         {
    //             dicStat[effect.upgrade_type] = effect.upgrade_value;
    //         }
    //     }
    // }
    //
    // public static int GetOrderByStat(this ISpecData<ObfuscatorInt, SpecVIP_Effect> data, eStat stat)
    // {
    //     var specVipEffect = data.All.ToList().FirstOrDefault(e => e.upgrade_type == stat);
    //     if (specVipEffect != null)
    //     {
    //         return specVipEffect.order;
    //     }
    //
    //     return 10000;
    // }
    //
    //
    // #endregion
    //
    //
    // #region #SpecStageDailyReward
    //
    // public static SpecStageDailyReward GetSpecByStage(this ISpecData<ObfuscatorInt, SpecStageDailyReward> data, int stage)
    // {
    //     return data.All.FirstOrDefault(e => e.start_stage <= stage && e.end_stage >= stage);
    // }
    //
    // #endregion
    //
    // #region #SpecPVPTier
    //
    // public static SpecPVPTier GetSpecByTier(this ISpecData<ObfuscatorInt, SpecPVPTier> data, int score)
    // {
    //     return data.All.FirstOrDefault(e => e.ranking_min <= score && e.ranking_max >= score);
    // }
    //
    // #endregion
    //
    // #region #SpecPVPDummyUserName
    //
    // public static IEnumerable<SpecPVPDummyUserName> GetByGroupId(this ISpecData<ObfuscatorInt, SpecPVPDummyUserName> data, int groupId)
    // {
    //     return data.All.Where(e => e.group_id == groupId);
    // }
    //
    // #endregion
}
