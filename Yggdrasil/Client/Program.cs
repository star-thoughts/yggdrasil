using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Yggdrasil.Client.Identity;
using Yggdrasil.Client.Services;
using Fiction.Controls.Toast;

namespace Yggdrasil.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("PublicAPI", (provider, client) =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                string token = ApiAuthenticationStateProvider.JwtToken;
                if (!string.IsNullOrWhiteSpace(token) && !string.Equals(token, "null", StringComparison.OrdinalIgnoreCase))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            });

            builder.Services.AddToastNotifications();

            builder.Services.AddScoped<ICampaignService, CampaignService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            await builder.Build().RunAsync();
        }
    }
}
