using UnityEngine;

public static class Facade
{
    public static Color COLOR_CURRENCY_LACK => Utils.GetColor("FB6969");
    public static Color COLOR_ORANGE => Utils.GetColor("FF8253");
    public static Color COLOR_YELLOW => Utils.GetColor("FFC125");
    public static Color TRIAL_INFO_COLOR => Utils.GetColor("FF8253");

    public static double DOUBLE_MAX => 1e+300;
    public static double DAMAGE_MIN { get; private set; }
    public static double WEAKNESS_DMG_UP_MULT { get; private set; }
    public static double REDUCTION_DMG_DOWN_MULT { get; private set; }
    public static int LOADING_TIMEOUT_SEC { get; private set; } = int.MaxValue;
    public static int SERVER_AUTO_RETRY_COUNT { get; private set; } = 10;
    public static int SERVER_DEADLINE_SEC { get; private set; } = 60;
    public static int SYNC_SAVE_THRES { get; private set; } = 5;
    public static int AUTO_SAVE_INTERVAL_NORMAL { get; private set; } = 10;
    public static int AUTO_SAVE_INTERVAL_POWER_SAVE { get; private set; } = 10;

    public static double STAT_MAX_SKILL_COOL_DOWN_RATE { get; private set; } = 7000;
    public static double STAT_MAX_MOVE_SPEED_RATE { get; private set; } = 10000;
    public static double STAT_MAX_ATK_SPEED_RATE { get; private set; } = 10000;

    public static SpecDataManager Spec => SpecDataManager.Instance;


    public static void Initialize()
    {
        // DAMAGE_MIN = SpecCall.Option(eOption.DAMAGE_MIN);
        // WEAKNESS_DMG_UP_MULT = 0.0001 * SpecCall.Option(eOption.WEAKNESS_DMG_UP_MULT);
        // REDUCTION_DMG_DOWN_MULT = 0.0001 * SpecCall.Option(eOption.REDUCTION_DMG_DOWN_MULT);
        // LOADING_TIMEOUT_SEC = SpecCall.Option(eOption.LOADING_TIMEOUT_SEC) > 0 ? SpecCall.Option(eOption.LOADING_TIMEOUT_SEC) : int.MaxValue;
        // SERVER_AUTO_RETRY_COUNT = SpecCall.Option(eOption.SERVER_AUTO_RETRY_COUNT) > 0 ? SpecCall.Option(eOption.SERVER_AUTO_RETRY_COUNT) : 1000;
        // SERVER_DEADLINE_SEC = SpecCall.Option(eOption.SERVER_DEADLINE_SEC) <= 0 ? 3600 : SpecCall.Option(eOption.SERVER_DEADLINE_SEC);
        // SYNC_SAVE_THRES = SpecCall.Option(eOption.SYNC_SAVE_THRES);
        // AUTO_SAVE_INTERVAL_NORMAL = SpecCall.Option(eOption.AUTO_SAVE_INTERVAL_SEC_NORMAL);
        // AUTO_SAVE_INTERVAL_POWER_SAVE = SpecCall.Option(eOption.AUTO_SAVE_INTERVAL_SEC_POWER_SAVE);
        // STAT_MAX_SKILL_COOL_DOWN_RATE = SpecCall.Option(eOption.STAT_MAX_SKILL_COOL_DOWN_RATE);
        // STAT_MAX_MOVE_SPEED_RATE = SpecCall.Option(eOption.STAT_MAX_MOVE_SPEED_RATE);
        // STAT_MAX_ATK_SPEED_RATE = SpecCall.Option(eOption.STAT_MAX_ATK_SPEED_RATE);
    }
}
