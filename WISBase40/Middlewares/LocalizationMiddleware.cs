using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Http;
using WIS.Http.Extensions;

namespace WIS.WebApplication.Middlewares
{
    public class LocalizationMiddleware
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly RequestDelegate _next;
        private readonly IWebApiClient _apiClient;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;

        public LocalizationMiddleware(RequestDelegate next,
            IWebApiClient apiClient,
            IOptions<ModuleUrl> moduleUrls,
            IOptions<ApplicationSettings> appSettings)
        {
            this._next = next;
            this._apiClient = apiClient;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //CultureInfo.CurrentCulture = new CultureInfo("pt");
                //CultureInfo.CurrentUICulture = new CultureInfo("pt");

                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var logId = Guid.NewGuid();

            _logger.Error(exception, $"LogId: {logId}");

            var t = await GetTranslations();

            var text = @"
                <html>
                    <head>
                        <style type='text/css'>
                            body {
                                margin: 0;
                                padding: 0;
                                background-color: #F2F2F2;
                                font-size: 1rem;
                                font-weight: 400;
                                line-height: 1.5;
                                font-family: 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Noto Sans', 'Liberation Sans', 'Arial', sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji';
                            }
                            .jumbotron {
                                padding: 1rem;
                            }
                            .lead {
                                font-size: 1.25rem;
                                fontn-weight: 300;
                            }
                            .container {
                                width: 100%;
                                padding-left: calc(1.5rem * 0.5);
                                padding-right: calc(1.5rem * 0.5);
                                margin-right: auto;
                                margin-left: auto;
                            }
                            @media (min-width: 576px) {
                                .container {
                                        max-width: 540px;
                                }
                            }
                            @media (min-width: 768px) {
                                .container {
                                    max-width: 720px;
                                }
                            }
                            @media (min-width: 992px) {
                                .container {
                                    max-width: 960px;
                                }
                            }
                            *, *::before, *::after {
                                box-sizing: border-box;
                            }
                            .main-background {
                                position: absolute;
                                opacity: 0.07;
                                top: 0px;
                                left: 0px;
                                width: 100%;
                                height: 100%;
                                z-index: -1;
                            }
                            .main-background img {
                                width: 650px;
                                position: fixed;
                                top: 50%;
                                left: 50%;
                                transform: translate(-50%, -50%);
                            }
                            .text-danger {
                                color: #dc3545 !important;
                            }
                            h1, h2 {
                                margin-top:0;
                                margin-bottom: 0.5rem;
                                font-weight: 500;
                                line-height: 1.2;
                            }
                            h1 {
                                font-size: calc(1.375rem + 1.5vw);
                            }
                            h2 {
                                font-size: calc(1.325rem + 0.9vw);
                            }
                            a {
                                color: #0d6efe;
                            }
                            .footer {
                                font-size: 0.85rem;
                                color: #A4A4A4;
                                text-align: center;
                                position: fixed;
                                z-index: 1;
                                bottom: 0;
                                padding-bottom: 1rem;
                                width: 100%;
                            }
                            .bg-light {
                                background-color: rgba(248,249,250,1) !important;
                            }
                            p {
                                margin-top: 0;
                                margin-bottom: 1rem;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='jumbotron'>
                            <h2 class='lead'></h2>
                        </div>
                        <main class='container'>
                            <div class='main-background'>
                                <img src='/api/Image/Get?id=background'>
                            </div>
                            <hgroup>
                                <h1 class='text-danger'>" + t["Master_Sec0_metaTitle_MasterError"] + @"</h1>
                                <h2 class='text-danger'>" + t["Master_Sec0_metaTitle_MasterErrorMessage"] + @"</h2>
                            </hgroup>
                            <span class='text-danger'>" + t["Master_Sec0_metaTitle_MasterErrorDetail"] + @"<strong>" + logId + @"</strong></span>
                            <br /><br />
                            <a href='/'>" + t["Master_Sec0_metaTitle_MasterGoHome"] + @" </a>
                        </main>
                        <div class='footer bg-light'>                            
                        </div>
                    </body>
                </html>
            ";

            context.Response.Clear();
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(text);
        }

        private async Task<Dictionary<string, string>> GetTranslations()
        {
            var keys = new List<string> {
                "Master_Sec0_metaTitle_MasterError",
                "Master_Sec0_metaTitle_MasterErrorMessage",
                "Master_Sec0_metaTitle_MasterErrorDetail",
                "Master_Sec0_metaTitle_MasterGoHome",
                "Master_Sec0_metaTitle_MasterTitle",
                "Master_Sec0_metaTitle_MasterFooter"
            };

            try
            {
                HttpResponseMessage response = await this._apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, "Translation", "Translate", keys, default);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

                return await response.Content.ReadAsStringAndDeserializeAsync<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }

            var t = new Dictionary<string, string>();

            foreach (var key in keys)
                t[key] = key;
                
            return t;
        }
    }
}