using System;
using System.Collections.Generic;
using System.Linq;
using Sigtrap.Relays;
using Unity.VisualScripting;
using UnityEngine;

public class PopupManager : GameObjectSingleton<PopupManager>
{
    #region public

    public static readonly Relay<ePopupType> HidePopupEvent = new();
    public static readonly Relay CompleteLayer = new();

    public static Transform Parent => Inst.trAnchor;
    public static int LayerCount => Inst._listPopupLayer.Count;
    public static int StackCount => Inst._listStackPopup.Count(x => !x.ignoreStackCount); // 레이어가 가변적으로 변하는 팝업 스택 카운트
    public static int AllStackCount => Inst._listStackPopup.Count; // 모든 팝업 스택 카운트

    public static PopupBase TopEscapablePopup
    {
        get
        {
            var listStack = Inst._listStackPopup.Where(x => x.Popup.CloseByEscapeType is PopupBase.ePopupEscapeType.CAN_ESCAPE or PopupBase.ePopupEscapeType.BLOCK_ESCAPE);
            if (!listStack.Any())
            {
                return null;
            }

            return listStack.Last().Popup;
        }
    }

#if UNITY_EDITOR
    protected override bool Valid => base.Valid && Application.isPlaying;
#endif

    public static PopupBase Show(ePopupType type, bool isAddLayer = false)
    {
        if (type == ePopupType.None)
            return null;

        if (isAddLayer)
        {
            return Inst.AddFirstLayer(type);
        }
        else
        {
            return Inst.ShowPopup(type);
        }
    }

    public static PopupBase Show(ePopupType type, Transform trParent)
    {
        if (type == ePopupType.None)
            return null;

        return Inst.ShowPopup(type, trParent);
    }

    public static void HidePopupData(PopupData data)
    {
        if (data.Type == ePopupType.None)
        {
            return;
        }

        if (Inst.HidePopup(data.Type))
        {
            HidePopupEvent?.Dispatch(data.Type);
        }
    }

    public static void Hide(ePopupType type)
    {
        if (type == ePopupType.None)
        {
            return;
        }

        var data = Inst.GetPopupData(type);

        if (data != null)
        {
            data.Popup.Hide();
        }
    }

    public static void RemoveStack(ePopupType type)
    {
        if (type == ePopupType.None)
        {
            return;
        }

        if (Inst == null)
        {
            return;
        }

        var data = inst.GetStackPopupData(type);

        if (data != null)
        {
            Inst.RemoveStackData(data);
        }
    }

    public static T GetPopup<T>() where T : Component => Inst.GetPopupData<T>();

    public static PopupBase CheckAndGetPopUp(ePopupType type)
    {
        var data = Inst.GetPopupData(type);

        if (data == null)
        {
            return inst.CreatePopup(type, false).Popup;
        }

        return data.Popup;
    }

    public static bool IsOpened(ePopupType type)
    {
        var stackData = inst.GetStackPopupData(type);

        if (stackData != null)
        {
            return true;
        }

        return false;
    }

    public static void DestroyPopup(PopupBase popup)
    {
        var data = Inst.GetPopupData(popup);

        if (data != null)
        {
            Inst._listStackPopup.Remove(data);
            Inst._listActivePopup.Remove(data);
            Destroy(popup.gameObject);
        }
    }

    public static void InitLayer()
    {
        foreach (var type in Inst._listPopupLayer)
        {
            Hide(type);
        }

        Inst._listPopupLayer.Clear();
    }

    public static void AddLayer(ePopupType type)
    {
        Inst._listPopupLayer.Add(type);
    }

    public static void InsertLayer(ePopupType type)
    {
        Inst._listPopupLayer.Insert(0, type);
    }

    public static bool ShowLayer()
    {
        if (Inst._listPopupLayer.Count == 0)
        {
            CompleteLayer?.Dispatch();
            return false;
        }

        Show(Inst._listPopupLayer[0]);
        // Inst._listPopupLayer.RemoveAt(0);

        return true;
    }

    public static void RemoveLayer(ePopupType type)
    {
        Inst._listPopupLayer.Remove(type);
    }

    public static void RemoveAllLayer()
    {
        Inst._listPopupLayer.Clear();
    }

    public static void CheckLayer()
    {
        // if(inst.listPopupLayer.Count > 0) ShowLayer();
        ShowLayer();
    }

    public static bool HasLayer(ePopupType type)
    {
        return Inst._listPopupLayer.Contains(type);
    }

    public static List<PopupData> GetStackedPopupAll()
    {
        return Instance._listStackPopup;
    }

    public static void HideAll()
    {
        foreach (var popupData in PopupManager.GetStackedPopupAll().ToList())
        {
            popupData.Popup.Hide();
        }

        foreach (var popupData in PopupManager.GetStackedPopupAll().ToList())
        {
            DestroyPopup(popupData.Popup);
        }
    }

    #endregion

    #region protected

    #endregion

    #region private

    [SerializeField]
    private Transform trAnchor;

    [SerializeField]
    private List<PopupData> listPopupData = new();

    private readonly List<PopupData> _listActivePopup = new();
    private readonly List<PopupData> _listStackPopup = new();
    private readonly List<ePopupType> _listPopupLayer = new();

