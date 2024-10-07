using UnityEngine;

// 인게임 상태
public enum eGameStatus
{
    NONE,
    PLAYER_TURN,    // 유저 차례 : 버블을 쏠 수 있는 상태
    SHOOTING,       // 슈팅 : 버블이 날라가고있는 상태
    SHOOT_RESULT,   //
    BUBBLE_TURN,    // 버블 차례 : 액션이 있는 버블들이 행동하는 상태
    GAME_START,     // 인게임에 돌입했을 때
    GAME_CLEAR,     // 인게임을 클리어했을 때
    GAME_FAILD,     // 인게임을 실패했을 때
    GAME_GIVEUP,    // 인게임에서 나갔을 때
    GAME_PAUSE,     // 게임을 멈췄을 때
}

public enum eMapBubbleStatusType
{
    NONE,       //
    ON_MAP,     // 맵에 올라가 있음
    DESTROY,    // 맵에서 파괴됨
    DROP,       // 연결을 잃고, 떨어지기
    BOTTOM_DESTROY // 하단에 내려와서 파괴
}

// 슛버블 상태
public enum eShootBubbleStatusType
{
    NONE,
    READY_MAIN, // 슛버블 상태
    READY_SUB,  // 대기버블 상태
    MOVING,     // 슈팅된 상태
    MOVE_END,   // 슈팅이 완료된 상태
}

// 슛버블의 슈팅방향
public enum eShootBubbleDirType
{
    NONE,
    UP,
    UP_RIGHT,
    UP_LEFT,
    DOWN,
    DOWN_RIGHT,
    DOWN_LEFT,
}

// 맵 캔버스의 상태
public enum eMapStatus
{
    NONE,
    MAP_MOVE,   // 캔버스 이동중
    COMPLETE,   // 모든 행동 완료(대기상태)
}

public enum eGameResultType
{
    NONE,
    CLEAR,
    FAILD,
}

public enum eDestroyType
{
    NONE,
    NORMAL,
    FINAL,
    FLOATING,
    SKILL,
    HIT_ITEM,
    BURST,
    POPPING,
    MIRROR,
    BOMB_ITEM, //아이템 BOMB
    LINE_ITEM //아이템 LASER
}

public enum eHeartStatus
{
    NONE,
    FULL,
    CHARGE,
    INFINITY,
}

public enum eIngameItemType
{
    NONE,
    BOMB_TYPE,
    LINE_TYPE,
    LASER_TYPE,
}

public static class Common
{
    public static readonly int BUBBLE_COLLEPSE_COUNT = 3;           // 버블이 터지는 기준 갯수
    public static readonly int BUBBLE_WIDTH_COUNT = 11;             // 첫라인 버블 가로 갯수

    public static readonly int BUBBLE_ACTIVE_HEIGHT_COUNT = 14;     // 액티브되어있는 버블 라인 수

    public static readonly int SKILL_DEFAULT_CHARGE_VALUE = 9;      // 스킬충전 포인트(첫 포인트)
    public static readonly int SKILL_COMBO_CHARGE_VALUE = 13;       // 스킬충전 포인트(콤보 포인트)
    public static readonly int SKILL_FULL_CHARGE_VALUE = 100;       // 스킬완충 포인트

    public static readonly int ALREADY_SET_MAPDATA_Y_COUNT = 2;     // 2차원 맵데이터를 최하단버블보다 미리 생성해 놓을 라인(Y) 수

    public static readonly int SHOOT_ANGLE_ROUND_POINT = 2;      // 슈팅각도 반올림 자릿수

    public static readonly float BUBBLE_HEIGHT_GAP_SIZE = 58f;      // 버블 세로 간격
    public static readonly float MAP_PLACED_TIME = 0.08f;            // 맵이 배치되는연출 시간
    public static readonly float ONE_BUBBLE_MOVE_TIME = 0.06f;      // 맵캔버스 움직일 때, 버블 한줄 움직이는 시간

    public static readonly float MULTI_SCORE_BUFF_VALUE = 1.2f;

    // 버블 이미지 지름
    public static readonly float BUBBLE_DIAMETER = 64;
    public static readonly float BOSS_BUBBLE_DIAMETER = 192f;
    // 슛버블 충돌체 지름
    public static readonly float SHOOT_BUBBLE_DIAMETER = 64f;

    public static readonly string GAME_OBJECT_RESOUCE_PATH = "Prefabs/";
    public static readonly string UI_RESOUCE_PATH = "Prefabs/Ingame/UI/";

    public static readonly Vector2 VECTOR2_ZERO = Vector2.zero;
    public static readonly Vector3 VECTOR3_ZERO = Vector3.zero;
    public static readonly Vector3 VECTOR3_ONE = Vector3.one;

    public static readonly Color COLOR_ALPHA = new Color(1f, 1f, 1f, 0f);
    public static readonly Color COLOR_NONE_ALPHA = new Color(1f, 1f, 1f, 1f);
}
