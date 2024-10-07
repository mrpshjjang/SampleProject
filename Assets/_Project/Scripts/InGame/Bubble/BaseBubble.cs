using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class BaseBubble : MonoBehaviour, IAddressablePoolHandler
{
    #region public

    public SpecBubble Spec { get; protected set; }
    public SpecBubbleAttribute MainAttribute { get; protected set; }
    public SpecBubbleAttribute SubAttribute { get; protected set; }

    public bool ActionIng => _action;

    public virtual void Init(SpecBubble spec, int idxX, int idxY)
    {
        Spec = spec;
        _idxX = idxX;
        _idxY = idxY;

        if (spec.attribute != eBubbleAttribute.NONE)
        {
            MainAttribute = Facade.Spec.Data.SpecBubbleAttribute.Find(e => e.attribute == spec.attribute);
            imgMainAttribute.sprite = AddressableManager.Instance.GetSprite(MainAttribute.icon);

            var scale = (spec.attribute == eBubbleAttribute.BOSS_1 ? Common.BOSS_BUBBLE_DIAMETER : Common.BUBBLE_DIAMETER) / (float)imgMainAttribute.sprite.rect.width;
            gameObject.transform.localScale = new Vector3(scale, scale);
        }


        if (spec.sub_attribute != eBubbleAttribute.NONE)
        {
            int randNum = UnityEngine.Random.Range(0, 1000);
            if (randNum < 250)
            {
                SubAttribute = Facade.Spec.Data.SpecBubbleAttribute.Find(e => e.attribute == spec.sub_attribute);
                imgSubAttribute.sprite = AddressableManager.Instance.GetSprite(SubAttribute.icon);
            }
            else
            {
                SubAttribute = Facade.Spec.Data.SpecBubbleAttribute.Find(e => e.attribute == eBubbleAttribute.NONE);
                imgSubAttribute.sprite = null;
            }
        }

        MapBubbleComponent.Init();
        MapBubbleComponent.SetPlaceOption();

        ShootBubbleComponent.Init();
    }

    public virtual void Init(SpecBubble spec)
    {
        Spec = spec;

        if (spec.attribute != eBubbleAttribute.NONE)
        {
            MainAttribute = Facade.Spec.Data.SpecBubbleAttribute.Find(e => e.attribute == spec.attribute);
            imgMainAttribute.sprite = AddressableManager.Instance.GetSprite(MainAttribute.icon);

            var scale = (spec.attribute == eBubbleAttribute.BOSS_1 ? Common.BOSS_BUBBLE_DIAMETER : Common.BUBBLE_DIAMETER) / (float)imgMainAttribute.sprite.rect.width;
            gameObject.transform.localScale = new Vector3(scale, scale);
        }

        SubAttribute = Facade.Spec.Data.SpecBubbleAttribute.Find(e => e.attribute == eBubbleAttribute.NONE);
        imgSubAttribute.sprite = null;

        MapBubbleComponent.Init();
        MapBubbleComponent.SetPlaceOption();

        ShootBubbleComponent.Init();
    }

    public virtual void DestroyBubble(float delayTime = 0f, Action destroyComplete = null, Action destroyStart = null)
    {
        if (isActiveAndEnabled)
        {
            StartCoroutine(StartDestroyDelay(delayTime, () =>
            {
                if (destroyStart != null)
                    destroyStart();

                if (destroyComplete != null)
                    destroyComplete();

                RemoveBubble();
            }));
        }
        else
        {
            if (destroyStart != null)
                destroyStart();

            if (destroyComplete != null)
                destroyComplete();

            RemoveBubble();
        }
    }

    private void RemoveBubble()
    {
        if (ShootBubbleComponent != null)
            ShootBubbleComponent.Dispose();

        if (MapBubbleComponent != null)
            MapBubbleComponent.Dispose();

        BubbleManager.ReturnBubble(Spec, this);
    }

    public void FloatingBubble(MapBubble mapBubble)
    {
        DestroyBubble();
    }

    public void MoveBubble(Vector3 position)
    {
        if (_sequence == null)
        {
            _sequence = DOTween.Sequence();
        }
        _sequence.Kill();
        _sequence.Append(transform.DOLocalMove(position, 0.2f));
    }

    private IEnumerator StartDestroyDelay(float delay, Action delayComplete)
    {
        yield return new WaitForSeconds(delay);

        if (delayComplete != null)
            delayComplete();
    }

    #endregion

    #region protected

    protected int _idxX;
    protected int _idxY;
    protected Sequence _sequence;
    protected bool _action;

    [SerializeField] protected SpriteRenderer imgMainAttribute;
    [SerializeField] protected SpriteRenderer imgSubAttribute;

    [SerializeField] public MapBubble MapBubbleComponent;
    [SerializeField] public ShootBubble ShootBubbleComponent;

    #endregion

    #region private

    #endregion

    #region lifecycle

    #endregion

    #region PoolHandler

    public void ReturnToPool()
    {
    }

    public void DestroyToPool()
    {

    }

    public GameObject poolObject => gameObject;

    #endregion
}
