using UnityEngine;

/// <summary>
/// 에디터 뷰포트에서 RectTransform에 컬러박스를 쳐주는 클래스
/// </summary>
public abstract class RectMonoBehaviour : MonoBehaviour
{
    public abstract Color validColor { get; }
    public abstract Color invalidColor { get; }
    public abstract int LineCount { get; }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var rt = GetComponent<RectTransform>();
        var rect = rt.rect;
        rect.x *= rt.lossyScale.x;
        rect.y *= rt.lossyScale.y;
        rect.x += rt.position.x;
        rect.y += rt.position.y;
        rect.width *= rt.lossyScale.x;
        rect.height *= rt.lossyScale.y;

        for (int iter = 0; iter < LineCount; iter++)
        {
            var temp = rect.height * (0.1f * iter);

            Gizmos.color = CheckRectCondition() ? validColor : invalidColor;
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(rect.center, Quaternion.identity, new Vector3(rect.width - temp, rect.height - temp, 4f));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = oldMatrix;
        }
    }
#endif

    protected virtual bool CheckRectCondition()
    {
        return true;
    }
}
