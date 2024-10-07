using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Unity.Collections;

public class SplashManager : GameObjectSingleton<SplashManager>
{
    #region public

    //스플레시 상황 정보
    public bool IsResourceLoadDone { get; private set; }
    public bool IsPlayerDataLoadDone  { get; private set; }
    public bool IsInitializeDone { get; private set; }
    public bool IsEnteringGame  { get; private set; }
    public string CurrentRegion  { get; private set; }

    //로딩 진행 정보
    public float Progress { get; private set; }
    public static long LoadingStartUnixSec { get; private set; }


    private async UniTask Init()
    {
        Screen.fullScreen = true;                                //풀스크린
        QualitySettings.vSyncCount = 0;                          //VSync 비활성화
        Application.targetFrameRate = 60;                        //프레임레이드 60
        Screen.sleepTimeout = SleepTimeout.NeverSleep;           //슬립 없도록
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled; //가비지 콜렉팅 활성화

// #if UNITY_EDITOR
//         Application.runInBackground = !UserConfig.Instance.IsLockBackGroundRunning;
// #else
//         Application.runInBackground = false;
// #endif

        //로딩 관련 파라미터 초기화
        IsResourceLoadDone = false;
        IsPlayerDataLoadDone = false;
        IsInitializeDone = false;
        IsEnteringGame = false;

        await AddressableManager.Instance.InitAddressable();

        await LoadResource();

        if (!IsResourceLoadDone)
            return;

        IsInitializeDone = true;
    }

    public void StartEntering()
    {
        EnterGame().Forget();
    }


    #endregion


    #region private

    private float _responseTime;
    private AsyncOperation _asyncOperation;

    // [SerializeField] private SplashUI splashUI;

    /// <summary>
    /// 로딩 과정 1 : 리소스 로딩
    /// </summary>
    private async UniTask LoadResource()
    {
        Debug.LogColor($"Splash Initialize Start. Device id is {SystemInfo.deviceUniqueIdentifier}", Color.yellow);

        SpecDataManager.Instance.Clear();
        SpecDataManager.Instance.LoadSpecData();

        await AddressableManager.Instance.CheckLoadAddressable();
    }


    #endregion


    #region lifecycle

    private void Update()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        var currentRegion = RegionInfo.CurrentRegion;
        CurrentRegion = currentRegion != null ? currentRegion.TwoLetterISORegionName : "US";

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        Init().Forget();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnApplicationQuit()
    {
        if (_asyncOperation != null && !_asyncOperation.isDone)
        {
            _asyncOperation.allowSceneActivation = false;
        }

        if (SceneManager.GetSceneByName("Game").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Game");
        }
    }

    /// <summary>
    /// 게임 진입
    /// </summary>
    private async UniTask EnterGame()
    {
        // if (!IsPlayerDataLoadDone || IsEnteringGame)
        //     return;

        if (_asyncOperation == null)
        {
            _asyncOperation = SceneManager.LoadSceneAsync("Game");
            _asyncOperation.allowSceneActivation = false;
            await UniTask.WaitForEndOfFrame();
        }

        _asyncOperation.allowSceneActivation = true;
        IsEnteringGame = true;

        // SoundManager.PlaySFX("sfx_buff");
        // Facade.FadeInOutFront.FadeOut(Color.black, null, 0.5f);
    }

    #endregion
}
