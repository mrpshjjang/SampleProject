using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static System.String;

namespace Sample.Inspector.Editor
{
    internal static class Helper
    {

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToLowerInvariant();
        }

        /// <summary>
        /// 하위 폴더에 포함되어 있는가?
        /// </summary>
        /// <param name="absoluteRootFolderPath"></param>
        /// <param name="absoluteTargetFolderPath"></param>
        /// <returns></returns>
        public static bool IsSubFolder(string absoluteRootFolderPath, string absoluteTargetFolderPath)
        {
            var rootDir = new DirectoryInfo(absoluteRootFolderPath);
            var targetDir = new DirectoryInfo(absoluteTargetFolderPath);
            do
            {
                string root = NormalizePath(rootDir.FullName);
                string target = NormalizePath(targetDir.FullName);
                if (root == target)
                {
                    return true;
                }
                targetDir = targetDir.Parent;
            } while (targetDir != null);

            return false;
        }

        /// <summary>
        /// 상대 경로 얻기
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetRelativePath(string relativeTo, string path)
        {
            if (IsNullOrEmpty(relativeTo)) throw new ArgumentNullException(nameof(relativeTo));
            if (IsNullOrEmpty(path))   throw new ArgumentNullException(nameof(path));

            var fromUri = new Uri(relativeTo);
            var toUri = new Uri(path);

            if (fromUri.Scheme != toUri.Scheme) { return path; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }



        /// <summary>
        /// 상대 경로 얻어로기
        /// </summary>
        /// <param name="absoluteRootFolderPath"></param>
        /// <param name="absoluteTargetFolderPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetRelativePath(DirectoryInfo absoluteRootFolderPath, DirectoryInfo absoluteTargetFolderPath)
        {
            var d = absoluteTargetFolderPath;
            var root1 = d.Root;
            var root2 = absoluteRootFolderPath.Root;

            if (!root1.FullName.ToUpperInvariant().Equals(root2.FullName.ToUpperInvariant()))
            {
                throw new ArgumentException();
            }

            var filePathParts = new LinkedList<DirectoryInfo>();
            var directoryPathParts = new LinkedList<DirectoryInfo>();

            while (d != null
                   && !d.Equals(d.Root))
            {
                filePathParts.AddFirst(new DirectoryInfo(d.Name));
                d = d.Parent;
            }

            d = absoluteRootFolderPath;
            while (d != null
                   && !d.Equals(d.Root))
            {
                directoryPathParts.AddFirst(new DirectoryInfo(d.Name));
                d = d.Parent;
            }

            // remove all common directories at the head of the list.
            while (directoryPathParts.Count > 0
                   && filePathParts.Count > 0)
            {
                var d1 = filePathParts.First.Value;
                var d2 = directoryPathParts.First.Value;
                if (d1.Name.ToUpperInvariant().Equals(d2.Name.ToUpperInvariant()))
                {
                    filePathParts.RemoveFirst();
                    directoryPathParts.RemoveFirst();
                }
                else
                {
                    break;
                }
            }

            var sb = new StringBuilder();
            //sb.Append(".\\");

            // add in a "..\" for each directory part left in the "from" list
            for (var i = 0; i < directoryPathParts.Count; i++)
            {
                sb.Append("../");
            }

            // add in the directory name for each directory part left in the "to" list
            var arrayFilePathParts = filePathParts.ToArray();
            for (var index = 0; index < arrayFilePathParts.Length; index++)
            {
                var di = arrayFilePathParts[index];
                sb.Append(di.Name);

                if (index != arrayFilePathParts.Length - 1)
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }
    }
}
