using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRandomOn : MonoBehaviour
{
    #region public

    #endregion

    #region protected

    #endregion

    #region private

    [SerializeField] private List<GameObject> objects;

    #endregion

    #region lifecycle

    private void OnEnable()
    {
        var pick = Utils.RandomPick(objects);

        foreach (var obj in objects)
        {
            obj.SetActive(obj == pick);
        }
    }

    #endregion
}
