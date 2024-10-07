/*
* Copyright (c) Sample.
*/

using System.Reflection;

namespace Sample.Inspector.Editor
{
    internal class ReflectionUtil
    {
        public static MethodInfo GetMethod(object target, string methodName)
        {
            return target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }
}
