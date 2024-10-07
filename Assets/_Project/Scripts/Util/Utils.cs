using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

//using Newtonsoft.Json.Linq;

public static class Utils
{
    /// <summary>
    /// 스톱워치 타이머
    /// </summary>
    private static Stopwatch _stopWatch = new();
    public static void StartTimer()
    {
        _stopWatch.Reset();
        _stopWatch.Start();
    }
    public static float StopTimer()
    {
        _stopWatch.Stop();
        return _stopWatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// 클래스 타입의 부모 타입들을 모두 반환하는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetParentTypes(this Type type)
    {
        if (type == null)
            yield break;

        foreach (var i in type.GetInterfaces())
            yield return i;

        var currentBaseType = type.BaseType;
        while (currentBaseType != null && currentBaseType != typeof(MonoBehaviour))
        {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
    }

    /// <summary>
    /// 두 벡터의 앵글
    /// </summary>
    /// <param name="fromPos"></param>
    /// <param name="toPos"></param>
    /// <returns></returns>
    public static float GetAngleFromTwoPositionXY(Vector3 fromPos, Vector3 toPos)
    {
        var xDistance = toPos.x - fromPos.x;
        var yDistance = toPos.y - fromPos.y;

        var angle = Mathf.Atan2(yDistance, xDistance) * Mathf.Rad2Deg;
        while (angle < 0f)
        {
            angle += 360f;
        }

        while (360f < angle)
        {
            angle -= 360f;
        }

        return angle;
    }

    public static float GetTwoVectorPositiveAngle(Vector3 from, Vector3 to)
    {
        var dot = Vector3.Dot(from, to);

        if (dot > 1.000f)
        {
            dot = 0.9999f;
        }

        if (dot < -1.000f)
        {
            dot = 0.0001f;
        }

        var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle;
    }

    /// <summary>
    /// 네 점의 교차지점
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <returns></returns>
    public static Vector2 GetIntersectionPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        double d = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
        if (d == 0)
        {
            return Vector2.zero;
        }

        double pre = p1.x * p2.y - p1.y * p2.x, post = p3.x * p4.y - p3.y * p4.x;
        var x = (pre * (p3.x - p4.x) - (p1.x - p2.x) * post) / d;
        var y = (pre * (p3.y - p4.y) - (p1.y - p2.y) * post) / d;

        return new Vector2((float)x, (float)y);
    }

    /// <summary>
    /// 벡터 방향
    /// </summary>
    /// <param name="start"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static Vector3 GetDirection(Vector3 start, Vector3 to)
    {
        return (to - start).normalized;
    }

    /// <summary>
    /// 2D 회전
    /// </summary>
    /// <param name="v"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static Vector2 Rotate2D(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    /// <summary>
    /// 코사인 유사도 계산
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float CalculateCosineSimilarity(Vector3 a, Vector3 b)
    {
        var dotProduct = Vector3.Dot(a, b);
        var magnitudeA = a.magnitude;
        var magnitudeB = b.magnitude;

        // 코사인 유사도 계산
        var cosineSimilarity = dotProduct / (magnitudeA * magnitudeB);

        return cosineSimilarity;
    }

    /// <summary>
    /// 레이캐스트의 히트 여부
    /// </summary>
    /// <param name="center"></param>
    /// <param name="direction"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static bool IsRayBlocked(Vector3 center, Vector3 direction, int layerMask)
    {
        RaycastHit hit;
        Ray2D ray = new(center, direction);

        return Physics2D.Raycast(center, direction, direction.magnitude, layerMask);
    }

    /// <summary>
    /// 알파벳 텍스트
    /// </summary>
    private static char[] alphabet = new char[] { };
    public static string GetAlphabetText(double num)
    {
        if (num < 0)
        {
            return "";
        }

        if (num < 1000)
        {
            return num.ToString("0");
        }

        if (alphabet.Length == 0)
        {
            alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        }

        var count = (int)Math.Floor(Math.Log(num, 1000)) - 1;

        var result = "";
        if (count == 0)
        {
            result = alphabet[0] + result;
        }
        else
        {
            while (count > 0)
            {
                var index = count % 26;
                result = result != "" ? alphabet[index - 1] + result : alphabet[index] + result;
                count /= 26;
            }
        }

        var value = num / Math.Pow(1000, Math.Floor(Math.Log(num, 1000)));
        return $"{value:N2}{result}";
    }

    /// <summary>
    /// 천 자리 콤마
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetThousandCommaText(double data)
    {
        if (data == 0)
        {
            return "0";
        }
        else
        {
            return string.Format("{0:#,###}", data);
        }
    }

    /// <summary>
    /// 랜덤 픽
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T RandomPick<T>(List<T> list)
    {
        var pick = UnityEngine.Random.Range(0, list.Count);
        return list[pick];
    }

    public static T RandomPick<T>(List<T> list, List<int> probs, int totalProb = -1)
    {
        if (list.Count != probs.Count)
        {
            UnityEngine.Debug.LogError("RandomPick:: Invalid lists");
            return list.First();
        }

        int randNum = UnityEngine.Random.Range(0, totalProb == -1 ? probs.Sum() : totalProb);
        int probSum = 0;

        foreach (var box in list)
        {
            var curProb = probs[list.IndexOf(box)];
            if (randNum >= probSum && randNum < probSum + curProb)
            {
                return box;
            }

            probSum += curProb;
        }

        return default;
    }

    /// <summary>
    /// 방어력 산정식
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public static double CalculateDmgAndDef(double damage, double def)
    {
        //방어상수 = 공격자 최종 공격력 * 1.2
        //방어율 = 방어력 / (방어력 + 방어상수)
        //최종 데미지 = 공격자 최종 공력력 * (1 - 방어율)

        double defAdjuster = damage * 1.2;
        double defRate = def / (def + defAdjuster);

        return Math.Floor(damage * (1 - defRate));
    }

    // /// <summary>
    // /// 엑셀 데이터 가져오기
    // /// </summary>
    // /// <param name="filePath"></param>
    // /// <returns></returns>
    // public static DataSet RoadExcelData(string filePath)
    // {
    //     using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
    //     {
    //         using (var reader = ExcelReaderFactory.CreateReader(stream))
    //         {
    //             var result = reader.AsDataSet();
    //
    //             return result;
    //         }
    //     }
    // }

    // /// <summary>
    // /// UI 셀 생성
    // /// </summary>
    // /// <param name="listCell"></param>
    // /// <param name="amount"></param>
    // /// <param name="cellRoot"></param>
    // /// <param name="uiType"></param>
    // /// <typeparam name="T"></typeparam>
    // public static void SetCellListSize<T>(ref List<T> listCell, int amount, Transform cellRoot, ePoolUi uiType, bool isForceRebuildLayout = false) where T : MonoBehaviour
    // {
    //     bool hasDiff = false;
    //
    //     //remove
    //     if (listCell.Count > amount)
    //     {
    //         for (var idx = listCell.Count - 1; idx >= amount; idx--)
    //         {
    //             var cell = listCell[idx];
    //             //PoolManager.Pools[ePoolType.UI.ToString()].Despawn(cell.transform, PoolManager.Pools[ePoolType.UI.ToString()].transform);
    //             ObjectPoolManager.Instance.ReturnPoolObject(eObjPoolType.UI, cell.name, cell.GetComponent<ObjectPoolBase>());
    //
    //             listCell.Remove(cell);
    //             hasDiff = true;
    //         }
    //     }
    //
    //     //create
    //     if (listCell.Count < amount)
    //     {
    //         var needCreate = amount - listCell.Count;
    //         for (var idx = 0; idx < needCreate; idx++)
    //         {
    //             //var trCell = PoolManager.Pools[ePoolType.UI.ToString()].Spawn(uiType.ToString());
    //             ObjectPoolManager.Instance.LoadObjectPool(eObjPoolType.UI, uiType.ToString());
    //             var cell = ObjectPoolManager.Instance.GetPoolObject<ObjectPoolBase>(eObjPoolType.UI, uiType.ToString());
    //             cell.gameObject.SetActive(true);
    //             var trCell = cell.transform;
    //
    //             trCell.SetParent(cellRoot);
    //             trCell.localPosition = Vector3.zero;
    //             trCell.localScale = Vector3.one;
    //
    //             listCell.Add(trCell.GetComponent<T>());
    //             hasDiff = true;
    //         }
    //     }
    //
    //     if (isForceRebuildLayout && hasDiff)
    //     {
    //         cellRoot.GetComponent<RectTransform>().ForceLayoutRebuildChild();
    //     }
    // }

    /// <summary>
    /// 컬러 변환
    /// </summary>
    public static Color GetColor(string hex)
    {
        Color color = new();
        ColorUtility.TryParseHtmlString($"#{hex}", out color);
        return color;
    }

    public static string ToRGBHex(Color c)
    {
        return ZString.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    /// <summary>
    /// 게임 나가기
    /// </summary>
    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 메모리 클린업
    /// </summary>
    public static async void CleanUpMemory()
    {
        await Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public static async void GCIncremental()
    {
        var completeCount = 0;

        while (completeCount < 5)
        {
            if (GarbageCollector.CollectIncremental(3000000))
            {
                completeCount++;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    /// <summary>
    /// 원형 랜덤 포지션
    /// </summary>
    /// <param name="inRadius"></param>
    /// <param name="outRadius"></param>
    /// <returns></returns>
    public static Vector3 RandomSpherePos(float inRadius, float outRadius)
    {
        var point = UnityEngine.Random.onUnitSphere;
        point.z = 0f;

        var r = UnityEngine.Random.Range(inRadius, outRadius);
        return point * r;
    }

    /// <summary>
    /// 게임 버전 코드
    /// </summary>
    /// <returns></returns>
    public static int GetGameVersion()
    {
        return int.Parse(Application.version.Replace(".", ""));
    }

    /// <summary>
    /// 단계적 레이아웃 리빌드
    /// </summary>
    /// <param name="rt"></param>
    public static void ForceLayoutRebuild(RectTransform rt)
    {
        if (rt == null || !rt.gameObject.activeSelf || rt.GetComponent<RectTransform>() == null)
        {
            return;
        }

        foreach (RectTransform child in rt)
        {
            ForceLayoutRebuild(child);
        }

        var layoutGroup = rt.GetComponent<LayoutGroup>();
        var contentSizeFitter = rt.GetComponent<ContentSizeFitter>();
        if (layoutGroup != null)
        {
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        if (contentSizeFitter != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }

    /// <summary>
    /// GUID 생성
    /// </summary>
    /// <returns></returns>
    public static string GenerateGUID()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// String을 DateTime으로 변환
    /// </summary>
    /// <param name="dateString"></param>
    /// <returns></returns>
    public static DateTime ConvertStringToDateTime(string dateString)
    {
        try
        {
            DateTime result = DateTime.Parse(dateString, CultureInfo.InvariantCulture);
            return result;
        }
        catch (FormatException)
        {
            UnityEngine.Debug.LogError("Invalid format.");
            return DateTime.MinValue;
        }
    }

    //───────────────────────────────────────────────────────────────────────────────────────
    // 메카님
    //───────────────────────────────────────────────────────────────────────────────────────
    /// <summary>
    /// 메카님 애니메이션 플레이
    /// </summary>
    /// <param name="animator_"></param>
    /// <param name="strAnimName_"></param>
    public static void PlayMecanim(Animator animator_, string strAnimName_)
    {
        if(animator_.gameObject.activeInHierarchy == false)
        {
            return;
        }

        animator_.enabled = true;
        animator_.Play(strAnimName_, -1, 0);
    }

    public static void StopMecanim(Animator animator_)
    {
        animator_.enabled = false;
    }

    private static IEnumerator PlayMecanimCoroutine(Animator animator_, string strAnimName_)
    {
        PlayMecanim(animator_, strAnimName_);

        yield return null;

        while (true)
        {
            if(animator_ == null || animator_.gameObject.activeInHierarchy == false)     //게임 오브젝트가 꺼져있으면 연출 멈춤
            {
                break;
            }
            AnimatorStateInfo info = animator_.GetCurrentAnimatorStateInfo(0);

            if (/*info.IsName(strAnimName_) && */info.normalizedTime <= 1.0f)
            {
                yield return null;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 메카님 애니메이션 플레이
    /// </summary>
    /// <param name="animator_"></param>
    /// <param name="strAnimName_"></param>
    /// <param name="delegateEndAnimation_"></param>
    /// <returns></returns>
    // public static Coroutine PlayMecanim(Animator animator_, string strAnimName_, Action onComplete_)
    // {
    //     return AnimationManager.Instance.StartCoroutine(PlayMecanimCoroutine(animator_, strAnimName_, onComplete_));
    // }

    private static IEnumerator PlayMecanimCoroutine(Animator animator_, string strAnimName_, Action onComplete_)
    {
        yield return PlayMecanimCoroutine(animator_, strAnimName_);

        if (onComplete_ != null)
        {
            onComplete_();
            onComplete_ = null;
        }
    }

    /// <summary>
    /// 닉네임 관련
    /// </summary>
    public static string DisplayNickName(string nickname, int uid)
    {
        if (IsDefaultNickName(nickname, uid))
        {
            return $"GUEST_{uid}";
        }

        return nickname;
    }

    public static bool IsDefaultNickName(string nickname, int uid)
    {
        return string.IsNullOrEmpty(nickname) || nickname.Equals(uid.ToString());
    }

    internal static DateTime ToLocalDate(this int unixTime)
    {
        return ToLocalDate((long) unixTime);
    }

    /// <summary>
    /// 해당 UnixTime을 LocalDateTime으로 변환
    /// </summary>
    /// <param name="unixTime"></param>
    /// <returns></returns>
    internal static DateTime ToLocalDate(this long unixTime)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        return dateTimeOffset.LocalDateTime;
    }
}
