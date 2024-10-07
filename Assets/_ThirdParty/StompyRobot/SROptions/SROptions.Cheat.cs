using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if ENABLE_SRD
public partial class SROptions
{
    #region 네트워크

    [Category("네트워크")]
    public void 데이터_즉시_저장()
    {
        PDManager.SaveImmediatelyAll().Forget();
    }

    [Category("네트워크")]
    public void 데이터_로드()
    {
        PDManager.SetPlayerData().Forget();
    }

    [Category("네트워크")]
    public void 커런시_네트워크_에러()
    {
        GameGrpcManager.Instance.GRewardHardCurrency(0, "Test", new List<int> {999999}, new List<double> {1}, true, null).Forget();
    }

    // [Category("네트워크")]
    // public void 네트워크_강제_에러()
    // {
    //     GameGrpcManager.IsForceNetworkError = !GameGrpcManager.IsForceNetworkError;
    // }

    #endregion

    #region 씬전환

    [Category("씬전환")]
    public void 스플레시_씬으로_이동()
    {
        AccountSystem.LogoutAndSplash(false).Forget();
    }

    #endregion

    #region 콘텐츠 해금

    [Category("콘텐츠 해금")]
    public eContentUnlockKey contentUnlockKey = eContentUnlockKey.NONE;

    [Category("콘텐츠 해금")]
    public eContentUnlockKey ContentUnlockKey
    {
        get => this.contentUnlockKey;
        set => this.contentUnlockKey = value;
    }

    [Category("콘텐츠 해금")]
    public void 특정_콘텐츠_해금()
    {
        PDManager.Data.ContentUnlock.AddUnlocked(ContentUnlockKey.ToString());

        ContentUnlockSystem.CheckCondition();
    }

    [Category("콘텐츠 해금")]
    public void 모든_콘텐츠_해금()
    {
        foreach (SpecContentUnlock spec in Facade.Spec.SpecContentUnlock.All)
        {
            PDManager.Data.ContentUnlock.AddUnlocked(spec.key.ToString());
        }

        ContentUnlockSystem.CheckCondition();
    }

    #endregion

    #region 스테이지

    [Category("스테이지")]
    public int stage = 1;

    [Category("스테이지")]
    public int Stage
    {
        get => this.stage;
        set => this.stage = value;
    }

    [Category("스테이지")]
    public void 스테이지_이동()
    {
        if (PDManager.Data.Stage.ClearedStage < Stage - 1)
            PDManager.Data.Stage.ClearedStage = Stage - 1;
        PDManager.Data.Stage.SetCurStage(Stage);

        InGameManager.Instance.ChangeBattleMode(eBattleMode.STAGE, Facade.FadeInOutFront);
    }

    #endregion

    #region 재화

    [Category("재화")]
    public eCurrencyType currencyType = eCurrencyType.GOLD;

    [Category("재화")]
    public string currencyAmount = "1000000000";

    [Category("재화")]
    public eCurrencyType CurrencyType
    {
        get => this.currencyType;
        set => this.currencyType = value;
    }

    [Category("재화")]
    public string CurrencyAmount
    {
        get => this.currencyAmount;
        set => this.currencyAmount = value;
    }

    [Category("재화")]
    public void 재화_획득()
    {
        if (long.TryParse(currencyAmount, out long result))
        {
            if ((double)result + ItemSystem.Get((int) CurrencyType).Amount > long.MaxValue)
            {
                result = long.MaxValue - ItemSystem.Get((int) CurrencyType).Amount;
            }

            if (Utils.IsServerBaseItem((int) CurrencyType))
            {
                ItemSystem.EarnHardCurrency(new List<HardCurrency>{new (){Type = (int) CurrencyType, Amount = result}}, "", "");
            }
            else
            {
                ItemSystem.Add((int) CurrencyType, result, "", "");
            }
        }
        else
        {
            currencyAmount = "0";
        }
    }

    [Category("재화")]
    public void 재화_소모()
    {
        if (!Utils.IsServerBaseItem((int) CurrencyType))
        {
            if (long.TryParse(currencyAmount, out long result))
            {
                ItemSystem.Use((int) CurrencyType, result, "", "");
            }
            else
            {
                currencyAmount = "0";
            }
        }
    }

