/*
* Copyright (c) Sample.
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;

namespace Sample.GoogleApis.Editor
{
    internal static class GoogleDriveHelper
    {
        public static DriveService CreateDriveService(ICredential credential)
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
            return service;
        }

        /// <summary>
        /// 구글 드라이브 파일 목록 얻기
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="mimeFileType">파일 타입 (https://developers.google.com/drive/api/guides/mime-types)</param>
        /// <param name="name_contains_filter">이름 필터는 and 처리</param>
        /// <returns></returns>
        public static IEnumerable<GoogleDriveFileInfo> GetDriveFileList(ICredential credential, string mimeFileType, params string[] name_contains_filter)
        {
            //string[] filters = contains_filter.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            string[] filters = name_contains_filter;
            using DriveService service = CreateDriveService(credential);
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.SupportsAllDrives = true;
            listRequest.IncludeTeamDriveItems = true;
            listRequest.Q = $"mimeType = 'application/vnd.google-apps.{mimeFileType}' and trashed = false";
            foreach (string filter in filters)
            {
                listRequest.Q += $" and name contains '{filter}'";
            }

            listRequest.Fields = "nextPageToken, files(id, name)";
            FileList resultFiles = listRequest.Execute();
            return resultFiles.Files.Select(x => new GoogleDriveFileInfo
            {
                Name = x.Name,
                Id = x.Id,
            }).ToArray();
        }

        //// <summary>
        /// 구글 드라이브 파일 목록 얻기
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="mimeFileType">파일 타입 (https://developers.google.com/drive/api/guides/mime-types)</param>
        /// <param name="name_contains_filter">이름 필터는 and 처리</param>
        /// <returns></returns>
        public static async Task<IEnumerable<GoogleDriveFileInfo>> GetDriveFileListAsync(ICredential credential, string mimeFileType, params string[] name_contains_filter)
        {
            //string[] filters = contains_filter.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            string[] filters = name_contains_filter;
            using DriveService service = CreateDriveService(credential);
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.SupportsAllDrives = true;
            listRequest.IncludeTeamDriveItems = true;
            listRequest.Q = $"mimeType = 'application/vnd.google-apps.{mimeFileType}' and trashed = false";
            foreach (string filter in filters)
            {
                listRequest.Q += $" and name contains '{filter}'";
            }

            listRequest.Fields = "nextPageToken, files(id, name)";
            FileList resultFiles = await listRequest.ExecuteAsync();
            return resultFiles.Files.Select(x => new GoogleDriveFileInfo
            {
                Name = x.Name,
                Id = x.Id,
            }).ToArray();
        }
    }
}
