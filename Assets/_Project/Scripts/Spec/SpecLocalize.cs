using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
[System.Reflection.Obfuscation(Exclude = false, ApplyToMembers = false)]
public class SpecLocalize
{
    public string key;
    public string kr;
    public string en;
}

public static class LocalizeExtension
{
    // public static string Localize(this string key)
    // {
    //     try
    //     {
    //         var result = SpecDataManager.Instance.GetLocalizeText(key).Replace("\\n", "\n");
    //         return result.Equals(string.Empty) ? key : result;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError(key);
    //         return key;
    //     }
    // }
    //
    // public static void TextLocalize(this Text txt, string key)
    // {
    //     TextLocalize(txt, key, Array.Empty<object>());
    // }
    //
    // public static void TextLocalizeColor(this Text txt, string key, Color customColor, params object[] args)
    // {
    //     txt.TextLocalize(key, true, customColor, args);
    // }
    //
    // public static void TextLocalize(this Text txt, string key, params object[] args)
    // {
    //     txt.TextLocalize(key, false, Color.clear, args);
    // }
    //
    // public static void TextLocalize(this Text txt, string key, bool isUseCustomColor, Color customColor, params object[] args)
    // {
    //     if (txt is null)
    //     {
    //         Debug.LogError("TextLocalize:: txt is null.");
    //         return;
    //     }
    //
    //     if (!string.IsNullOrEmpty(key))
    //     {
    //         var info = new LanguageManager.TextLocalizeInfo()
    //         {
    //             txt = txt,
    //             key = key,
    //             isUseCustomColor = isUseCustomColor,
    //             color = customColor,
    //         };
    //
    //         if (args != null && args.Length > 0)
    //         {
    //             info.args = args;
    //         }
    //
    //         LanguageManager.AddTextLocalizeInfo(info);
    //
    //         if (txt.gameObject.GetComponent<TextLocalizeInfoConditioner>() is null)
    //         {
    //             txt.gameObject.AddComponent<TextLocalizeInfoConditioner>();
    //         }
    //
    //         info.LocalizeInfo();
    //     }
    //     else
    //     {
    //         txt.text = string.Empty;
    //         LanguageManager.RemoveTextLocalizeInfo(txt);
    //     }
    // }
}
