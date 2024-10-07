using System;
using System.Collections.Generic;
using System.Linq;
using Sample.SpecData.Generator;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class SpecDataManager : GameObjectSingleton<SpecDataManager>
{
    #region public

    public static bool Initialized => isInitialized;
    public Dictionary<string, int> optionData = new();
    public SpecData Data => _data;

    public bool Init()
    {
        SpecCall.InitializeSpecs();

        optionData.Clear();

        isInitialized = true;

        Debug.LogColor($"SpecDataManager.Init:: Initialized successfully.", Color.yellow);
        return true;
    }

    public void Clear()
    {
        optionData.Clear();
        SpecCall.Clear();
    }


    /// <summary>
    /// 스펙 로드
    /// </summary>
    /// <returns></returns>
    public void LoadSpecData()
    {
        var jsonData = SpecDataResourceLoader.LoadSpecData();
        _data = JsonUtility.FromJson<SpecData>(jsonData);

        SpecCall.InitializeSpecs();
    }

    #endregion

    #region private

    private static bool isInitialized = false;
    private static bool isSpecReloaded = false;
    private static bool isLocalizeReloaded = false;
    private Dictionary<string, SpecLocalize> _dicLocalize = new();
    private SpecData _data;

    private static async UniTask DestroySelf()
    {
        if (Instance == null) return;
        isInitialized = false;
        Destroy(Instance.gameObject);
        await UniTask.NextFrame();
    }

    #endregion

    #region lifecycle

    protected override void OnDestroy()
    {
        DestroySelf().Forget();
        base.OnDestroy();
    }

    #endregion

    [Serializable]
    [System.Reflection.Obfuscation(Exclude = false, ApplyToMembers = false)]
    public class TempLocalize
    {
        public List<SpecLocalize> Localization = new();
    }
}
