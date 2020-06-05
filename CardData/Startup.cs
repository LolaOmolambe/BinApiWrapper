using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using CardData.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CardData
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddScoped<ICardDataLogic, CardDataLogic>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = "Okta";
            })
               .AddOAuth("Okta", options =>
               {
                   var oktaDomain = Configuration.GetValue<string>("Okta:OktaDomain");

                   options.AuthorizationEndpoint = $"{oktaDomain}/oauth2/default/v1/authorize";

                   options.Scope.Add("openid");
                   options.Scope.Add("profile");
                   options.Scope.Add("email");

                   options.CallbackPath = new PathString("/authorization-code/callback");

                   options.ClientId = Configuration.GetValue<string>("Okta:ClientId");
                   options.ClientSecret = Configuration.GetValue<string>("Okta:ClientSecret");
                   options.TokenEndpoint = $"{oktaDomain}/oauth2/default/v1/token";

                   options.UserInformationEndpoint = $"{oktaDomain}/oauth2/default/v1/userinfo";

                   options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                   options.ClaimActions.MapJsonKey(ClaimTypes.Name, "given_name");
                   options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                   options.Events = new OAuthEvents
                   {
                       OnCreatingTicket = async context =>
                       {

                           var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);

                           request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                           request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                           var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                           response.EnsureSuccessStatusCode();

                           JsonElement user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
                           context.RunClaimActions(user);

                          
                       }
                   };
               });
            services.AddControllers();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
