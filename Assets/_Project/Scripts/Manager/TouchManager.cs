using UnityEngine;

public class TouchManager : MonoBehaviour
{
    private enum eTouchArea
    {
        NONE,
        GAME_TOP_AREA,
        GAME_BOTTOM_AREA,
        SHOOTER,
    }

    private bool _isTouchStart;
    private eTouchArea _touchingArea;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _isTouchStart = false;
        _touchingArea = eTouchArea.NONE;
    }

    public void Dispose()
    {
        _isTouchStart = false;
        _touchingArea = eTouchArea.NONE;
    }

    private void Update()
    {
        if (IngameManager.instance.GameStatus == eGameStatus.PLAYER_TURN)
            CheckTouch();
    }

    private void CheckTouch()
    {
        if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            if (Input.touchCount > 0 && Input.touches != null)
            {
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        TouchDown(Input.touches[i].position);
                        break;
                    }
                    else if ((Input.touches[i].phase == TouchPhase.Ended) || (Input.touches[i].phase == TouchPhase.Canceled))
                    {
                        TouchUp(Input.touches[i].position);
                        break;
                    }
                    else if ((Input.touches[i].phase == TouchPhase.Moved) || (Input.touches[i].phase == TouchPhase.Stationary))
                    {
                        TouchMove(Input.touches[i].position);
                        break;
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                TouchDown(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                TouchUp(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                TouchMove(Input.mousePosition);
            }
        }
    }

    // 터치 다운
    private void TouchDown(Vector3 tPos)
    {
        _isTouchStart = true;

        if (IngameManager.instance.GameStatus != eGameStatus.PLAYER_TURN)
            return;

        _touchingArea = eTouchArea.NONE;
        RaycastHit[] hits = Physics.RaycastAll(IngameViewManager.instance.IngameCamera.ScreenPointToRay(tPos));

        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.name.IndexOf("TopGameTouchArea") > -1)
                {
                    // 플레이어 턴이고, 터치중이 아닐때만 터치시작 가능
                    if (ShootControlManager.instance.TouchStatus == ShootControlManager.eTouchStatus.NONE &&
                        IngameViewManager.instance.BubbleShooter.isShootReady)
                    {
                        _touchingArea = eTouchArea.GAME_TOP_AREA;
                        ShootControlManager.instance.TouchStart(-1, tPos);
                    }
                }
                else if (hits[i].collider.gameObject.name.IndexOf("BottomGameTouchArea") > -1)
                {
                    // 플레이어 턴이고, 터치중이 아닐때만 터치시작 가능
                    if (ShootControlManager.instance.TouchStatus == ShootControlManager.eTouchStatus.NONE &&
                        IngameViewManager.instance.BubbleShooter.isShootReady)
                    {
                        _touchingArea = eTouchArea.GAME_BOTTOM_AREA;
                        ShootControlManager.instance.TouchStart(1, tPos);
                    }
                }
                else if (hits[i].collider.gameObject.name.IndexOf("BubbleShooter") > -1)
                {
                    // 플레이어 턴이고, 터치중이 아닐때만 터치시작 가능
                    if (ShootControlManager.instance.TouchStatus == ShootControlManager.eTouchStatus.NONE &&
                        IngameViewManager.instance.BubbleShooter.isShootReady)
                    {
                        _touchingArea = eTouchArea.SHOOTER;
                    }
                }
            }
        }
    }

    // 터치 무브
    private void TouchMove(Vector3 tPos)
    {
        if (!_isTouchStart || IngameManager.instance.GameStatus != eGameStatus.PLAYER_TURN)
            return;

        eTouchArea checkArea = eTouchArea.NONE;

        RaycastHit[] hits = Physics.RaycastAll(IngameViewManager.instance.IngameCamera.ScreenPointToRay(tPos));

        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.name.IndexOf("TopGameTouchArea") > -1)
                {
                    // 플레이어 턴이고, 터치중이 아닐때만 터치시작 가능
                    if (IngameViewManager.instance.BubbleShooter.isShootReady)
                    {
                        checkArea = eTouchArea.GAME_TOP_AREA;
                        ShootControlManager.instance.TouchDrag(-1, tPos);
                    }
                }
                else if (hits[i].collider.gameObject.name.IndexOf("BottomGameTouchArea") > -1)
                {
                    // 플레이어 턴이고, 터치중이 아닐때만 터치시작 가능
                    if (IngameViewManager.instance.BubbleShooter.isShootReady)
                    {
                        checkArea = eTouchArea.GAME_BOTTOM_AREA;
                        ShootControlManager.instance.TouchDrag(1, tPos);
                    }
                }
            }
        }

        if (checkArea != eTouchArea.NONE && (_touchingArea == eTouchArea.NONE || ShootControlManager.instance.TouchStatus != ShootControlManager.eTouchStatus.TOUCHING))
        {
            _touchingArea = checkArea;

            switch (_touchingArea)
            {
                case eTouchArea.GAME_TOP_AREA:
                    ShootControlManager.instance.TouchStart(-1, tPos);
                    break;
                case eTouchArea.GAME_BOTTOM_AREA:
                    ShootControlManager.instance.TouchStart(1, tPos);
                    break;
            }
        }
        else if (checkArea != eTouchArea.GAME_TOP_AREA && checkArea != eTouchArea.GAME_BOTTOM_AREA)
        {
            if (_touchingArea != eTouchArea.SHOOTER)
                _touchingArea = eTouchArea.NONE;

            ShootControlManager.instance.ResetTouch();
        }
    }

    // 터치 업
    private void TouchUp(Vector3 tPos)
    {
        if (!_isTouchStart)
            return;
        else
            _isTouchStart = false;

        if (IngameManager.instance.GameStatus != eGameStatus.PLAYER_TURN)
        {
            return;
        }

        switch (_touchingArea)
        {
            case eTouchArea.GAME_TOP_AREA:
                ShootControlManager.instance.TouchEnd(-1, tPos);
                break;
            case eTouchArea.GAME_BOTTOM_AREA:
                ShootControlManager.instance.TouchEnd(1, tPos);
                break;
            case eTouchArea.SHOOTER:
                if (IngameViewManager.instance.BubbleShooter.isShootReady)
                    ShootControlManager.instance.TouchSwap();
                break;
        }

        _touchingArea = eTouchArea.NONE;
        ShootControlManager.instance.ResetTouch();
    }
}