    [Category("재화")]
    public void 모든_영웅_수량_1_증가()
    {
        foreach (var data in PDManager.Data.Unit.Units)
        {
            data.AddAmount(1);
        }
    }

    [Category("재화")]
    public void 모든_영웅_수량_50_증가()
    {
        foreach (var data in PDManager.Data.Unit.Units)
        {
            data.AddAmount(50);
        }
    }

    [Category("재화")]
    public void 재화_획득_락()
    {
        ItemDataCurrency.IsCurrencyAddLock = !ItemDataCurrency.IsCurrencyAddLock;
    }

    [Category("재화")]
    public void 재화_디스플레이()
    {
        foreach (var data in PDManager.Data.Currency.Items)
        {
            Debug.LogColor($"{data.Id}:Amount:{data.Amount}, AccAdd:{data.AccAdd}, AccUse:{data.AccUse}");
        }
    }

    #endregion

    #region 아이템지급

    [Category("아이템지급")]
    public int itemId = 0;

    [Category("아이템지급")]
    public int itemAmount = 1;

    [Category("아이템지급")]
    public int ItemId
    {
        get => this.itemId;
        set => this.itemId = value;
    }

    [Category("아이템지급")]
    public int ItemAmount
    {
        get => this.itemAmount;
        set => this.itemAmount = value;
    }

    [Category("아이템지급")]
    public async void 아이템_지급()
    {
        if (itemId == 0) return;

        var rewardBundle = new RewardBundle(true);

        if (Utils.IsServerBaseItem(itemId))
        {
            ItemSystem.EarnHardCurrency(new List<HardCurrency>{new (){Type = ItemId, Amount = ItemAmount}}, "", "", rewardBundle);
        }
        else
        {
            ItemSystem.Add(ItemId, ItemAmount, "", "", rewardBundle);
        }

        ItemSystem.ShowReward(rewardBundle);
    }

    #endregion

    #region 가이드퀘스트

    [Category("가이드퀘스트")]
    public void 가이드퀘스트_클리어()
    {
        if (!PDManager.Data.GuideMission.HasGuideMission) return;
        if (PDManager.Data.GuideMission.TryGetReward(null,true))
            GuideMissionSystem.UpdateMission();
    }

    [Category("가이드퀘스트")]
    public void 가이드퀘스트_패스()
    {
        PDManager.Data.GuideMission.IsRewarded = true;
        GuideMissionSystem.UpdateMission();
    }

    [Category("가이드퀘스트")]
    public int guideQuestLevel = 1;

    [Category("가이드퀘스트")]
    public int GuideQuestLevel
    {
        get => this.guideQuestLevel;
        set => this.guideQuestLevel = value;
    }

    [Category("가이드퀘스트")]
    public void 특정_가이드퀘스트로_변경()
    {
        PDManager.Data.GuideMission.Level = GuideQuestLevel - 1;
        PDManager.Data.GuideMission.IsRewarded = true;
        GuideMissionSystem.UpdateMission();
    }

    #endregion

    #region 장난감 뽑기
    [Category("장난감뽑기")]
    public void 장난감뽑기_패스()
    {
        PDManager.Data.Benefit.AddBenefit(401401);
    }

    [Category("장난감뽑기")]
    public int totemDrawLevel = 1;

    [Category("장난감뽑기")]
    public int TotemDrawLevel
    {
        get => this.totemDrawLevel;
        set => this.totemDrawLevel = Math.Min(value, 41);
    }
    [Category("장난감뽑기")]
    public void 특정_장난감뽑기_레벨()
    {
        PDManager.Data.TotemDraw.Level = TotemDrawLevel;
    }

    [Category("장난감뽑기")]
    public int totemDrawUpgrade = 1;

    [Category("장난감뽑기")]
    public int TotemDrawUpgrade
    {
        get => this.totemDrawUpgrade;
        set => this.totemDrawUpgrade = Math.Min(value, 20);
    }
    [Category("장난감뽑기")]
    public void 특정_장난감뽑기_업그레이드_레벨()
    {
        PDManager.Data.TotemDraw.Upgrade = TotemDrawUpgrade;
    }


