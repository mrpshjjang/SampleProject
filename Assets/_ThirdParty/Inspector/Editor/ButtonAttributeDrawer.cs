/*
* Copyright (c) Sample.
*/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomEditor(typeof(Object), false, isFallback = false)]
    public class ButtonAttributeDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (Object obj in targets)
            {
                IEnumerable<MethodInfo> methodInfos = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttributes().Any(a => a.GetType() == typeof(ButtonAttribute)));
                foreach (MethodInfo method in methodInfos)
                {
                    var attribute = (ButtonAttribute)method.GetCustomAttribute(typeof(ButtonAttribute));
                    string btnName = string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name;
                    if (GUILayout.Button(btnName))
                    {
                        method.Invoke(obj, null);
                    }
                }
            }
        }
    }
}
