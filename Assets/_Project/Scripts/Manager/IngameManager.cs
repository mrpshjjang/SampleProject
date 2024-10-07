using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAGUNI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class IngameManager : TSingleton<IngameManager>
{
    #region "Fields"

    // 현재 게임상태 변수
    private eGameStatus _gameStatus;
    public eGameStatus GameStatus
    {
        get { return _gameStatus; }
        private set
        {
            // 상태가 변경되었는지 체크
            if (_gameStatus != value)
            {
                // 변경되었다면 상태값 변경
                _gameStatus = value;

                // 변경 액션함수 호출
                if (UpdateGameStatus != null)
                    UpdateGameStatus(_gameStatus);
            }
        }
    }

    // 게임상태가 변경됐을 때, 호출될 액션함수
    public Action<eGameStatus> UpdateGameStatus;

    #endregion "Fields"

    // IngameViewmanager.Init() 보다 우선
    public void Init()
    {
        GameStatus = eGameStatus.NONE;

        // 게임매니저들 초기화
        MapManager.instance.Init();
        ShootControlManager.instance.Init();
        GameStart();
    }

    public void Dispose()
    {
        GameStatus = eGameStatus.NONE;

        MapManager.instance.Dispose();
        ShootControlManager.instance.Dispose();
    }

    public void GameStart()
    {
        GameStatus = eGameStatus.GAME_START;
    }

    public void GameEnd(bool isClear)
    {
        if (isClear)
        {
            GameStatus = eGameStatus.GAME_CLEAR;
            var popup = PopupManager.Show(ePopupType.COMMON).Cast<PopupCommon>();
            PopupManager.GetPopup<PopupCommon>().SetData(
                "PopupGameEnd", "CLEAR",
                (true, "YES", () => {
                    popup.Hide();
                    GameRetry();
                }),
                (false, "RETRY", null),
                PopupManager.GetPopup<PopupCommon>().OnClickClose);
        }
        else
        {
            GameStatus = eGameStatus.GAME_FAILD;
            var popup = PopupManager.Show(ePopupType.COMMON).Cast<PopupCommon>();
            PopupManager.GetPopup<PopupCommon>().SetData(
                "PopupGameEnd", "FAILED",
                (true, "YES", () => {
                    popup.Hide();
                    GameRetry();
                }),
                (false, "RETRY", null),
                PopupManager.GetPopup<PopupCommon>().OnClickClose);
        }
        
        //IngameViewManager.instance.BubbleShooter.ResetShootingBubble();
    }

    public void GameRetry()
    {
        Init();
        IngameViewManager.instance.Init();

        GameStart();
    }

    public void GameExit()
    {
        // IngameViewManager.instance.Dispose();
    }

    public async void StartPlayerTurn()
    {
        eGameResultType resultType = eGameResultType.NONE;

        await MapManager.instance.BubbleAction();

        if (IsGameClear())
            resultType = eGameResultType.CLEAR;
        else
        {
            if (IngameDataManager.instance.CurrentBubbleCount <= 0)
                resultType = eGameResultType.FAILD;
            else
                resultType = eGameResultType.NONE;
        }

        switch (resultType)
        {
            case eGameResultType.CLEAR:
                GameEnd(true);
                break;
            case eGameResultType.FAILD:
                GameEnd(false);
                break;
            case eGameResultType.NONE:
            default:
                IngameViewManager.instance.BubbleShooter.ReloadBubble(() =>
                {
                    GameStatus = eGameStatus.PLAYER_TURN;
                });
                break;
        }
    }

    public void StartShooting()
    {
        GameStatus = eGameStatus.SHOOTING;
    }

    public void StartCheckShootResult(BaseBubble shootBubble, IngameCollider ingameCollider)
    {
        GameStatus = eGameStatus.SHOOT_RESULT;

        CollisionManager.instance.CheckShootBubbleCollision(shootBubble, null, ingameCollider, () =>
        {
            IngameViewManager.instance.BubbleMapCanvas.SetMapCanvas(false, StartPlayerTurn);
        });
    }

    private bool IsGameClear()
    {
        var clearType = IngameDataManager.instance.ClearType;
        switch (clearType)
        {
            case eClearType.BOSS_HP:
                return IngameDataManager.instance.CurrentBossHP == IngameDataManager.instance.ClearValue;
                break;
        }

        return false;
    }
}
