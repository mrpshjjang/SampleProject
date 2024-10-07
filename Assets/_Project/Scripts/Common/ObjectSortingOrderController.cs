using System;
using System.Collections.Generic;
using SRF;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 2D 오브젝트 소팅 오더를 자동으로 맞춰주는 컴포넌트
/// </summary>
public class ObjectSortingOrderController : MonoBehaviour
{
    #region public

    #endregion

    #region protected

    #endregion

    #region private

    private const int UNIT_POS_ORDER = 100;
    private static long _offset = 0;
    private static List<ObjectSortingOrderController> _controllers = new();

    private bool _isOffsetAlerted = false;
    private long _updateCount = 0;
    private bool _isRendererInitialized = false;
    private List<MeshObjectRenderer> _listMeshRenderer = new();
    private List<SpriteObjectRenderer> _listSpriteRenderer = new();

    [SerializeField] private bool updateEveryFrame = false;

    private void InitRenderer()
    {
        if (_isRendererInitialized)
            return;

        _listMeshRenderer.Clear();
        _listSpriteRenderer.Clear();

        foreach (var temp in GetComponentsInChildren<MeshRenderer>(true))
        {
            _listMeshRenderer.Add(new MeshObjectRenderer(temp));
        }
        foreach (var temp in GetComponentsInChildren<SpriteRenderer>(true))
        {
            _listSpriteRenderer.Add(new SpriteObjectRenderer(temp));
        }

        _isRendererInitialized = true;
    }

    private void UpdateOffset()
    {
        long avg = 0;
        foreach (var controller in _controllers)
            avg += -(long) (controller.transform.position.y * UNIT_POS_ORDER);
        avg /= _controllers.Count;

        _offset = -avg;

        foreach (var controller in _controllers)
            controller.UpdateOrder(false);
    }

    private void UpdateOrder(bool autoUpdateOffset)
    {
        if(!_isRendererInitialized)
            InitRenderer();

        var order = (int)(_offset + GetOrder());
        bool isOutOrder = (order > 15000 || order < -15000);
        if (autoUpdateOffset && !_isOffsetAlerted && isOutOrder)
        {
            _isOffsetAlerted = true;
            UpdateOffset();
            return;
        }

        if (!isOutOrder)
        {
            _isOffsetAlerted = false;
        }

        if (_listSpriteRenderer.Count > 0)
        {
            foreach (var temp in _listSpriteRenderer)
                temp.SetOrder(order);
        }

        if (_listMeshRenderer.Count > 0)
        {
            foreach (var temp in _listMeshRenderer)
                temp.SetOrder(order);
        }
    }

    private int GetOrder()
    {
        return -(int) (transform.position.y * UNIT_POS_ORDER);
    }

    #endregion

    #region lifecycle

    private void Update()
    {
        if (updateEveryFrame && _updateCount % 10 == 0)
            UpdateOrder(true);

        _updateCount++;
    }

    private void OnEnable()
    {
        _controllers.Add(this);
        UpdateOrder(false);
    }

    private void OnDisable()
    {
        _controllers.Remove(this);
    }

    #endregion

    public class SpriteObjectRenderer
    {
        public SpriteRenderer Renderer => renderer;

        private SpriteRenderer renderer;
        private int originSortingOrder;

        public SpriteObjectRenderer(SpriteRenderer renderer)
        {
            this.renderer = renderer;
            originSortingOrder = renderer.sortingOrder;
        }

        public void SetOrder(int value)
        {
            renderer.sortingOrder = originSortingOrder + value;
        }

        public void SetSortingName(string str)
        {
            renderer.sortingLayerName = str;
        }
    }

    public class MeshObjectRenderer
    {
        public MeshRenderer Renderer => renderer;

        private MeshRenderer renderer;
        private int originSortingOrder;

        public MeshObjectRenderer(MeshRenderer renderer)
        {
            this.renderer = renderer;
            originSortingOrder = renderer.sortingOrder;
        }

        public void SetOrder(int value)
        {
            renderer.sortingOrder = originSortingOrder + value;
        }

        public void SetSortingName(string str)
        {
            renderer.sortingLayerName = str;
        }
    }
}
