using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using DG.Tweening;
using PathologicalGames;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


/// <summary>
/// 일반적으로 사용하는 Extension 모음
/// </summary>
public static class CommonExtension
{
    /// <summary>
    /// 값을 등수형 넘버 스트링으로 반환
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToOrdinalNumbers(this int source)
    {
        switch (source)
        {
            case 0: return "0";
            case 1: return $"1st";
            case 2: return $"2nd";
            case 3: return $"3rd";
            default: return $"{source}th";
        }
    }

    /// <summary>
    /// 랜덤 셔플
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(new System.Random());
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (rng == null)
        {
            throw new ArgumentNullException(nameof(rng));
        }

        return source.ShuffleIterator(rng);
    }

    private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, System.Random rng)
    {
        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            var j = rng.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    /// <summary>
    /// 모든 애니메이션 트리거 리셋
    /// </summary>
    /// <param name="animator"></param>
    public static void ResetAllAnimatorTriggers(this Animator animator)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

    /// <summary>
    /// 더블 타입 값 트리밍(지정해놓은 맥스값을 넘지 못하게 함)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double Trim(this double value)
    {
        if (double.IsPositiveInfinity(value) || value > Facade.DOUBLE_MAX)
        {
            return Facade.DOUBLE_MAX;
        }

        return value;
    }

    /// <summary>
    /// 리스트 전용 함수
    /// </summary>
    public static bool AddUnique<T>(this List<T> list, T element)
    {
        if (list.Contains(element))
            return false;

        list.Add(element);
        return true;
    }

    public static bool TryRemove<T>(this List<T> list, T element)
    {
        if (!list.Contains(element))
            return false;

        list.Remove(element);
        return true;
    }

    public static bool RemoveRange<T>(this List<T> list, List<T> listElement)
    {
        foreach (var temp in listElement)
        {
            if (!list.Contains(temp))
                continue;

            list.Remove(temp);
        }

        return true;
    }

    public static T GetPrev<T>(this List<T> list, T element)
    {
        if (list == null) return default;

        var idx = list.IndexOf(element);

        if (idx - 1 < 0)
            return default;

        return list[idx - 1];
    }

    public static T GetNext<T>(this List<T> list, T element)
    {
        if (list == null) return default;

        var idx = list.IndexOf(element);

        if (idx + 1 >= list.Count)
            return default;

        return list[idx + 1];
    }

    /// <summary>
    /// 트랜스폼의 이동
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static bool CustomMove(this Transform tr, Vector3 direction)
    {
        var checkCenter = tr.position + Vector3.up;
        var timeDirection = Time.deltaTime * direction;

        var layerMask = 1 << LayerMask.NameToLayer("Wall");

        var isIgnorColl = tr.GetComponent<Collider2D>() == null || !tr.GetComponent<Collider2D>().enabled;

        if (isIgnorColl || !Utils.IsRayBlocked(checkCenter, timeDirection, layerMask))
        {
            tr.transform.position += timeDirection;
            return true;
        }
        else
        {
            Vector3 vecX = new(timeDirection.x, 0, 0);
            Vector3 vecZ = new(0, timeDirection.z, 0);

            if (!Utils.IsRayBlocked(checkCenter, vecX, layerMask))
            {
                tr.transform.position += vecX;
            }
            else if (!Utils.IsRayBlocked(checkCenter, vecZ, layerMask))
            {
                tr.transform.position += vecZ;
            }

            return false;
        }
    }

    /// <summary>
    /// 스파인 초기화
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="index"></param>
    public static void SetInitSkin(this SkeletonAnimation spine, string index)
    {
        var skeletonData = spine.skeleton.Data;

        Skin newSkin = new("");

        for (var i = 1; i <= 3; ++i)
        {
            var name = $"0{i}/0{i}-{index}";
            newSkin.AddSkin(skeletonData.FindSkin(name));
        }

        spine.skeleton.SetSkin(newSkin);
        spine.skeleton.SetSlotsToSetupPose();
    }

    public static void SetInitSkin(this SkeletonGraphic spine, string index)
    {
        var skeletonData = spine.SkeletonData;

        Skin newSkin = new("");

        for (var i = 1; i <= 3; ++i)
        {
            var name = $"0{i}/0{i}-{index}";
            newSkin.AddSkin(skeletonData.FindSkin(name));
        }

        spine.Skeleton.SetSkin(newSkin);
        spine.Skeleton.SetSlotsToSetupPose();
        spine.LateUpdate();
        spine.gameObject.SetActive(false);
        spine.gameObject.SetActive(true);
    }

    /// <summary>
    /// 특정 컴포넌트가 있는 가장 가까운 부모를 찾는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindClosestParentWithComponent<T>(this Transform tr) where T : MonoBehaviour
    {
        var first = tr.GetComponent<T>();
        if (first != null)
            return first;

        Transform currentTransform = tr.parent;

        while (currentTransform != null)
        {
            T component = currentTransform.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            currentTransform = currentTransform.parent;
        }

        return null;
    }

    public static Transform FindChildByRecursive(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindChildByRecursive(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// DoTween 바운스
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="value"></param>
    public static void TweenBounceObject(this Transform tr, float value = 1.25f, float duration = 0.15f)
    {
        var tweenId = "BounceObject" + tr.GetInstanceID();
        DOTween.Kill(tweenId);

        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.SetId(tweenId);
        seq.Append(tr.transform.DOScale(value, duration).SetUpdate(true));
        seq.Append(tr.transform.DOScale(1.0f, duration).SetUpdate(true));
    }

    /// <summary>
    /// 범용 캐스팅
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Cast<T>(this object obj) where T : class
    {
        return (obj as T);
    }

    /// <summary>
    /// Second -> DateTime 변환
    /// </summary>
    /// <param name="unixSeconds"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this long unixSeconds)
    {
        DateTime dateTime = TimeManager.Jan1st1970;
        return dateTime.AddSeconds(unixSeconds);
    }

    /// <summary>
    /// DateTime -> 날짜 카운트 변환
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long ToDays(this DateTime dateTime)
    {
        var timeSpan = new TimeSpan((dateTime - TimeManager.Jan1st1970).Ticks);
        return (long)timeSpan.TotalDays;
    }

    /// <summary>
    /// 초 혹은 TimeSpan을 남은 시간 디스플레이용 시간으로 바꿔줌
    /// </summary>
    /// <param name="unixSeconds"></param>
    /// <returns></returns>
    public static string ToLeftTimeString(this long unixSeconds, bool isAlwaysShowSecond)
    {
        return TimeSpan.FromSeconds(unixSeconds).ToLeftTimeString(isAlwaysShowSecond);
    }

    public static string ToLeftTimeString(this TimeSpan leftTimeSpan, bool isAlwaysShowSecond)
    {
        var result = string.Empty;

        if (isAlwaysShowSecond)
        {
            if (leftTimeSpan.TotalHours > 1)
            {
                result = ZString.Format("{0:0}H {1:0}M {2:0}S", (long)leftTimeSpan.TotalHours, leftTimeSpan.Minutes, leftTimeSpan.Seconds);
            }
            else if (leftTimeSpan.Minutes == 0)
            {
                result = ZString.Format("{0:0}S", leftTimeSpan.Seconds);
            }
            else
            {
                result = ZString.Format("{0:0}M {1:0}S", leftTimeSpan.Minutes, leftTimeSpan.Seconds);
            }
        }
        else
        {
            if (leftTimeSpan.Days > 0)
            {
                result = ZString.Format("{0:0}D {1:0}H", leftTimeSpan.Days, leftTimeSpan.Hours);
            }
            else if (leftTimeSpan.Hours > 0)
            {
                result = ZString.Format("{0:0}H {1:0}M", leftTimeSpan.Hours, leftTimeSpan.Minutes);
            }
            else if (leftTimeSpan.Minutes == 0)
            {
                result = ZString.Format("{0:0}S", leftTimeSpan.Seconds);
            }
            else
            {
                result = ZString.Format("{0:0}M {1:0}S", leftTimeSpan.Minutes, leftTimeSpan.Seconds);
            }
        }

        return result;
    }

    public static string ToLeftTimeStringHHMMSS(this long unixSeconds)
    {
        return TimeSpan.FromSeconds(unixSeconds).ToLeftTimeStringHHMMSS();

    }

    public static string ToLeftTimeStringHHMMSS(this TimeSpan leftTimeSpan)
    {
        //return $"{leftTimeSpan.ToString(@"hh\:mm\:ss")}";

        return $"{(int) leftTimeSpan.TotalHours:00}:{leftTimeSpan.Minutes:00}:{leftTimeSpan.Seconds:00}";
    }

    /// <summary>
    /// 스트링에 컬러코드를 붙이는 함수
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="colorString"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    public static string InsertColor(this string inputString, Color color)
    {
        // int indexStart = inputString.IndexOf("<<", StringComparison.InvariantCulture);
        // int indexEnd = inputString.IndexOf(">>", StringComparison.InvariantCulture);
        // if (indexStart == -1 || indexEnd == -1)
        //     return inputString;
        //
        // string colorFront = $"<color={Utils.ToRGBHex(color)}>";
        // string colorEnd = "</color>";
        // return inputString.Insert(indexStart, colorFront).Insert(indexEnd, colorEnd);

        return inputString.Replace("<<", $"<color={Utils.ToRGBHex(color)}>").Replace(">>", "</color>");
    }

    /// <summary>
    /// Repeated Field의 Sort 함수
    /// </summary>
    /// <param name="field"></param>
    /// <param name="comparison"></param>
    /// <typeparam name="T"></typeparam>
    // public static void Sort<T>(this RepeatedField<T> field, Comparison<T> comparison)
    // {
    //     var items = field.ToList();
    //     items.Sort(comparison);
    //     field.Clear();
    //     field.AddRange(items);
    // }

    /// <summary>
    /// 하위 레이아웃 강제 리빌드
    /// </summary>
    /// <param name="rt"></param>
    public static void ForceLayoutRebuildChild(this RectTransform rt)
    {
        if (rt == null || !rt.gameObject.activeSelf || rt.GetComponent<RectTransform>() == null)
        {
            return;
        }

        foreach (Transform child in rt)
        {
            if(child is RectTransform rectChild)
                ForceLayoutRebuildChild(rectChild);
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
    /// Z축 값 트림
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 TrimZ(this Vector3 pos)
    {
        pos.z = 0;
        return pos;
    }

    /// <summary>
    /// 앱이벤트용 콤마스트링 생성 함수
    /// </summary>
    /// <param name="inputs"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string MakeListString<T>(this List<T> inputs, string prefix)
    {
        StringBuilder sBuilder = new StringBuilder();

        string result = string.Empty;

        for (int i = 0; i < inputs.Count; i++)
        {
            sBuilder.Append(inputs[i].ToString());

            if(i != inputs.Count - 1)
                sBuilder.Append(prefix);
        }

        return sBuilder.ToString();
    }

    /// <summary>
    /// 모든 타일맵 포지션 가져오기
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns></returns>
    public static List<Vector3> GetAllTileWorldPositions(this Tilemap tilemap)
    {
        List<Vector3> tilePositions = new List<Vector3>();
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(localPlace))
                {
                    tilePositions.Add(tilemap.CellToWorld(localPlace));
                }
            }
        }

        return tilePositions;
    }

    /// <summary>
    /// 타일 그리드의 한 칸의 크기 구하기
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns></returns>
    public static Vector2 GetTileSizeInWorldCoords(this Tilemap tilemap)
    {
        Grid grid = tilemap.layoutGrid;
        if (grid != null)
        {
            Vector2 tileSize = grid.cellSize;
            return tileSize;
        }
        else
        {
            return Vector2.one;
        }
    }

    public static T AddComponentUnique<T>(this GameObject go) where T : MonoBehaviour
    {
        if (go.TryGetComponent<T>(out var temp))
            return temp;

        return go.AddComponent<T>();
    }

    /// <summary>
    /// 스트링의 숫자만 추출
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ExtractNumbers(this string input)
    {
        // 정규 표현식을 사용하여 숫자만 추출
        return Regex.Replace(input, "[^0-9]", "");
    }
}
