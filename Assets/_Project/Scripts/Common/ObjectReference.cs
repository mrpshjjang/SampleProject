using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 전역 오브젝트를 유니크하게 레퍼런싱할 수 있는 컴포넌트
/// </summary>
public class ObjectReference : MonoBehaviour
{
    public static List<ObjectReference> ListReference = new();

    #region public

    /// <summary>
    /// 오브젝트 참조
    /// </summary>
    /// <param name="type"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    // public static bool GetReference(eObjectRefType type, out ObjectReference result)
    // {
    //     result = ListReference.FirstOrDefault(x => x.type == type);
    //     return result != null;
    // }

    #endregion


    #region protected

    #endregion


    #region private

    // [SerializeField] private eObjectRefType type;
    //
    // private static void AddToList(ObjectReference obj)
    // {
    //     ListReference.AddUnique(obj);
    // }
    //
    // private static void RemoveFromList(ObjectReference obj)
    // {
    //     ListReference.TryRemove(obj);
    // }


    #endregion


    #region lifecycle

    // private void OnEnable()
    // {
    //     AddToList(this);
    // }
    //
    // private void OnDisable()
    // {
    //     RemoveFromList(this);
    // }
    //
    // private void OnDestroy()
    // {
    //     RemoveFromList(this);
    // }

    #endregion
}
