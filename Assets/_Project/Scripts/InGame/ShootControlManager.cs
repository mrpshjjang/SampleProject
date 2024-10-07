using UnityEngine;
using TAGUNI;
using System;

public class ShootControlManager : TSingleton<ShootControlManager>
{
    // 사용가능한 각도
    private readonly float LIMIT_ANGLE = 25f;

    // 현재 터치상태
    public enum eTouchStatus
    {
        NONE,
        TOUCHING,
        SWAP,
    }

    // 터치상태 저장변수
    private eTouchStatus _touchStatus;
    public eTouchStatus TouchStatus
    {
        get
        {
            return _touchStatus;
        }
        private set
        {
            if (_touchStatus != value)
            {
                _touchStatus = value;

                if (UpdateTouchStatus != null)
                    UpdateTouchStatus(_touchStatus);
            }
        }
    }

    public Vector3 ShootAngle { get; private set; }
    public float ShootAngleValue { get; private set; }

    public Action<eTouchStatus> UpdateTouchStatus;


    public override void Init()
    {
        // 초기화
        base.Init();

        // 터치상태는 터치안됨으로 초기화
        TouchStatus = eTouchStatus.NONE;

        // 게임상태 업데이트받을 함수 세팅
        //IngameManager.instance.UpdateGameStatus += updateGameStatus;
    }

    public void Dispose()
    {
        // 게임상태 업데이트받는 함수 세팅 해제
        //IngameManager.instance.UpdateGameStatus -= updateGameStatus;
    }

    private void UpdateTouchPosition(int directOption, Vector3 touchPosition)
    {
        if (TouchStatus == eTouchStatus.TOUCHING)
        {
            // 슈터 회전각 세팅
            SetShooterRotation(directOption, touchPosition);
        }
    }

    public void TouchStart(int directOption, Vector3 touchPosition)
    {
        if(IngameManager.instance.GameStatus != eGameStatus.PLAYER_TURN)
            return;
        if (TouchStatus != eTouchStatus.TOUCHING)
        {
            TouchStatus = eTouchStatus.TOUCHING;

            GuideLineManager.instance.SetGuideLine();

            UpdateTouchPosition(directOption, touchPosition);
        }
    }

    public void TouchDrag(int directOption, Vector3 touchPosition)
    {
        if (TouchStatus == eTouchStatus.TOUCHING)
            UpdateTouchPosition(directOption, touchPosition);
    }

    public void TouchEnd(int directOption, Vector3 touchPosition)
    {
        // 터치상태를 터치안됨으로 초기화
        if (TouchStatus != eTouchStatus.NONE)
            TouchStatus = eTouchStatus.NONE;

        // 슈터 회전각 세팅
        SetShooterRotation(directOption, touchPosition);

        // 인게임매니저에 실제 슈팅시작함수 호출 / 소수점 버리고 계산 (타겟팅이 흔들리는 문제 수정)
        //IngameViewManager.instance.BubbleShooter.BubbleShooting(ShootAngle.z);
        IngameViewManager.instance.BubbleShooter.BubbleShooting(ShootAngleValue);
#if USE_CHEAT
        IngameViewManager.instance.ClearScoreInfo();
#endif

        ResetTouch();
    }

    public void TouchSwap ()
    {
        // 스왑상태가 아니었다면 스왑 시작
        if (TouchStatus != eTouchStatus.SWAP)
            TouchStatus = eTouchStatus.SWAP;

        IngameViewManager.instance.BubbleShooter.SwapBubble(ResetTouch);
    }

    private void SetShooterRotation(int directOption, Vector3 touchPosition)
    {
        // 터치지점기준으로 슈터회전각 계산
        Vector3 diff = IngameViewManager.instance.IngameCamera.ScreenToWorldPoint(touchPosition) - IngameViewManager.instance.BubbleShooter.transform.position;
        diff.Normalize();
        float rotationZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // 사용각도를 벗어났을 경우, 터치를 취소시킴
        if (directOption == -1 && rotationZ < LIMIT_ANGLE || rotationZ > (180f - LIMIT_ANGLE))
        {
            ResetTouch();
        }
        else if (directOption == 1 && rotationZ > -LIMIT_ANGLE || rotationZ < -(180 - LIMIT_ANGLE))
        {
            ResetTouch();
        }
        else
        {
            // 사용각도 내에서만
            // 슈터회전 적용
            ShootAngle = Quaternion.Euler(0f, 0f, rotationZ + 90f * directOption).eulerAngles;
            ShootAngleValue = (float)Math.Round(ShootAngle.z, Common.SHOOT_ANGLE_ROUND_POINT);

            // 가이드라인 업데이트
            GuideLineManager.instance.UpdateGuideLine();
        }
    }

    public void ResetTouch()
    {
        // 터치 취소처리
        TouchStatus = eTouchStatus.NONE;

        // 가이드라인 종료
        //GuideLineManager.instance.UpdateGuideLine();
        GuideLineManager.instance.ResetGuideLine();
    }

    private void updateGameStatus(eGameStatus gameStatus)
    {
        // 플레이어턴이 아닐경우, 터치 취소
        if (gameStatus != eGameStatus.PLAYER_TURN && TouchStatus != eTouchStatus.NONE)
            ResetTouch();
    }
}
