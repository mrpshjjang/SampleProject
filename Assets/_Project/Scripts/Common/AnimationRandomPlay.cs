using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRandomPlay : MonoBehaviour
{
    #region public

    #endregion


    #region protected

    #endregion


    #region private

    private Animator _animator;

    [SerializeField] private float prob;
    [SerializeField] private float interval;
    [SerializeField] private string triggerName;

    /// <summary>
    /// 애니메이션 체크
    /// </summary>
    /// <returns></returns>
    private IEnumerator Check()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(interval);

            if (UnityEngine.Random.Range(0f, 1f) < prob)
            {
                _animator.ResetAllAnimatorTriggers();
                _animator.SetTrigger(triggerName);
            }
        }

    }

    #endregion


    #region lifecycle

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(nameof(Check));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #endregion
}
