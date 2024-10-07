using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using UnityEngine;

namespace Sample.GoogleApis.Editor
{
    internal class Credential
    {
        public static ICredential GetUserCredential(GoogleAppInfo googleAppInfo, CancellationToken cancellationToken = default)
        {
            try
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = googleAppInfo.ClientId,
                    ClientSecret = googleAppInfo.ClientSecret,
                };

                cancellationToken = cancellationToken != default
                    ? cancellationToken
                    : new CancellationTokenSource(TimeSpan.FromSeconds(120)).Token;
                Task<UserCredential> authorizeAsync = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    googleAppInfo.Scopes,
                    EditorDataStoreImpl.SampleUser,
                    cancellationToken,
                    new EditorDataStoreImpl(googleAppInfo.Identifier));

                if (!authorizeAsync.IsCompleted)
                {
                    Task.FromResult(authorizeAsync.Result).Wait(cancellationToken);
                }

                if (authorizeAsync.Status == TaskStatus.Faulted)
                {
                    throw new Exception($"구글 인증 실패.\n{authorizeAsync.Exception}");
                }

                UserCredential credential = authorizeAsync.Result;
                return credential;
            }
            catch (Exception e)
            {
                Debug.LogError("구글 인증 실패");
                return null;
            }
        }
    }
}
