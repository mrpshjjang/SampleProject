using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : GameObjectSingleton<InputManager>
{
    #region public

    public void AddEscapeStack(GameObject source, Action action)
    {
        if (_listStackInfo.Exists(x => x.source == source))
        {
            var info = _listStackInfo.FirstOrDefault(x => x.source == source);
            _listStackInfo.Remove(info);
            _listStackInfo.Add(info);

            return;
        }

        _listStackInfo.Add(new EscapeStackInfo(source, action));
    }

    public void RemoveEscapeStack(GameObject source)
    {
        if (!_listStackInfo.Exists(x => x.source == source))
        {
            return;
        }

        _listStackInfo.Remove(_listStackInfo.FirstOrDefault(x => x.source == source));
    }

    #endregion


    #region protected

    #endregion


    #region private

    private List<EscapeStackInfo> _listStackInfo = new();

    private void DoTopEscapeStack()
    {
        if (_listStackInfo.Count == 0)
        {
            return;
        }

        var activeIdx = -1;
        while (_listStackInfo.Count != 0 && activeIdx == -1)
        {
            var temp = _listStackInfo.Last();
            if (temp.source == null || !temp.source.activeInHierarchy)
            {
                _listStackInfo.Remove(temp);
                continue;
            }
            else
            {
                activeIdx = _listStackInfo.IndexOf(temp);
                break;
            }
        }

        if (activeIdx != -1)
        {
            _listStackInfo[activeIdx].action?.Invoke();
        }
    }

    private void EscapeTopView()
    {
        var topPopup = PopupManager.TopEscapablePopup;
        if (topPopup != null && topPopup.CloseByEscapeType is PopupBase.ePopupEscapeType.BLOCK_ESCAPE)
            return;

        // if (LoadingManager.Instance.IsShowingLoading())
        //     return;

        if (_listStackInfo.Count != 0)
        {
            DoTopEscapeStack();
            return;
        }

#if !UNITY_IOS
        OpenExitGamePopup();
#endif
    }

    private void OpenExitGamePopup()
    {
#if !UNITY_IOS
        if (!SpecDataManager.Initialized) return;

        // var popup = PopupManager.Show(ePopupType.COMMON).Cast<PopupCommon>();
        // PopupManager.GetPopup<PopupCommon>().SetData(
        //     "Input_Escape_ExitPopup_Title".Localize(), "Input_Escape_ExitPopup_Desc".Localize(),
        //     (true, "Input_Escape_ExitPopup_N".Localize(), () => { popup.Hide(); }),
        //     (true, "Input_Escape_ExitPopup_Y".Localize(), () => { Utils.ExitGame(); }),
        //     PopupManager.GetPopup<PopupCommon>().OnClickClose);
#endif
    }

    #endregion


    #region lifecycle

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeTopView();
        }
    }

    #endregion

    public class EscapeStackInfo
    {
        public GameObject source;
        public Action action;

        public EscapeStackInfo(GameObject source, Action action)
        {
            this.source = source;
            this.action = action;
        }
    }
}