    [Category("장난감뽑기_변경")]
    public eStat baseStat = eStat.SKILL_COOL_DOWN_RATE;
    [Category("장난감뽑기_변경")]
    public eStat BaseStat
    {
        get => this.baseStat;
        set => this.baseStat = value;
    }
    [Category("장난감뽑기_변경")]
    public eStat convertStat = eStat.FINAL_ATK_RATE;
    [Category("장난감뽑기_변경")]
    public eStat ConvertStat
    {
        get => this.convertStat;
        set => this.convertStat = value;
    }
    [Category("장난감뽑기_변경")]
    public int convertMulti = 100;

    [Category("장난감뽑기_변경")]
    public int ConvertMulti
    {
        get => this.convertMulti;
        set => this.convertMulti = value;
    }

#if UNITY_EDITOR
    [Category("장난감뽑기_변경")]
    public void 특정_장난감뽑기_변경()
    {
        PDManager.Data.TotemDraw.AllConvertSubStat(BaseStat, ConvertStat, ConvertMulti);
    }

    [Category("장난감검증")]
    public eStat checkSubStat = eStat.CRITICAL_RATE;
    [Category("장난감검증")]
    public eStat CheckSubStat
    {
        get => this.checkSubStat;
        set => this.checkSubStat = value;
    }

    [Category("장난감검증")]
    public int totemDrawTestLevel = 15;

    [Category("장난감검증")]
    public int TotemDrawTestLevel
    {
        get => this.totemDrawTestLevel;
        set => this.totemDrawTestLevel = Math.Min(value, 40);
    }

    [Category("장난감검증")]
    public void 장난감뽑기_검증_CRITICAL_RATE()
    {
        TotemDrawSystem.CheckSubStat(eStat.CRITICAL_RATE, totemDrawTestLevel);
    }

    [Category("장난감검증")]
    public void 장난감뽑기_검증_WEAKNESS_RATE()
    {
        TotemDrawSystem.CheckSubStat(eStat.WEAKNESS_RATE, totemDrawTestLevel);
    }
    [Category("장난감검증")]
    public void 장난감뽑기_검증_REDUCTION_RATE()
    {
        TotemDrawSystem.CheckSubStat(eStat.REDUCTION_RATE, totemDrawTestLevel);
    }

    [Category("장난감검증")]
    public void 장난감뽑기_검증_ATK_SPEED_RATE()
    {
        TotemDrawSystem.CheckSubStat(eStat.ATK_SPEED_RATE, totemDrawTestLevel);
    }

    [Category("장난감검증")]
    public void 장난감뽑기_검증()
    {
        TotemDrawSystem.CheckSubStat(CheckSubStat, totemDrawTestLevel);
    }

#endif

    #endregion

    #region 특성

    [Category("특성")]
    public async void 특성_모두_마스터()
    {
        foreach (var spec in Facade.Spec.SpecTraitTree.All)
        {
            PDManager.Data.Trait.AddTrait(spec);
        }
    }


    #endregion

    #region 보물

    [Category("보물")]
    public void 모든_보물_획득()
    {
        foreach (var data in PDManager.Data.Treasure.Treasures)
        {
            data.AddAmount(1);
        }
    }

    #endregion

    #region 유물

    [Category("유물")]
    public void 노말_유물_100개_획득()
    {
        foreach (var data in PDManager.Data.Relic.Relics.Where(x=>x.SpecItem.rarity == 1))
            data.AddAmount(100);
    }
    [Category("유물")]
    public void 레어_유물_100개_획득()
    {
        foreach (var data in PDManager.Data.Relic.Relics.Where(x=>x.SpecItem.rarity == 2))
            data.AddAmount(100);
    }

    [Category("유물")]
    public void 에픽_유물_100개_획득()
    {
        foreach (var data in PDManager.Data.Relic.Relics.Where(x=>x.SpecItem.rarity == 3))
            data.AddAmount(100);
    }

    [Category("유물")]
    public void 전설_유물_100개_획득()
    {
        foreach (var data in PDManager.Data.Relic.Relics.Where(x=>x.SpecItem.rarity == 4))
            data.AddAmount(100);
    }


    #endregion

    #region 약관동의
    [Category("약관동의")]
    public void 약관동의_팝업_오픈()
    {
        PopupManager.Show(ePopupType.AGREEMENT);
    }
    #endregion

