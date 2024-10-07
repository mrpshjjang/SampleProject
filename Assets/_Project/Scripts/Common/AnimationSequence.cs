using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSequence : MonoBehaviour
{
    #region public

    #endregion


    #region protected

    #endregion


    #region private

    private Animator _animator;

    [SerializeField] private List<SequenceInfo> listSequence;

    /// <summary>
    /// 애니메이션 시퀀스 재생
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlaySequence()
    {
        int max = listSequence.Count;
        int cur = 0;

        while (cur != max)
        {
            var seq = listSequence[cur];

            yield return new WaitForSeconds(seq.time);

            _animator.ResetAllAnimatorTriggers();
            _animator.SetTrigger(seq.triggerName);

            cur++;
        }
    }

    #endregion


    #region lifecycle

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(nameof(PlaySequence));
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

    [Serializable]
    public class SequenceInfo
    {
        public float time;
        public string triggerName;
    }
}
