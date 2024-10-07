/*
 * Copyright (c) Sample.
 */

using System.Collections.Generic;
using Google.Apis.Drive.v3;

namespace Sample.GoogleApis.Editor
{
    /// <summary>
    /// 앱 정보 (사용자 인증 정보 -> OAuth 2.0 클라이언트 ID 참조)
    /// https://console.cloud.google.com
    /// </summary>
    public sealed class GoogleAppInfo
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public IEnumerable<string> Scopes { get; init; } = new[] {DriveService.Scope.DriveReadonly};
        public string Identifier { get; init; }
    }
}
