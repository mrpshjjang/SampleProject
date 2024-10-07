#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using UnityEngine;

public static class Debug
{
    public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

    public static ILogger unityLogger => UnityEngine.Debug.unityLogger;

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message, Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(logType, logOptions, context, format, args);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogFormat(Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogColor(object message, Color? color = null)
    {
        var colorStr = "yellow";
        if (color != null)
        {
            colorStr = ColorUtility.ToHtmlStringRGBA(color.Value);
        }

        UnityEngine.Debug.Log("<color='#" + colorStr + "'>" + message + "</color>");
    }


    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message.ToString());
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message, Object context)
    {
        UnityEngine.Debug.LogWarning(message.ToString(), context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default, float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default, float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Assert(bool condition)
    {
        if (!condition)
        {
            throw new System.Exception();
        }
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Assert(bool condition, Object context)
    {
        UnityEngine.Debug.Assert(condition, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Assert(bool condition, object context)
    {
        UnityEngine.Debug.Assert(condition, context);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogAssertion(string message)
    {
        UnityEngine.Debug.LogAssertion(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogException(System.Exception e)
    {
        UnityEngine.Debug.LogException(e);
    }
}
