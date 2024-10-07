using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteChanger : MonoBehaviour
{
    #region public

    #endregion

    #region protected

    #endregion

    #region private

    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private List<Sprite> listSprite;

    private void RandomChange()
    {
        renderer.sprite = Utils.RandomPick(listSprite);
    }

    #endregion

    #region lifecycle

    private void OnEnable()
    {
        RandomChange();
    }

    #endregion
}
