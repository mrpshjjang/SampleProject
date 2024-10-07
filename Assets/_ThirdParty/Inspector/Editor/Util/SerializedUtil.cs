/*
* Copyright (c) Sample.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Sample.Inspector.Editor
{
    internal static class SerializedUtil
    {
        /// <summary>
        /// 오브젝트로 반환 한다
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="getParent">부모 오브젝트 반환?</param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty prop, bool getParent)
        {
            if (prop == null)
            {
                return null;
            }

            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            List<string> elements = path.Split('.').ToList();
            if (getParent)
            {
                elements.RemoveAt(elements.Count - 1);
            }

            foreach (string element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("[", StringComparison.InvariantCulture));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.InvariantCulture)).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                {
                    return f.GetValue(source);
                }

                PropertyInfo p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                {
                    return p.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (var i = 0; i <= index; i++)
            {
                if (!enm.MoveNext())
                {
                    return null;
                }
            }

            return enm.Current;
        }
    }
}