    #region 광고버프

    [Category("광고버프")]
    public void 광고버프_팝업_오픈()
    {
        PopupManager.Show(ePopupType.AD_BUFF);
    }

    [Category("광고버프")]
    public void 광고버프_적용()
    {
        foreach (var data in PDManager.Data.AdBuff.Buffs)
            AdBuffSystem.StartBuff(data);
    }

    [Category("광고버프")]
    public void 광고버프_해제()
    {
        foreach (var data in PDManager.Data.AdBuff.Buffs)
        {
            data.LeftTimeSec = 0;
        }
    }

    [Category("광고버프")]
    public void 광고버프_레벨업()
    {
        foreach (var data in PDManager.Data.AdBuff.Buffs)
        {
            var diff = data.NeedExp - data.Exp;
            data.AddCount(diff);
            data.SetExp(data.Exp + diff);
        }
    }

    #endregion

    #region 경험치

    [Category("경험치")]
    public string exp = "10000";
    public string level = "10";

    [Category("경험치")]
    public string Exp
    {
        get => this.exp;
        set => this.exp = value;
    }

    [Category("경험치")]
    public async void 경험치_획득()
    {
        if (BigInteger.TryParse(exp, out BigInteger _result))
        {
            ItemSystem.Add((int)ePointType.EXP,(long)_result, "", "");
        }
        else
        {
            currencyAmount = "0";
        }
    }

    [Category("레벨")]
    public string Level
    {
        get => this.level;
        set => this.level = value;
    }

    [Category("레벨")]
    public void 레벨_설정()
    {
        if (BigInteger.TryParse(level, out BigInteger _result))
        {
            PDManager.Data.Level.SetLevel((int)_result);
            //ItemSystem.Add((int)ePointType.EXP,(long)_result, "", "");
        }
        else
        {
            currencyAmount = "0";
        }
    }

    #endregion

    #region VIP
    [Category("VIP")]
    public async void VIP_데일리_초기화()
    {
        PDManager.Data.VIP.AddDay();
    }
    #endregion


    #region 코스튬

    public int unitId = 103401;
    public int costumeId = 10340101;

    [Category("코스튬")]
    public int UnitId
    {
        get => this.unitId;
        set => this.unitId = value;
    }

    [Category("코스튬")]
    public int CostumeId
    {
        get => this.costumeId;
        set => this.costumeId = value;
    }

    [Category("코스튬")]
    public void 코스튬_장착()
    {
        if (!PDManager.Data.Costume.DicAllCostumes[CostumeId].IsUnlocked)
        {
            PDManager.Data.Costume.DicAllCostumes[CostumeId].AddAmount(1);
        }

        PDManager.Data.Unit.DicUnits[UnitId].EquipCostume(CostumeId);
    }

    [Category("코스튬")]
    public void 코스튬_해제()
    {
        PDManager.Data.Unit.DicUnits[UnitId].EquipCostume(0);
    }

    #endregion


    #region 스테이트머신

    [Category("스테이트머신")]
    public void 강제_스테이트_해제()
    {
        foreach (var unit in UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits)
        {
            unit.FsmModule.ForceState = eFsmState.None;
            unit.IsForceHeroNormalAttack = false;
            unit.IsForceHeroStrongAttack = false;
            unit.IsForceHeroActiveAttack = false;
        }
        PopupManager.ShowToast("영웅의 스테이트를 자율로 만듭니다.");
    }

    [Category("스테이트머신")]
    public void 강제_스테이트_IDLE()
    {
        foreach (var unit in UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits)
        {
            unit.FsmModule.ForceState = eFsmState.IDLE;
            unit.IsForceHeroNormalAttack = false;
            unit.IsForceHeroStrongAttack = false;
            unit.IsForceHeroActiveAttack = false;
        }
        PopupManager.ShowToast("영웅의 스테이트를 강제로 Idle 상태로 만듭니다.");
    }

    [Category("스테이트머신")]
    public void 강제_스테이트_ATTACK()
    {
        foreach (var unit in UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits)
        {
            unit.FsmModule.ForceState = eFsmState.ATTACK;
            unit.IsForceHeroNormalAttack = true;
            unit.IsForceHeroStrongAttack = false;
            unit.IsForceHeroActiveAttack = false;
        }
        PopupManager.ShowToast("영웅의 스테이트를 강제로 일반 공격 상태로 만듭니다.");
    }

