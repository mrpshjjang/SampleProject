using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GuideLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    //[SerializeField] private Transform _startParticle;

    public void Init()
    {
        ResetGuideLine();

        _lineRenderer.SetPosition(0, IngameViewManager.instance.BubbleShooter.ShootReadyBubblePosition());
    }

    public void Dispose()
    {

    }

    public void SetGuideLine()
    {
        Material lineMaterial = null;

        lineMaterial = Resources.Load("Material/Ingame/G_Lani_W") as Material;

        _lineRenderer.material = lineMaterial;

        //_startParticle.gameObject.SetActive(true);
    }

    public void ResetGuideLine()
    {
        _lineRenderer.positionCount = 1;

        //_startParticle.gameObject.SetActive(false);
    }

    public int GetGuideLinePositionCount()
    {
        return _lineRenderer.positionCount;
    }

    public void AddGuideLinePosition(int collisionCount, Vector3 position, bool isTarget = false)
    {
        if (isTarget)
        {
            _lineRenderer.positionCount = collisionCount + 1;

            // if (_startParticle.gameObject.activeSelf)
            //     _startParticle.localEulerAngles = new Vector3(0f, 0f, ShootControlManager.instance.ShootAngleValue);
        }
        else
        {
            if (_lineRenderer.positionCount < collisionCount + 1)
                _lineRenderer.positionCount = collisionCount + 1;
        }

        _lineRenderer.SetPosition(collisionCount, position);

        if (Vector3.Distance(_lineRenderer.GetPosition(collisionCount - 1), _lineRenderer.GetPosition(collisionCount)) <= Common.BUBBLE_DIAMETER * 1.5f)
            _lineRenderer.positionCount--;
    }
}
