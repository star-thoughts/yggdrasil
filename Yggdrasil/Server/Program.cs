using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Identity;
using Yggdrasil.Server.Configuration;
using Yggdrasil.Server.Identity;
using Yggdrasil.Server.Storage;

namespace Yggdrasil.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            ICampaignStorage storage = host.Services.GetService<ICampaignStorage>();
            await storage.Connect();

            await SetupIdentity(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel();
                });

        private static async Task SetupIdentity(IHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                string[] existingRoles = roleManager.Roles.Select(p => p.Name).ToArray();
                string[] allRoles = Roles.GetAllRoles().ToArray();
                string[] rolesToAdd = allRoles.Except(existingRoles).ToArray();

                //  Add any new roles to the system
                foreach (string role in rolesToAdd)
                    await roleManager.CreateAsync(new ApplicationRole() { Name = role });

                IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                IdentityConfiguration identityDefaults = new IdentityConfiguration();
                config.Bind("Identity", identityDefaults);

                if (!userManager.Users.Any())
                {
                    if (string.IsNullOrWhiteSpace(identityDefaults.AdminAccount)
                        || string.IsNullOrWhiteSpace(identityDefaults.AdminPassword))
                        throw new InvalidOperationException("Cannot start service without users defined.");

                    IdentityResult createResult = await userManager.CreateAsync(new ApplicationUser() { UserName = identityDefaults.AdminAccount, IsVerified = true, IsSiteAdmin = true }, identityDefaults.AdminPassword);
                    if (!createResult.Succeeded)
                        throw new InvalidOperationException("Could not create default admin account");
                }

                ApplicationUser admin = userManager.Users.FirstOrDefault(p => p.UserName == identityDefaults.AdminAccount);
                IEnumerable<string> roles = await userManager.GetRolesAsync(admin);
                roles = Roles.GetDefaultAdminRoles().Except(roles);

                await userManager.AddToRolesAsync(admin, roles);
            }
        }
    }
}
