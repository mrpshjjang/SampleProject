using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Sigtrap.Relays;
using UnityEngine;

public class TimeManager : GameObjectSingleton<TimeManager>
{
    #region public

    public static readonly Relay OnTick = new();
    public static readonly Relay OnQuarterTick = new();
    public static readonly Relay OnNextHour = new();
    public static readonly Relay OnNextDay = new();
    public static readonly Relay OnNextWeekly = new();
    public static readonly Relay OnNextMonthly = new();

    //Unix Time Base
    internal static readonly DateTime Jan1st1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    //현재시간(DateTime)
    internal static DateTime Now => DateTime.UtcNow.AddSeconds(gapSecond + UserConfig.Instance.TimeAdder);

    //내일자정(DateTime)
    internal static DateTime Tomorrow => new DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
    internal static DateTime NextWeeklyMonday => new DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Now.DayOfWeek == 0 ? 1 : (7 - (int)Now.DayOfWeek + 1));
    internal static DateTime NextMonthFirstDay => new DateTime(Now.Month == 12 ? Now.Year + 1 : Now.Year, Now.Month == 12 ? 1 : Now.Month + 1, 1, 0, 0, 0, DateTimeKind.Utc);

    //현재시간(Unix 초) => 유저 데이터 저장은 이걸 활용할 것
    internal static long UnixSeconds => (long)((Now - Jan1st1970).TotalSeconds);

    //타임스케일
    internal static float BaseTimeScale => baseTimeScale;
    internal static float MultTileScale => multTimeScale;
    internal static float BuffTimeScale => buffTimeScale;
    internal static float StageTimeScale => stageTimeScale;
    internal static bool IsNetworkFine { get; private set; } = true;
    internal static bool IsStopTick = false;

    //프레임레이트
    internal static int FrameRate = 0;


    internal static void SetServerTime(long serverTime)
    {
        gapSecond = serverTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        prevSystemTimeSecondTick = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        inst.StopAllCoroutines();
        inst.StartCoroutine(nameof(TickProcess));
        inst.StartCoroutine(nameof(QuarterTickProcess));

        Debug.Log($"TimeManager:: Set Successfully. Now DateTime is {Now}");
    }

    internal static void ResetTimeScale()
    {
        baseTimeScale = 1;
        multTimeScale = 1;
        buffTimeScale = 1;
        stageTimeScale = 1;
        RefreshTimeScale();
    }

    internal static void SetMultTimeScale(float t)
    {
        multTimeScale = t;

        RefreshTimeScale();
    }

    internal static void SetBaseTimeScale(float t)
    {
        baseTimeScale = UserConfig.Instance.TimeScaleBase * t;

        RefreshTimeScale();
    }

    internal static void SetBuffTimeScale(float t)
    {
        buffTimeScale = t;

        RefreshTimeScale();

    }
    internal static void SetStageTimeScale(float t)
    {
        stageTimeScale = t;

        RefreshTimeScale();
    }

    internal static void SetNetworkState(bool active)
    {
        Debug.LogError($"SetNetworkState: {active}");
        IsNetworkFine = active;

        RefreshTimeScale();
    }

    public void StopTimeManager()
    {
        inst.StopAllCoroutines();
        OnTick?.RemoveAll();
        OnQuarterTick?.RemoveAll();
        OnNextHour?.RemoveAll();
        OnNextDay?.RemoveAll();
        OnNextWeekly?.RemoveAll();
        StopAllCoroutines();
    }

    #endregion


    #region protected

    #endregion


    #region private

    private const long SYSTEM_TIME_UPDATE_GAP = 60;
    private const int TIME_CHANGE_REPORT_THRES = 5;

    private static int lastCheckedHours = 0;
    private static long gapSecond = 0;
    private static long sessionSecond = 0;
    private static long prevSystemTimeSecondTick = 0;
    private static int TimeChangeCount = 0;

    //TimeScale
    private static float baseTimeScale = 1;
    private static float multTimeScale = 1;
    private static float buffTimeScale = 1;
    private static float stageTimeScale = 1;

    //frameRate
    private float deltaTime = 0.0f;

    private IEnumerator TickProcess()
    {
        WaitForSecondsRealtime delay = new(1.0f / UserConfig.Instance.TimeScaleBase);

        //yield return new WaitUntil(() => PDManager.IsInitialized);
        //yield return new WaitUntil(() => StaticManagerController.IsSystemInitialize);

        while (true)
        {
            OnTick?.Dispatch();
            CheckNextHour();
            CheckNextDay();

            sessionSecond++;
            //PDManager.Data.Info.TotalPlaySecond++;

            yield return delay;
        }
    }

    private IEnumerator QuarterTickProcess()
    {
        WaitForSecondsRealtime delay = new(0.25f / UserConfig.Instance.TimeScaleBase);

        //yield return new WaitUntil(() => PDManager.IsInitialized);

        while (true)
        {
            //시간 변조 검사
            var curSystemTimeSecond = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var diff = Math.Abs(prevSystemTimeSecondTick - curSystemTimeSecond);
            if (prevSystemTimeSecondTick != 0 && diff > SYSTEM_TIME_UPDATE_GAP)
            {
                Debug.LogError($"TimeManager:: User System time gap is detected. gap is {diff}");

                TimeChangeCount++;
                if (TimeChangeCount % TIME_CHANGE_REPORT_THRES == 0)
                {
                    TimeChangeCount = 0;
                }

                //StopAndUpdateServerTime();
                yield break;
            }
            prevSystemTimeSecondTick = curSystemTimeSecond;

            OnQuarterTick?.Dispatch();

            yield return delay;
        }
    }

    private void CheckNextHour()
    {
        if (lastCheckedHours != Now.Hour)
        {
            lastCheckedHours = Now.Hour;
            OnNextHour?.Dispatch();
        }
    }

    private void CheckNextDay()
    {
        // long lastDays = PDManager.Data.Info.LastLoginTimeSecond.ToDateTime().ToDays();
        // long nowDays = Now.ToDays();
        //
        // if (PDManager.Data.Info.LastLoginTimeSecond == 0 || lastDays != nowDays)
        // {
        //     PDManager.Data.Info.LastLoginTimeSecond = UnixSeconds;
        //     ContentResetSystem.ContentReset();
        //     OnNextDay?.Dispatch();
        // }
    }

    private static void RefreshTimeScale()
    {
        if (IsNetworkFine)
        {
            Time.timeScale = baseTimeScale * multTimeScale * buffTimeScale * stageTimeScale;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    #endregion


    #region lifecycle

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FrameRate = (int)fps;
    }

    protected override void OnDestroy()
    {
        StopTimeManager();
        base.OnDestroy();
    }

    #endregion
}
