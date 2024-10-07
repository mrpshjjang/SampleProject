/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Authentication;
using UnityEngine;

#pragma warning disable CS0162 // Unreachable code detected

namespace CookApps.PackageTemplate.Editor
{
    internal static class ExecuteCommand
    {
        public static (bool success, string output) RunGitHubCli(string command)
        {
            try
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                var procStartInfo =
                    new ProcessStartInfo("/bin/zsh", $"-c '{command}'")
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                string envPath = Environment.GetEnvironmentVariable("PATH");
                procStartInfo.Environment.Add("PATH", "/opt/homebrew/bin:" + envPath);
#else
                var procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + command)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
#endif
                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                proc.WaitForExit();
                string result = proc.StandardOutput.ReadToEnd();
                ProcessGitHubCliExitCode(proc.ExitCode);
                return (proc.ExitCode == 0, result);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("command exception: " + e.Message);
                return (false, string.Empty);
            }
            // ReSharper disable once HeuristicUnreachableCode
            return (false, string.Empty);
        }

        private static void ProcessGitHubCliExitCode(int exitCode)
        {
            if (exitCode == 4)
            {
                throw new AuthenticationException("GitHub CLI 인증 실패 : gh auth login 사용하여 인증을 먼저 진행해 주세요");
            }

            if (exitCode != 0)
            {
                throw new Exception($"{exitCode} 오류 발생 (GitHub CLI 설치 및 PATH 지정 확인 해주세요)");
            }
        }
    }
}
