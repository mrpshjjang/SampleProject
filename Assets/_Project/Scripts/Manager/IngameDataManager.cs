using System;
using TAGUNI;
using System.Collections.Generic;

public class IngameDataManager : TSingleton<IngameDataManager>
{
    public int Level  { get; private set; }
    public eClearType ClearType  { get; private set; }
    public int ClearValue  { get; private set; }

    private int _currentScore;
    public int CurrentScore
    {
        get
        {
            return _currentScore;
        }
        private set
        {
            _currentScore = value;
            UpdateCurrentScore?.Invoke(_currentScore);
        }
    }

    public Action<int> UpdateCurrentScore;

    public int MaxBossHP { get; private set; }

    private int _currentBossHP;
    public int CurrentBossHP
    {
        get
        {
            return _currentBossHP;
        }
        private set
        {
            _currentBossHP = value;
            UpdateCurrentBossHP?.Invoke(_currentBossHP);
        }
    }

    public Action<int> UpdateCurrentBossHP;

    private int _currentBubbleCount;
    public int CurrentBubbleCount
    {
        get
        {
            return _currentBubbleCount;
        }
        private set
        {
            _currentBubbleCount = value;
            UpdateCurrentBubbleCount?.Invoke(_currentBubbleCount);
        }
    }

    public Action<int> UpdateCurrentBubbleCount;

    public void InitData(int level)
    {
        Level = level;
        CurrentScore = 0;
        var specMapConfig = Facade.Spec.Data.SpecMapConfig.Find(e => e.level == level);
        CurrentBubbleCount = specMapConfig.bubble_cnt;
        ClearValue = specMapConfig.clear_value;
        ClearType = specMapConfig.clear_type;
        CheckClearTypeCustomData(specMapConfig.clear_type);
    }

    private void CheckClearTypeCustomData(eClearType type)
    {
        switch (type)
        {
            case eClearType.BOSS_HP:
                MaxBossHP = GetBossHP();
                CurrentBossHP = MaxBossHP;
                break;
        }
    }

    public int GetBossHP()
    {
        var lstSpec = Facade.Spec.Data.SpecMap.FindAll(e => e.level == Level);
        foreach (var spec in lstSpec)
        {
            var specBubble = Facade.Spec.Data.SpecBubble.Find(e => e.id == spec.bubble);
            if (specBubble.type == eBubbleType.BOSS)
            {
                var specAttribute = Facade.Spec.Data.SpecBubbleAttribute.Find(e => e.attribute == specBubble.attribute);
                return specAttribute.value;
            }
        }
        return 100;
    }

    public void AttackBoss(int hp)
    {
        hp = Math.Min(hp, CurrentBossHP);
        CurrentBossHP -= hp;
    }

    public override void Init()
    {
        base.Init();

        CurrentScore = 0;
    }

    public void Dispose()
    {

    }

    public void SetBubbleAddCount (int addCount)
    {
        CurrentBubbleCount += addCount;
    }
}
