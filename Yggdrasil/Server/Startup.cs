using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Yggdrasil.Server.Configuration;
using Yggdrasil.Server.Hubs;
using Yggdrasil.Server.Identity;
using Yggdrasil.Server.MiddleWare;
using Yggdrasil.Server.Storage;
using Yggdrasil.Server.Storage.Mongo;

namespace Yggdrasil.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(configure =>
            {
                configure.CacheProfiles.Add("Default", new CacheProfile() { NoStore = true, Location = ResponseCacheLocation.None });
            });
            services.AddRazorPages();
            services.AddSignalR();

            ConfigureIdentity(services);
            ConfigureStorage(services);
        }

        private void ConfigureStorage(IServiceCollection services)
        {
            services.Configure<StorageConfiguration>(Configuration.GetSection("Storage"));
            services.AddSingleton<ICampaignStorage, MongoDbCampaignStorage>();
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            IdentityConfiguration config = new IdentityConfiguration();
            Configuration.Bind("Identity", config);

            JwtConfiguration jwtConfig = new JwtConfiguration();
            Configuration.Bind("Jwt", jwtConfig);

            if (jwtConfig.Key == null)
                throw new InvalidOperationException("A JWT Key must be specified.");

            services.Configure<JwtConfiguration>(Configuration.GetSection("Jwt"));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Issuer,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            StringValues accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            PathString path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 6;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = true;
                identityOptions.Password.RequireDigit = true;
                identityOptions.Lockout.AllowedForNewUsers = true;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                identityOptions.Lockout.MaxFailedAccessAttempts = 5;
            },
            databaseOptions =>
            {
                databaseOptions.ConnectionString = config.MongoDB?.ConnectionString;
                databaseOptions.MigrationCollection = "yggdrasil_migration";
                databaseOptions.RolesCollection = "yggdrasil_roles";
                databaseOptions.UsersCollection = "yggdrasil_users";
            });
            services.AddSingleton(config);

            //  This seems to be required to prevent ASP.NET Core from redirecting to another page when a 401 is returned
            services.ConfigureApplicationCookie(p =>
            {
                p.Events.OnRedirectToAccessDenied =
                p.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCustomExceptionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<AdminHub>("/hub/admin");
                endpoints.MapHub<ServiceHub>("/hub/service");
                endpoints.Map("api/{**slug}", HandleApi404);
                endpoints.Map("hub/{**slug}", HandleApi404);
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private Task HandleApi404(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }
    }
}
