/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CookApps.PackageTemplate.Editor
{
    internal static class YamlUtil
    {
        // yaml의 indent size
        public const int IndentSize = 2;

        // string 앞에 space의 개수를 가져옴
        public static int GetSpaceCount(string str)
        {
            var count = 0;
            foreach (char t in str)
            {
                if (char.IsWhiteSpace(t))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        // string 앞에 space를 추가
        public static string CreateStringWithSpace(string str, int spaceCount)
        {
            return new string(' ', spaceCount) + str;
        }

        // key: value 형식의 yaml에서 key의 line을 가져옴
        private static int GetKeyLine(string[] yaml, string key)
        {
            for (var i = 0; i < yaml.Length; i++)
            {
                if (yaml[i].Contains(key))
                {
                    return i;
                }
            }

            return -1;
        }

        // key: value 형식의 yaml에서 자식 value를 가져옴
        private static string[] GetChildrenItems(string[] yaml, string key)
        {
            var items = new List<string>();
            int keyLine = GetKeyLine(yaml, key);
            int index = keyLine + 1;
            while (true)
            {
                if (yaml[index].Contains("- "))
                {
                    items.Add(yaml[index].Replace("-", "").Trim());
                    index++;
                }
                else
                {
                    break;
                }
            }

            return items.ToArray();
        }

        // key: value 형식의 yaml에서 자식 value를 가져옴
        public static string[] GetChildrenItems(string filePath, string key)
        {
            string[] yaml = File.ReadAllLines(filePath);
            return GetChildrenItems(yaml, key);
        }

        // key: value 형식의 yaml에서 자식 value를 설정
        public static string[] SetChildrenItems(string[] yaml, string key, IEnumerable<string> items)
        {
            int keyLine = GetKeyLine(yaml, key);
            if (keyLine == -1)
                throw new Exception($"key: {key} not found");
            if(items == null)
                throw new Exception($"items is null");

            List<string> listContent = yaml.ToList();

            // 기존의 자식 value 삭제
            string keyLineStr = yaml[keyLine];
            int keyLineSpaceCount = GetSpaceCount(keyLineStr);
            int index = keyLine + 1;
            while (true)
            {
                string item = listContent[index];
                if (string.IsNullOrEmpty(item))
                {
                    break;
                }

                if (GetSpaceCount(item) > keyLineSpaceCount)
                {
                    listContent.RemoveAt(index);
                    continue;
                }
                break;
            }

            index = keyLine + 1;
            listContent.InsertRange(index, items.Select(item => CreateStringWithSpace($"- {item}", keyLineSpaceCount + IndentSize)));
            return listContent.ToArray();
        }

        // key: value 형식의 yaml에서 자식 value를 설정
        public static void SetChildrenItems(string filePath, string key, IEnumerable<string> items)
        {
            string[] yaml = File.ReadAllLines(filePath);
            string[] newContent = SetChildrenItems(yaml, key, items);
            File.WriteAllLines(filePath, newContent);
        }
    }
}
