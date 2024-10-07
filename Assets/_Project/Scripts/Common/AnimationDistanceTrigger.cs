using System.Collections;
using UnityEngine;

public class AnimationDistanceTrigger : MonoBehaviour
{
    #region public

    #endregion


    #region protected

    #endregion


    #region private

    private bool _isTriggered = false;

    [SerializeField] private Animator animator;
    [SerializeField] private float distance;
    [SerializeField] private string closeTrigger;
    [SerializeField] private string farTrigger;
    [SerializeField] private bool isPlayOnlyOnce;

    private IEnumerator Check()
    {
        WaitForSeconds delay = new WaitForSeconds(0.3f);

        while (gameObject.activeInHierarchy)
        {
            yield return delay;

            bool isClose = false;

            //거리 계산
            var pos = transform.position;
            // foreach (var unit in UnitSpawner.ListAllUnit)
            // {
            //     var distSquared = (pos - unit.CachedTr.position).sqrMagnitude;
            //     var thresholdSquared = distance * distance;
            //     if (distSquared < thresholdSquared)
            //     {
            //         isClose = true;
            //         break;
            //     }
            // }

            if (isClose && !_isTriggered)
            {
                _isTriggered = true;
                if (!closeTrigger.Equals(string.Empty))
                    animator.SetTrigger(closeTrigger);
            }
            else if (!isClose && _isTriggered)
            {
                _isTriggered = false;
                if (!farTrigger.Equals(string.Empty))
                    animator.SetTrigger(farTrigger);

                if (isPlayOnlyOnce)
                    break;
            }
        }
    }

    #endregion


    #region lifecycle

    private void OnEnable()
    {
        _isTriggered = false;
        StartCoroutine(Check());
    }

    #endregion
}