    [Category("스테이트머신")]
    public void 강제_스테이트_STRONG()
    {
        foreach (var unit in UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits)
        {
            unit.FsmModule.ForceState = eFsmState.ATTACK;
            unit.IsForceHeroNormalAttack = false;
            unit.IsForceHeroStrongAttack = true;
            unit.IsForceHeroActiveAttack = false;
        }
        PopupManager.ShowToast("영웅의 스테이트를 강제로 강화 공격 상태로 만듭니다.");
    }

    [Category("스테이트머신")]
    public void 강제_스테이트_ACTIVE()
    {
        foreach (var unit in UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits)
        {
            unit.FsmModule.ForceState = eFsmState.ATTACK;
            unit.IsForceHeroNormalAttack = false;
            unit.IsForceHeroStrongAttack = false;
            unit.IsForceHeroActiveAttack = true;
        }
        PopupManager.ShowToast("영웅의 스테이트를 강제로 액티브 공격 상태로 만듭니다.");
    }

    #endregion

    #region 전투

    [Category("전투")]
    public void 영웅_무적_ON_OFF()
    {
        InGameManager.IsHeroNoDamage = !InGameManager.IsHeroNoDamage;
    }

    [Category("전투")]
    public void 히어로_한마리_죽이기()
    {
        if (UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].IsLivingUnitEmpty)
            return;

        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];

        DamageData damageData = new(hero, long.MaxValue, false, hero.CachedTr.position);
        damageData.isForceKill = true;

        hero.Damage(damageData);
    }

    [Category("전투")]
    public void 적_모두_죽이기()
    {
        if (UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].IsLivingUnitEmpty)
            return;

        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];

        foreach (var enemy in UnitSpawner.DicSpawnInfo[eUnitGroup.ENEMY].LivingUnits.ToList())
        {
            DamageData damageData = new(hero, long.MaxValue, false, hero.CachedTr.position);
            damageData.isForceKill = true;

            enemy.Damage(damageData);
        }
    }

    [Category("전투")]
    public void 모든_유닛_공속맥스_버프()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.stat_atk_speed, 5000000, 0, 0, 1, 0, 10000, 5000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 모든_유닛_이속맥스_버프()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.stat_move_speed, 5000000, 0, 0, 1, 0, 10000, 5000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 모든_영웅_쿨감()
    {
        Facade.Player.StatModule.StatModel.Add(eStat.SKILL_COOL_DOWN_RATE, 100000,"TEST", eStatLayer.BASIC);
    }

    [Category("전투")]
    public void 모든_유닛_빙결()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.cc_frozen, 5000, 1000, 0, 1, 0, 10000, 5000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 모든_유닛_침묵()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.cc_silence, 0, 0, 0, 1, 0, 10000, 5000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 모든_유닛_쉴드()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.shield, 5, 9000, 0, 1, 0, 10000, 5000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 모든_유닛_부활버프()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.rebirth, 5000, 5000, 0, 1, 0, 10000, 10000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 모든_유닛_스턴()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        foreach (UnitBase unit in UnitSpawner.ListAllUnit)
        {
            var data = new List<SkillAbilityData>();
            data.Add(new SkillAbilityData(hero, uid, eSkillAbility.cc_stun, 0, 0, 0, 1, 0, 10000, 5000));
            unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
        }
    }

    [Category("전투")]
    public void 랜덤_유닛_스턴()
    {
        var hero = UnitSpawner.DicSpawnInfo[eUnitGroup.HERO].LivingUnits[0];
        var uid = UnitBuffModule.GenerateUid();
        var unit = Utils.RandomPick(UnitSpawner.ListAllUnit.ToList());
        var data = new List<SkillAbilityData>();
        data.Add(new SkillAbilityData(hero, uid, eSkillAbility.cc_stun, 0, 0, 0, 1, 0, 10000, 5000));
        unit.Module<UnitBuffModule>().CheckSkillBuff(null, data);
    }

    [Category("전투")]
    public void 네비메시_빌드()
    {
        RuntimeBakeMesh.Instance._BuildNavMesh().Forget();
    }

    [Category("전투")]
    public void 몹_킬수_표기_OnOff()
    {
        StageUI.IsShowEnemyKillCountForTest = !StageUI.IsShowEnemyKillCountForTest;
    }

    [Category("전투")]
    public void 몹_킬수_표기_초기화()
    {
        StageBattleController.TotalStageKillCount = 0;
    }

    #endregion

    #region 던전

    [Category("던전")]
    public eStageType dungeonType = eStageType.DUNGEON_GOLD;

    [Category("던전")]
    public int dungeonLevel = 1;

    [Category("던전")]
    public eStageType DungeonType
    {
        get => this.dungeonType;
        set => this.dungeonType = value;
    }

    [Category("던전")]
    public int DungeonLevel
    {
        get => this.dungeonLevel;
        set => this.dungeonLevel = value;
    }

    [Category("던전")]
    public void 던전_입장()
    {
        DungeonSystem.EnterDungeon(DungeonType, DungeonLevel);
    }
    [Category("던전")]
    public void 던전_레벨_클리어()
    {
        var dungeonData = PDManager.Data.Dungeon.GetData(dungeonType);
        dungeonData.ClearDungeon(this.dungeonLevel);
    }

    #endregion

    #region 상점

    [Category("상점")]
    public void 모든_상품_구매_횟수_초기화()
    {
        foreach (var data in PDManager.Data.Shop.ShopData)
            data.InitLeftCount();
    }

    [Category("상점")]
    public void 모든_상품_언락()
    {
        PDManager.Data.Purchase.UnlockedIds.Clear();
        foreach (var spec in Facade.Spec.SpecShop.All.Where(x => x.condition.unlock_condition is not ePurchaseUnlockType.NONE))
        {
            PDManager.Data.Shop.GetData(spec.id).InitLeftCount();
            PurchaseUnlockSystem.UnlockProductForCheat(spec.id);
        }
    }

    [Category("상점")]
    public void 모든_상품_언락_초기화()
    {
        PDManager.Data.Purchase.UnlockedIds.Clear();
        foreach (var spec in Facade.Spec.SpecShop.All.Where(x => x.condition.unlock_condition is not ePurchaseUnlockType.NONE))
        {
            PDManager.Data.Shop.GetData(spec.id).InitLeftCount();
        }
    }

    #endregion

