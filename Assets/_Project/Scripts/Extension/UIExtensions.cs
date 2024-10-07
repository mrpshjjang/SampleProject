using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIExtensions
{
    // Shared array used to receive result of RectTransform.GetWorldCorners
    private static Vector3[] corners = new Vector3[4];

    /// <summary>
    /// Transform the bounds of the current rect transform to the space of another transform.
    /// </summary>
    /// <param name="source">The rect to transform</param>
    /// <param name="target">The target space to transform to</param>
    /// <returns>The transformed bounds</returns>
    public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
    {
        // Based on code in ScrollRect's internal GetBounds and InternalGetBounds methods
        Bounds bounds = new();
        if (source != null)
        {
            source.GetWorldCorners(corners);

            Vector3 vMin = new(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vMax = new(float.MinValue, float.MinValue, float.MinValue);

            var matrix = target.worldToLocalMatrix;
            for (var j = 0; j < 4; j++)
            {
                var v = matrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
        }

        return bounds;
    }

    /// <summary>
    /// Normalize a distance to be used in verticalNormalizedPosition or horizontalNormalizedPosition.
    /// </summary>
    /// <param name="axis">Scroll axis, 0 = horizontal, 1 = vertical</param>
    /// <param name="distance">The distance in the scroll rect's view's coordiante space</param>
    /// <returns>The normalized scoll distance</returns>
    public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
    {
        // Based on code in ScrollRect's internal SetNormalizedPosition method
        var viewport = scrollRect.viewport;
        var viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
        Bounds viewBounds = new(viewRect.rect.center, viewRect.rect.size);

        var content = scrollRect.content;
        var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();

        var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
        return distance / hiddenLength;
    }

    /// <summary>
    /// Scroll the target element to the vertical center of the scroll rect's viewport.
    /// Assumes the target element is part of the scroll rect's contents.
    /// </summary>
    /// <param name="scrollRect">Scroll rect to scroll</param>
    /// <param name="target">Element of the scroll rect's content to center vertically</param>
    public static async UniTask ScrollToCeneter(this ScrollRect scrollRect, RectTransform target, float offsetPadding, bool isHorizontal, bool isTween, float startDelay, bool forceLayoutRebuild, UnityAction callback)
    {
        scrollRect.DOKill();
        // scrollRect.RefreshTaskToken();
        //
        // var frame = (int)(Application.targetFrameRate * startDelay);
        // for (var iter = 0; iter < frame; iter++)
        // {
        //     await UniTask.Yield(PlayerLoopTiming.Update, scrollRect.GetTaskToken());
        // }
        //await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: scrollRect.GetTaskToken(), ignoreTimeScale: true);

        if (forceLayoutRebuild)
        {
            Utils.ForceLayoutRebuild(scrollRect.GetComponent<RectTransform>());
        }

        // The scroll rect's view's space is used to calculate scroll position
        var view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

        // Calcualte the scroll offset in the view's space
        var viewRect = view.rect;
        var elementBounds = target.TransformBoundsTo(view);

        if (isHorizontal)
        {
            var offset = viewRect.center.x - elementBounds.center.x + offsetPadding;
            var scrollPos = scrollRect.horizontalNormalizedPosition - scrollRect.NormalizeScrollDistance(0, offset);

            if (isTween)
            {
                scrollRect.DOHorizontalNormalizedPos(Mathf.Clamp(scrollPos, 0f, 1f), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() => { callback?.Invoke(); });
            }
            else
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
                callback?.Invoke();
            }
        }
        else
        {
            var offset = viewRect.center.y - elementBounds.center.y + offsetPadding;
            var scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);

            if (isTween)
            {
                scrollRect.DOVerticalNormalizedPos(Mathf.Clamp(scrollPos, 0f, 1f), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() => { callback?.Invoke(); });
            }
            else
            {
                scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
                callback?.Invoke();
            }
        }
    }

    public static void SetSprite(this Image image, Sprite sprite)
    {
        if (image == null) return;
        image.sprite = sprite;
    }

    public static void SetSprite(this Image image, string spriteName)
    {
        if (image == null) return;
        if (AddressableManager.Instance == null) return;

        image.sprite = AddressableManager.Instance.GetSprite(spriteName);
    }

    public static void SetActive(this Component component, bool isOn)
    {
        if (component == null)
        {
            Debug.LogError("Component가 존재하지 않습니다.");
            return;
        }

        if (component.gameObject.activeSelf != isOn)
            component.gameObject.SetActive(isOn);
    }
}
