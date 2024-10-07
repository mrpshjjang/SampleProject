using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(UserConfig))]
public class UserConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (UserConfig)target;

        if (GUILayout.Button("시간 스케일 적용", GUILayout.Height(40)))
        {
            script.ApplyTimeScale();
        }

        if (GUILayout.Button("유저 데이터 삭제", GUILayout.Height(40)))
        {
            script.RemoveLocalData();
        }
    }
}
#endif


public class UserConfig : ScriptableObject
{
    private static UserConfig instance = null;

    public static UserConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("UserConfig") as UserConfig;
            }

            return instance;
        }
    }

    #region public

    //게임 시간
    public long TimeAdder => IS_NOT_EDITOR ? 0 : 게임_시간_일 * 86400 + 게임_시간_시 * 3600 + 게임_시간_분 * 60 + 게임_시간_초;
    public float TimeScaleBase => IS_NOT_EDITOR ? 1f : 시간_배속;

    //로컬 데이터
#if __DEV
    public bool IsLoadLocalSpec => 로컬_스펙_로드;
#else
    public bool IsLoadLocalSpec => false;
#endif

    public bool CanPlatformLogin => 플랫폼_로그인_활성화;
    //전투
    public bool LineDraw => IS_NOT_EDITOR ? false : 공격_영역_보기;
    public bool NoSkillApSpend => IS_NOT_EDITOR ? false : 스킬_AP_소모_없음;
    public bool NoSkillCoolTime => IS_NOT_EDITOR ? false : 스킬_쿨타임_없음;
    public bool HeroNoDamage => IS_NOT_EDITOR ? false : 영웅_무적;
    public bool EnemyNoDamage => IS_NOT_EDITOR ? false : 몬스터_무적;
    public bool InfiniteDungeonTimer => IS_NOT_EDITOR ? false : 던전_타이머_무한;
    public bool IsShowHeroDamageLog => IS_NOT_EDITOR ? false : 아군_데미지_로그_출력;
    public bool IsShowEnemyDamageLog => IS_NOT_EDITOR ? false : 적군_데미지_로그_출력;
    public bool BossSummonButtonAnyWhere => 보스_소환_버튼_항상_등장;

    //유닛 테스트
    public bool IsSpawnCheatUnit => IS_NOT_EDITOR ? false : 아래_ID의_유닛만_소환;
    public List<int> ListCheatSpawnUnitId => IS_NOT_EDITOR ? new List<int>() : 소환할_유닛_ID_리스트;

    //테스트 편의성
    public bool IsBlockOfflineReward => IS_NOT_EDITOR ? false : 오프라인_보상_팝업_차단;
    public bool IsBlockPurchaseOffer => IS_NOT_EDITOR ? false : 상품_제안_팝업_차단;
    public bool IsSkipPrologue => IS_NOT_EDITOR ? false : 프롤로그_스킵;
    public bool IsShowCombatToastMoreDetailed => IS_NOT_DEV ? false : 전투력_팝업_심화;
    public bool IsHardCurrencyOff => IS_NOT_DEV ? false : 하드커런시_로직_OFF;
    public bool IsLockBackGroundRunning => IS_NOT_DEV ? false : 에디터_백그라운드_실행_차단;

    //플레이어 데이터
#if UNITY_EDITOR
    public TextAsset PlayerDataJson => IS_NOT_EDITOR ? null : 불러올_데이터;
#endif

    public void ApplyTimeScale()
    {
        //TimeManager.SetBaseTimeScale(1.0f);
        //TimeManager.Instance.StopAndUpdateServerTime();
    }

    public void RemoveLocalData()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion


    #region private

#if UNITY_EDITOR
    private const bool IS_NOT_EDITOR = false;
#else
    private const bool IS_NOT_EDITOR = true;
#endif

#if __DEV
    private const bool IS_NOT_DEV = false;
#else
    private const bool IS_NOT_DEV = true;
#endif

    [Header("테스트용 파라미터")]
    [SerializeField] public string TestString = string.Empty;
    [SerializeField] public float TestFloat1 = 0;
    [SerializeField] public float TestFloat2 = 0;

    [Header("게임 시간")]
    [SerializeField] private long 게임_시간_초 = 0;
    [SerializeField] private long 게임_시간_분 = 0;
    [SerializeField] private long 게임_시간_시 = 0;
    [SerializeField] private long 게임_시간_일 = 0;
    [SerializeField] private float 시간_배속 = 1f;

    [Header("로컬 데이터")]
    [SerializeField] private bool 로컬_스펙_로드 = false;
    [SerializeField] private bool 플랫폼_로그인_활성화 = false;

    [Header("전투")]
    [SerializeField] private bool 공격_영역_보기 = false;
    [SerializeField] private bool 스킬_AP_소모_없음 = false;
    [SerializeField] private bool 스킬_쿨타임_없음 = false;
    [SerializeField] private bool 강화공격만_사용 = false;
    [SerializeField] private bool 아군_액티브스킬만_사용 = false;
    [SerializeField] private bool 적군_액티브스킬만_사용 = false;
    [SerializeField] private bool 영웅_무적 = false;
    [SerializeField] private bool 몬스터_무적 = false;
    [SerializeField] private bool 던전_타이머_무한 = false;
    [SerializeField] private bool 아군_데미지_로그_출력 = false;
    [SerializeField] private bool 적군_데미지_로그_출력 = false;
    [SerializeField] private bool 보스_소환_버튼_항상_등장 = false;

    [Header("유닛 테스트")]
    [SerializeField] private bool 아래_ID의_유닛만_소환 = false;
    [SerializeField] private List<int> 소환할_유닛_ID_리스트 = new();

    [Header("테스트 편의성")]
    [SerializeField] private int 오프라인_분 = 0;
    [SerializeField] private bool 오프라인_보상_팝업_차단 = false;
    [SerializeField] private bool 상품_제안_팝업_차단 = false;
    [SerializeField] private bool 프롤로그_스킵 = false;
    [SerializeField] private bool 전투력_팝업_심화 = false;
    [SerializeField] private bool 하드커런시_로직_OFF = false;
    [SerializeField] private bool 에디터_백그라운드_실행_차단 = false;

#if UNITY_EDITOR
    [Header("플레이어 데이터")] [SerializeField] private TextAsset 불러올_데이터;
#endif

    #endregion


    #region lifecycle

    #endregion

    [Serializable]
    public class PoolContainer
    {
        public string poolName;
        public List<UnityEngine.Object> directoryList = new();
    }
}
