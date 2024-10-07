/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    internal class StyleUtil
    {
        public static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 1.5f;
        public static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
        public static readonly Color errorBackgroundColor = Color.red;
        public static readonly Color warningBackgroundColor = Color.yellow;
        public static readonly Color normalBackgroundColor = Color.white;
    }
}
