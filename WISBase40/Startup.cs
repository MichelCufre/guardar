using I18Next.Net.AspNetCore;
using I18Next.Net.Backends;
using I18Next.Net.Extensions;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Http;
using WIS.TrafficOfficer;
using WIS.TrafficOfficer.Configuration;
using WIS.WebApplication.Middlewares;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;
using WIS.WebApplication.Models.Menu;
using WIS.WebApplication.Security;

namespace WIS.WebApplication
{
    public class Startup
    {
        Logger _logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionAsyncFilter));
            });

            services.Configure<ApplicationSettings>(Configuration.GetSection(ApplicationSettings.Position));
            services.Configure<ModuleUrl>(Configuration.GetSection(ModuleUrl.Position));
            services.Configure<TrafficOfficerSettings>(Configuration.GetSection(TrafficOfficerSettings.Position));

            services.AddHttpContextAccessor();

            // In production, React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddTransient<IWebApiClient, WebApiClient>();
            services.AddTransient<ISessionManager, SessionManager>();

            //Manejo de sesión
            services.AddDistributedMemoryCache();
            services.AddSession(options => 
            {
                long expireMinutes = 14 * 24 * 60; // 14 dias por defecto
                long.TryParse(Configuration.GetValue<string>("AuthSettings:ExpireMinutes"), out expireMinutes);
                options.IdleTimeout = TimeSpan.FromMinutes(expireMinutes);
            });

            services.AddHttpClient();

            services.AddI18NextLocalization(i18n =>
                i18n.IntegrateToAspNetCore()
                    .AddBackend(new JsonFileBackend("ClientApp/public/locales"))
                    .UseDefaultLanguage("es")
            );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    long expireMinutes = 14 * 24 * 60; // 14 dias por defecto
                    long.TryParse(Configuration.GetValue<string>("AuthSettings:ExpireMinutes"), out expireMinutes);

                    var clientId = Configuration.GetValue<string>("AuthSettings:ClientId");

                    options.LoginPath = "/api/Security/Login";
                    options.LogoutPath = "/api/Security/Logout";

                    options.ExpireTimeSpan = TimeSpan.FromMinutes(expireMinutes);
                    options.SlidingExpiration = true;

                    // https://hajekj.net/2017/03/20/cookie-size-and-cookie-authentication-in-asp-net-core/
                    options.SessionStore = new MemoryCacheTicketStore(expireMinutes);

                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.Name = $".WisWebPanel.{clientId}.Cookies";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                })
                .AddWISOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthSettings:Authority");
                    options.ClientId = Configuration.GetValue<string>("AuthSettings:ClientId");
                    options.ClientSecret = Configuration.GetValue<string>("AuthSettings:ClientSecret");

                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                   
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    options.UsePkce = true;

                    options.CallbackPath = Configuration.GetValue<string>("AuthSettings:CallbackPath");
                    options.SignedOutCallbackPath = Configuration.GetValue<string>("AuthSettings:SignedOutCallbackPath");
                    options.SignedOutRedirectUri = "/";

                    options.Scope.Add("web");
                    options.Scope.Add("api");

                    options.Events.OnRedirectToIdentityProvider = context =>
                    {
                        if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                        {
                            var codeVerifier = CryptoRandom.CreateUniqueId(32);

                            context.Properties.Items["code_verifier"] = codeVerifier;

                            string codeChallenge;
                            using (var sha256 = SHA256.Create())
                            {
                                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                                codeChallenge = Base64Url.Encode(challengeBytes);
                            }

                            context.ProtocolMessage.Parameters["code_challenge"] = codeChallenge;
                            context.ProtocolMessage.Parameters["code_challenge_method"] = "S256";
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnAuthorizationCodeReceived = context =>
                    {
                        if (context.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode)
                        {
                            if (context.Properties.Items.TryGetValue("code_verifier", out var codeVerifier))
                            {
                                context.TokenEndpointRequest.Parameters["code_verifier"] = codeVerifier;
                            }
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedOutCallbackRedirect += context =>
                    {
                        context.Response.Redirect(context.Options.SignedOutRedirectUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    };

                    options.Events.OnRemoteFailure = context =>
                    {
                        this._logger.Error(context.Failure, "Error autenticacion");

                        return Task.CompletedTask;
                    };

                    options.Events.OnAuthenticationFailed = context =>
                    {
                        this._logger.Error(context.Exception, "Error autenticacion");

                        context.Response.Redirect("/api/Security/Logout");
                        context.HandleResponse();

                        return Task.CompletedTask;
                    };

                    options.SecurityTokenValidator = new JwtSecurityTokenHandler
                    {
                        // Disable the built-in JWT claims mapping feature.
                        InboundClaimTypeMap = new Dictionary<string, string>()
                    };
                });

            services.AddAuthorization();

            services.AddTransient<IModuleEndpointProvider, ModuleEndpointProvider>();
            services.AddTransient<IModuleEndpointResolver, ModuleEndpointResolver>();
            services.AddTransient<IMenuItemParser, MenuItemParser>();
            services.AddTransient<IMenuItemProvider, MenuItemProvider>();

            services.AddTransient<ITrafficOfficerService, TrafficOfficerService>();
            services.AddScoped<ITrafficOfficerFrontendService, TrafficOfficerFrontendService>();

            services.AddScoped<IGridCallService, GridCallService>();
            services.AddScoped<IFormCallService, FormCallService>();
            services.AddScoped<IPageCallService, PageCallService>();

            services.AddScoped<IGridService, GridService>();
            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IPageService, PageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseLocalization();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                .RequireAuthorization(); 
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