    private PopupBase ShowPopup(ePopupType type, Transform trParent = null)
    {
        PopupData data;

        //팝업이 생성 안되어 있으니 새로 생성
        if (GetPopupData(type) == null)
        {
            data = CreatePopup(type, true, trParent);

            if (data != null)
                AddStackData(data);

            return data.Popup;
        }
        else
        {
            data = GetPopupData(type);
            var popupActive = data.Popup;
            if (trParent != null)
            {
                var trPopup = popupActive.transform;
                trPopup.localPosition = Vector2.zero;
                trPopup.localScale = Vector3.one;
            }

            popupActive.Show();

            if (data != null)
                AddStackData(data);

            return popupActive;
        }

    }

    private bool HidePopup(ePopupType type)
    {
        var data = GetPopupData(type);

        if (data != null)
        {
            data.Popup.transform.SetParent(trAnchor);
            RemoveStackData(data);
            data.Popup.HideAnim();
            return true;
        }

        return false;
    }

    private void AddStackData(PopupData data)
    {
        var stackData = inst.GetStackPopupData(data.Type);
        if (stackData != null)
        {
            _listStackPopup.Remove(stackData);
        }

        if (!data.ignoreStackLayer)
        {
            var maxOrder = 0;
            var layeredPopups = _listStackPopup.Where(x => !x.ignoreStackLayer).ToList();
            if (layeredPopups.Count != 0)
            {
                maxOrder = layeredPopups.Max(x => x.Popup.GetComponent<Canvas>().sortingOrder);
            }

            data.Popup.GetComponent<Canvas>().sortingOrder = maxOrder + 10;
        }

        _listStackPopup.Add(data);

        if (data.Popup.GetComponent<Canvas>().worldCamera != null)
        {
            data.Popup.GetComponent<Canvas>().worldCamera.depth = 0.001f * data.Popup.GetComponent<Canvas>().sortingOrder;
        }
    }

    private void RemoveStackData(PopupData data)
    {
        _listStackPopup.Remove(data);
    }

    private PopupBase AddFirstLayer(ePopupType type)
    {
        if (_listPopupLayer.Count > 0)
        {
            _listPopupLayer.Insert(0, type);
            return CreatePopup(type, false).Popup;
        }
        else
        {
            return ShowPopup(type);
        }
    }

    private PopupData CreatePopup(ePopupType type, bool isShow, Transform trParent = null)
    {
        if (!Valid)
        {
            return null;
        }

        var popupData = listPopupData.Find(d => d.Type == type);
        var goPopup = popupData.Popup.gameObject;
        var popup = Instantiate(goPopup).GetComponent<PopupBase>();
        var trPopup = popup.transform;
        trPopup.SetParent(trParent == null ? trAnchor : trParent);
        trPopup.localPosition = Vector3.zero;
        trPopup.localRotation = Quaternion.Euler(Vector3.zero);
        trPopup.localScale = Vector3.one;
        popup.SetType(popupData);

        if (!Application.isPlaying)
        {
            Debug.LogError("Application is not playing!!");
        }

        PopupData data = new();
        data.Type = type;
        data.Popup = popup;
        data.ignoreStackLayer = listPopupData.Find(x => x.Type == type).ignoreStackLayer;
        data.ignoreStackCount = listPopupData.Find(x => x.Type == type).ignoreStackCount;
        _listActivePopup.Add(data);

        if (isShow)
        {
            popup.Show();
        }
        else
        {
            popup.gameObject.SetActive(false);
        }

        return data;
    }

    private PopupData GetStackPopupData(ePopupType type)
    {
        PopupData data = null;

        for (int i = 0; i < _listStackPopup.Count; i++)
        {
            if (_listStackPopup[i].Type == type)
            {
                data = _listStackPopup[i];
                break;
            }
        }

        return data;
    }

    private PopupData GetPopupData(ePopupType type)
    {
        PopupData data = null;

        for (int i = 0; i < _listActivePopup.Count; i++)
        {
            if (_listActivePopup[i].Type == type)
            {
                data = _listActivePopup[i];
                break;
            }
        }

        return data;
    }

    private PopupData GetPopupData(PopupBase popup)
    {
        PopupData data = null;

        for (int i = 0; i < _listActivePopup.Count; i++)
        {
            if (_listActivePopup[i].Popup == popup)
            {
                data = _listActivePopup[i];
                break;
            }
        }

        return data;
    }

    private T GetPopupData<T>() where T : Component
    {
        for (int i = 0; i < _listActivePopup.Count; i++)
        {
            var component = _listActivePopup[i].Popup.GetComponent<T>();

            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    #endregion

    #region lifecycle

    protected override void OnDestroy()
    {
        CompleteLayer.RemoveAll();
        HidePopupEvent.RemoveAll();
        base.OnDestroy();
    }

    #endregion
}

[Serializable]
public class PopupData
{
    [Header("Required")]
    public ePopupType Type;

    public PopupBase Popup;

    [Header("Option")]
    public bool ignoreStackLayer; //스택의 레이어를 고정함

    public bool ignoreStackCount; //스택 카운트에 포함 안시킴
}
