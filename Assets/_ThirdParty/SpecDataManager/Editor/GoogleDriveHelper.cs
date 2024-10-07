/*
 * Copyright (c) Sample.
 */

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sample.GoogleApis.Editor;
using ExcelDataReader;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using UnityEditor;

namespace Sample.SpecData.Editor
{
    internal static class GoogleDriveHelper
    {
        private static readonly GoogleAppInfo AppInfo = new()
        {
            // ClientId = Encoding.UTF8.GetString(new byte[]
            // {
            //     49, 48, 54, 49, 52, 57, 55, 54, 56, 53, 50, 48, 51, 45, 111, 115, 97, 114, 55, 52, 112, 109, 48, 50,
            //     114, 105, 114, 97, 56, 98, 117, 113, 111, 105, 110, 111, 57, 49, 115, 53, 112, 50, 117, 103, 53, 56, 46,
            //     97, 112, 112, 115, 46, 103, 111, 111, 103, 108, 101, 117, 115, 101, 114, 99, 111, 110, 116, 101, 110,
            //     116, 46, 99, 111, 109,
            // }),
            // ClientSecret = Encoding.UTF8.GetString(new byte[]
            // {
            //     71, 79, 67, 83, 80, 88, 45, 119, 72, 107, 73, 86, 79, 51, 110, 111, 109, 113, 71, 90, 68, 100, 99, 104,
            //     118, 65, 49, 68, 98, 77, 121, 68, 52, 107, 66,
            // }),

            ClientId = "348397097335-edlndf13eaqqr1kki72pn4bg79r08v30.apps.googleusercontent.com",
            ClientSecret = "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",

            Identifier = "TestProject",
            Scopes = new[] {DriveService.Scope.DriveReadonly},
        };

        public static bool IsGoogleLogin()
        {
            return AppInfo.IsSignIn();
        }

        public static void GoogleLogout()
        {
            AppInfo.SignOut();
        }

        public static async Task<IEnumerable<GoogleDriveFileInfo>> AsyncGetDriveSheetFileList(string contains_filter)
        {
            string[] filters = contains_filter.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            return await AppInfo.GetDriveSheetFileListAsync(filters);
        }

        public static IExcelDataReader DownloadSheet(GoogleDriveFileInfo googleDriveFileInfo)
        {
            EditorUtility.DisplayProgressBar("Google Sheet Download",
                $"Google Sheet Download : {googleDriveFileInfo.Name}", 0);
            using DriveService service = CreateDriveService();
            Task<HttpResponseMessage> data = service.HttpClient.GetAsync(
                $"https://docs.google.com/spreadsheets/export?id={googleDriveFileInfo.Id}&exportFormat=xlsx");
            data.Wait();
            EditorUtility.ClearProgressBar();

            if (!data.Result.IsSuccessStatusCode)
            {
                return default;
            }

            IExcelDataReader reader = ExcelReaderFactory.CreateReader(data.Result.Content.ReadAsStreamAsync().Result);
            return reader;
        }

        private static DriveService CreateDriveService()
        {
            ICredential credential = AppInfo.SignIn();
            if (credential == null)
            {
                return null;
            }

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
            return service;
        }
    }
}
