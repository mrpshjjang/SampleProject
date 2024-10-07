/*
 * Copyright (c) Sample.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using JetBrains.Annotations;

namespace Sample.GoogleApis.Editor
{
    public static class GoogleAppInfoExtension
    {
        public static ICredential SignIn([NotNull] this GoogleAppInfo inst, CancellationToken cancellationToken = default)
        {
            ICredential credential = Credential.GetUserCredential(inst, cancellationToken);
            return credential;
        }

        public static void SignOut([NotNull] this GoogleAppInfo inst)
        {
            EditorDataStoreImpl.Clear(inst.Identifier);
        }

        public static bool IsSignIn([NotNull] this GoogleAppInfo inst)
        {
            TokenResponse tokenResponse = EditorDataStoreImpl.GetTokenResponse(inst.Identifier);
            return tokenResponse?.ExpiresInSeconds != null;
        }

        /// <summary>
        /// Google Sheet File 리스트 얻기
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name_contains_filter">이름 필터는 and 처리</param>
        /// <returns></returns>
        public static IEnumerable<GoogleDriveFileInfo> GetDriveSheetFileList([NotNull] this GoogleAppInfo inst, params string[] name_contains_filter)
        {
            ICredential credential = inst.SignIn();
            return credential == null ? Array.Empty<GoogleDriveFileInfo>() : GoogleDriveHelper.GetDriveFileList(credential, "spreadsheet", name_contains_filter);
        }

        /// <summary>
        /// Google Sheet File 리스트 얻기
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name_contains_filter">이름 필터는 and 처리</param>
        /// <returns></returns>
        public static async Task<IEnumerable<GoogleDriveFileInfo>> GetDriveSheetFileListAsync([NotNull] this GoogleAppInfo inst, params string[] name_contains_filter)
        {
            ICredential credential = inst.SignIn();
            if (credential == null)
            {
                return Array.Empty<GoogleDriveFileInfo>();
            }
            return await GoogleDriveHelper.GetDriveFileListAsync(credential, "spreadsheet", name_contains_filter);
        }

        /// <summary>
        /// Google Drive File 리스트 얻기
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="mimeFileType">파일 타입 (https://developers.google.com/drive/api/guides/mime-types)</param>
        /// <param name="name_contains_filter">이름 필터는 and 처리</param>
        /// <returns></returns>
        public static IEnumerable<GoogleDriveFileInfo> GetDriveFileList([NotNull] this GoogleAppInfo inst, string mimeFileType, params string[] name_contains_filter)
        {
            ICredential credential = inst.SignIn();
            return credential == null ? Array.Empty<GoogleDriveFileInfo>() : GoogleDriveHelper.GetDriveFileList(credential, mimeFileType, name_contains_filter);
        }

        /// <summary>
        /// Google Drive File 리스트 얻기
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="mimeFileType">파일 타입 (https://developers.google.com/drive/api/guides/mime-types)</param>
        /// <param name="name_contains_filter">이름 필터는 and 처리</param>
        /// <returns></returns>
        public static async Task<IEnumerable<GoogleDriveFileInfo>> GetDriveFileListAsync([NotNull] this GoogleAppInfo inst, string mimeFileType, params string[] name_contains_filter)
        {
            ICredential credential = inst.SignIn();
            if (credential == null)
            {
                return Array.Empty<GoogleDriveFileInfo>();
            }
            return await GoogleDriveHelper.GetDriveFileListAsync(credential, mimeFileType, name_contains_filter);
        }
    }
}
