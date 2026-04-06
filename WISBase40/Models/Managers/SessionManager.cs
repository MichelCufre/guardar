using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using WIS.Configuration;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Security;
using WIS.Security.Models;
using WIS.Security.Serialization;
using WIS.Serialization;
using WIS.WebApplication.Extensions;

namespace WIS.WebApplication.Models.Managers
{
    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebApiClient _apiClient;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;

        const string USER_SESSION_BAG = "UserSessionBag";

        public const string UserInfo = "UserInfo";
        public const string SessionToken = "WISBASE40_SessionToken";
        public const string TooManySessions = "WISBASE40_TooManySessions";
        public const string WIS_SESSION = "WIS_SESSION";

        public SessionManager(IHttpContextAccessor httpContextAccessor,
            IOptions<ModuleUrl> moduleUrls,
            IWebApiClient apiClient,
            IOptions<ApplicationSettings> appSettings)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._apiClient = apiClient;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
        }

        public void SetValue(string key, object value)
        {
            Dictionary<string, object> sessionBag = this._httpContextAccessor.HttpContext.Session.Get<Dictionary<string, object>>(USER_SESSION_BAG);

            if (sessionBag == null)
                sessionBag = new Dictionary<string, object>();

            if (value != null)
                sessionBag[key] = value;
            else
                sessionBag.Remove(key);

            this._httpContextAccessor.HttpContext.Session.Set(USER_SESSION_BAG, sessionBag);

        }

        public object GetValue(string key)
        {
            Dictionary<string, object> sessionBag = this._httpContextAccessor.HttpContext.Session.Get<Dictionary<string, object>>(USER_SESSION_BAG);

            if (sessionBag == null)
                return null;

            if (!sessionBag.ContainsKey(key))
                return null;

            return sessionBag[key];
        }

        public T GetValue<T>(string key)
        {
            Dictionary<string, object> sessionBag = this._httpContextAccessor.HttpContext.Session.Get<Dictionary<string, object>>(USER_SESSION_BAG);

            if (sessionBag == null)
                return default(T);

            if (!sessionBag.ContainsKey(key))
                return default(T);

            return (T)sessionBag[key];
        }

        public Usuario GetUserInfo()
        {
            var user = this.GetValue<Usuario>(SessionManager.UserInfo);

            var sub = this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimsName.Subject)?.Value;
            var role = this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimsName.Role)?.Value;
            var name = this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimsName.Name)?.Value;
            var email = this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimsName.Email)?.Value;
            var locale = this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimsName.Locale)?.Value;

            if (sub == null)
                return null;

            if (user != null && sub.Trim().ToLower() != user.Username.Trim().ToLower())
            {
                this._httpContextAccessor.HttpContext.Session.Clear();
                user = null;
            }

            if (user == null)
            {
                var transferData = new SecurityWrapper
                {
                    PageToken = ""
                };

                transferData.SetData(new SecurityRequest()
                {
                    Username = sub,
                    Name = name,
                    Email = email,
                    Language = locale,
                    Role = role,
                });

                var taskPost = this._apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, "Security/GetUserByUsername", transferData, default);
                taskPost.Wait();

                HttpResponseMessage postResponse = taskPost.Result;

                var taskContent = postResponse.Content.ReadAsStringAsync();
                taskContent.Wait();

                var content = taskContent.Result;

                if (!postResponse.IsSuccessStatusCode)
                    throw new InvalidOperationException("Error, status: " + postResponse.StatusCode + " - " + postResponse.ReasonPhrase + "-" + content);

                var taskResult = postResponse.Content.ReadAsStringAndDeserializeAsync<SecurityWrapper>();
                taskResult.Wait();

                var result = taskResult.Result;

                if (result.Status == TransferWrapperStatus.Error)
                    throw new InvalidOperationException("General_Sec0_Error_Error60");

                user = result.GetData<SecurityContent>().Usuario;

                this.SetValue(SessionManager.UserInfo, user);
            }

            return user;
        }
    }
}