#region 랜덤박스

    [Category("랜덤박스")]
    public int randomBoxGroupId = 1001;

    [Category("랜덤박스")]
    public int RandomBoxGroupId
    {
        get => this.randomBoxGroupId;
        set => this.randomBoxGroupId = value;
    }

    [Category("랜덤박스")]
    public void 랜덤박스_1회()
    {
        SimulateRandomBox(1);
    }

    [Category("랜덤박스")]
    public void 랜덤박스_100회_시뮬레이션()
    {
        SimulateRandomBox(100);
    }

    [Category("랜덤박스")]
    public void 랜덤박스_1000회_시뮬레이션()
    {
        SimulateRandomBox(1000);
    }

    [Category("랜덤박스")]
    public void 랜덤박스_10000회_시뮬레이션()
    {
        SimulateRandomBox(10000);
    }

    [Category("랜덤박스")]
    public void 랜덤박스_100000회_시뮬레이션()
    {
        SimulateRandomBox(100000);
    }

    private void SimulateRandomBox(int count)
    {
        List<SimulateRandomBoxInfo> report = new();

        var result = RandomBoxSystem.RewardSeed(RandomBoxGroupId, 1, count);
        foreach (var temp in result)
        {
            if (!report.Exists(x => x.id == temp.specBox.id))
            {
                report.Add(new SimulateRandomBoxInfo()
                {
                    id = temp.specBox.id,
                    count = 1,
                });
            }
            else
            {
                report.FirstOrDefault(x => x.id == temp.specBox.id)!.count++;
            }
        }

        report.Sort((x, y) => x.id.CompareTo(y.id));

        foreach (var temp in report)
        {
            Debug.LogColor($"시뮬레이션: {temp.id}번 박스 {temp.count}회 획득. 확률={(double) temp.count / result.Count}");
        }
    }

    private class SimulateRandomBoxInfo
    {
        public int id;
        public int count;
    }

    #endregion

    #region 임시

    [Category("카메라")]
    public void 메인_카메라_온오프()
    {
        CameraManager.MainCam.enabled = !CameraManager.MainCam.enabled;
    }

    [Category("카메라")]
    public void UI_카메라_온오프()
    {
        InGameManager.MainCanvas.gameObject.SetActive(!InGameManager.MainCanvas.gameObject.activeSelf);
    }

    [Category("임시")]
    public void 하드커런시_정합성_테스트()
    {
        //서버 아이템 지급 부분
        GameGrpcManager.Instance.GEarnedHardCurrency(new List<HardCurrency> {new() {Type = (int) eCurrencyType.GEM, Amount = 1000}}, false, null).Forget();
        PDManager.RequestSave(eUserData.Currency);
        PDManager.SaveImmediately();
    }

    [Category("임시")]
    public void 광고_보기()
    {
        AdNetworkManager.Instance.ShowAd("");
    }

    [Category("임시")]
    public void 영웅_소환_경험치_1000_증가()
    {
        PDManager.Data.Summon.GetData(eSummonType.HERO).SetExp(PDManager.Data.Summon.GetData(eSummonType.HERO).Exp + 1000);
    }

    [Category("임시")]
    public void 보물_소환_경험치_1000_증가()
    {
        PDManager.Data.Summon.GetData(eSummonType.TREASURE).SetExp(PDManager.Data.Summon.GetData(eSummonType.TREASURE).Exp + 1000);
    }

    #endregion

    #region 테스트

    [Category("테스트")]
    public async void 데이터_즉시_저장_로드_후_재화비교()
    {
        Debug.LogError(PDManager.Data.Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).Amount + "  " +
                       PDManager.Data.Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).AccAdd);

        await PDManager.SaveImmediatelyAll();

        var types = new List<string>();
        foreach (eUserData fruit in Enum.GetValues(typeof(eUserData)))
        {
            types.AddUnique(fruit.ToEnumString());
        }
        var response = await GameGrpcManager.Instance.GGetUserData(types);

        PlayerData playerData = new();
        foreach (var info in response.PlayerDatas)
        {
            var category = (eUserData) Enum.Parse(typeof(eUserData), info.Key);

            // 캐릭터 데이터
            response.PlayerDatas.TryGetValue(info.Key, out string characterData);
            playerData.OverrideData(category, characterData);
        }

        playerData.UpdateDicData();

        Debug.LogError(playerData.Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).Amount + "  " +
                       playerData.Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).AccAdd);
    }

    [Category("테스트")]
    public async void 데이터_암복호화_후_재화비교()
    {
        Debug.LogError(PDManager.Data.Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).Amount + "  " +
                       PDManager.Data.Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).AccAdd);


        var str = MessageUtility.ToBase64String(PDManager.Data.Currency);
        var Currency = MessageUtility.FromBase64String<PlayerCurrency>(str);


        Debug.LogError(Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).Amount + "  " +
                       Currency.Items.FirstOrDefault(x=>(eCurrencyType)x.Id == eCurrencyType.GOLD).AccAdd);
    }

    [Category("테스트")]
    public async void 랭킹조회()
    {
        GetPlayerRankingDataResponse response = await GameGrpcManager.Instance.GGetPlayerRankingData("stage", 10);
        foreach (PlayerRankingData responseRankingData in response.RankingDatas)
        {
            var info = Utils.ConvertStrToProfileInfo(responseRankingData.ProfileInfo);
            var test = 0;
        }
    }

    [Category("테스트")]
    public async void 뉴비퀘스트_완료()
    {
        var rewardBundle = new RewardBundle(true);
        foreach (var data in PDManager.Data.Quest.NewbieQuests)
        {
            if (!data.IsRewardDone)
                data.SetRewardNormal(true, rewardBundle);
        }

        ItemSystem.ShowReward(rewardBundle);
    }

    [Category("테스트")]
    public async void 우편함_가져오기()
    {
        PostSystem.UpdatePlayerPostData();
    }

    [Category("테스트")]
    public void 레드닷_전체_갱신()
    {
        RedDotSystem.AddRequest(eRedDotFlag.ALL);
    }

    #endregion
}
#endif
