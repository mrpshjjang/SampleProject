/*
 * Copyright (c) Sample.
 */

using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Sample.GoogleApis.Editor
{
    internal sealed class EditorDataStoreImpl : IDataStore
    {
        internal const string SampleUser = "SampleUser";
        private readonly string _identifier;

        private struct Void
        {
        }

        private static string EditorPrefsKey(string identifier)
        {
            return $"SampleGoogleApp_{identifier}";
        }

        internal static void Clear(string identifier)
        {
            Delete(identifier);
        }

        internal static TokenResponse GetTokenResponse(string identifier)
        {
            string jsonText = Load(identifier);
            JObject json = JObject.Parse(jsonText);
            JToken node = json[SampleUser];
            var value = node?.ToObject<TokenResponse>();
            return value;
        }

        internal EditorDataStoreImpl(string identifier)
        {
            _identifier = identifier;
        }

        public Task StoreAsync<T>(string key, T value)
        {
            return Task.Factory.StartNew(() =>
            {
                var completionSource = new TaskCompletionSource<Void>();
                EditorApplication.delayCall += () =>
                {
                    string jsonValue = Load(_identifier);
                    JObject json = JObject.Parse(jsonValue);
                    json[key] = JObject.FromObject(value);
                    Save(_identifier, json);
                    completionSource.SetResult(default);
                };
                return completionSource.Task;
            });
        }

        public Task DeleteAsync<T>(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                var completionSource = new TaskCompletionSource<Void>();
                EditorApplication.delayCall += () =>
                {
                    string jsonValue = Load(_identifier);
                    JObject json = JObject.Parse(jsonValue);
                    json.Remove(key);
                    Save(_identifier, json);
                    completionSource.SetResult(default);
                };
                return completionSource.Task;
            });
        }

        public Task<T> GetAsync<T>(string key)
        {
            var completionSource = new TaskCompletionSource<T>();
            string jsonText = Load(_identifier);
            JObject json = JObject.Parse(jsonText);
            JToken node = json[key];
            if (node == null)
            {
                completionSource.SetResult(default);
                return completionSource.Task;
            }

            var value = node.ToObject<T>();
            completionSource.SetResult(value);
            return completionSource.Task;
        }

        public Task ClearAsync()
        {
            var completionSource = new TaskCompletionSource<Void>();
            Delete(_identifier);
            completionSource.SetResult(default);
            return completionSource.Task;
        }

        private static void Save(string identifier, [NotNull] JToken json)
        {
            var jsonText = json.ToString(Formatting.None);
            EditorPrefs.SetString(EditorPrefsKey(identifier), jsonText);
        }

        private static string Load(string identifier)
        {
            string jsonText = EditorPrefs.GetString(EditorPrefsKey(identifier), "{}");
            return jsonText;
        }

        private static void Delete(string identifier)
        {
            EditorPrefs.DeleteKey(EditorPrefsKey(identifier));
        }
    }
}
