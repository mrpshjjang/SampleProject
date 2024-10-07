/*
* Copyright (c) Sample.
*/

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace Sample.SpecData.Editor
{
    /// <summary>
    /// Spec Data 검증기
    /// </summary>
    public static class SpecDataValidator
    {
        private const string InterfaceValidator = "ISpecDataManagerValidator";
        private const string ValidManager = "SpecDataValidManager";
        private const string MethodValidate = "Validate";
        private const string MethodLoad = "Load";

        private const string ResourceLoader = "SpecDataResourceLoader";
        private const string MethodLoadSpecData = "LoadSpecData";

        private static string ReservedValidKey => $"SpecDataValidatorReservedValid-{Application.dataPath.GetHashCode()}";

        /// <summary>
        /// 컴파일 이후 검증 예약 검사
        /// </summary>
        [InitializeOnLoadMethod]
        private static void OnInitialized()
        {
            if (!EditorPrefs.GetBool(ReservedValidKey))
            {
                return;
            }

            EditorPrefs.DeleteKey(ReservedValidKey);
            Valid();
        }

        private static void ReservedValid()
        {
            EditorPrefs.SetBool(ReservedValidKey, true);
        }

        internal static void AutoValid()
        {
            // 컴파일중이면 검증 예약
            if (EditorApplication.isCompiling)
            {
                ReservedValid();
            }
            else
            {
                // 컴파일중 아니면 바로 검증
                Valid();
            }
        }

        /// <summary>
        /// 검증 (Editor에서 사용시 모든 SpecData 검증을 진행합니다.
        /// </summary>
        /// <returns>검증 결과</returns>
        public static bool Valid()
        {
            (Type Validator, Type ResourceLoader) type = GetValidator();

            if (type.Validator == default || type.ResourceLoader == default)
            {
                Debug.unityLogger.Log(Define.Tag, "<color=red>CS 생성 먼저 해주세요</color>");
                return false;
            }

            MethodInfo methodLoadSpecData = type.ResourceLoader.GetMethod(MethodLoadSpecData);
            var json = (string)methodLoadSpecData!.Invoke(null, null);

            if (string.IsNullOrEmpty(json))
            {
                Debug.unityLogger.Log(Define.Tag, "<color=red>Spec Data json 로드 실패</color>");
                return false;
            }

            object instance = Activator.CreateInstance(type.Validator);
            MethodInfo methodLoad = type.Validator.GetMethod(MethodLoad);
            MethodInfo methodValidate = type.Validator.GetMethod(MethodValidate);

            if ((bool) methodLoad!.Invoke(instance, new object[] {json}) == false)
            {
                Debug.unityLogger.Log(Define.Tag, "<color=red>Json Load 실패</color>");
                return false;
            }

            var result = (bool)methodValidate!.Invoke(instance, null);
            if (result)
            {
                Debug.unityLogger.Log(Define.Tag, "<color=green>검증 성공</color>");
            }
            else
            {
                Debug.unityLogger.Log(Define.Tag, "<color=red>검증 실패</color>");
            }

            return result;
        }

        /// <summary>
        /// SpecDataValidManager, SpecDataResourceLoader Type 찾기
        /// </summary>
        /// <returns></returns>
        private static (Type Validator, Type ResourceLoader) GetValidator()
        {
            (Type Validator, Type ResourceLoader) result;
            result.Validator = default;
            result.ResourceLoader = default;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (TypeInfo typeInfo in assembly.DefinedTypes)
                {
                    if(!typeInfo.IsClass)
                        continue;

                    if (typeInfo.Name == ValidManager && typeInfo.GetInterface(InterfaceValidator) != default)
                        result.Validator = typeInfo;

                    if (typeInfo.Name == ResourceLoader && typeInfo.GetMethod(MethodLoadSpecData) != default)
                        result.ResourceLoader = typeInfo;
                }
            }

            return result;
        }
    }
}
