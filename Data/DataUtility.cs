using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PenSword.Models;

namespace PenSword.Data
{
    public static class DataUtility
    {
        private const string? _adminRole = "Admin";
        private const string? _modRole = "Moderator";
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // Obtaining the necessary services based on the IServiceProvider parameter
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Align the database by checking Migrations
            await dbContextSvc.Database.MigrateAsync();

            // Seed app roles
            await SeedRolesAsync(roleManagerSvc);

            // Seed User(s)
            await SeedBlogUsersAsync(userManagerSvc, configurationSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManagerSvc)
        {
            if(!await roleManagerSvc.RoleExistsAsync(_adminRole!))
            {
                await roleManagerSvc.CreateAsync(new IdentityRole(_adminRole!));
            }
            
            if(!await roleManagerSvc.RoleExistsAsync(_modRole!))
            {
                await roleManagerSvc.CreateAsync(new IdentityRole(_modRole!));
            }

        }
        
        private static async Task SeedBlogUsersAsync(UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            string? adminEmail = configuration["AdminLoginEmail"] 
                ?? Environment.GetEnvironmentVariable("AdminLoginEmail");
            string? adminPwd = configuration["AdminPwd"] 
                ?? Environment.GetEnvironmentVariable("AdminPwd");
            
            string? modEmail = configuration["ModLoginEmail"] 
                ?? Environment.GetEnvironmentVariable("ModLoginEmail");
            string? modPwd = configuration["ModPwd"] 
                ?? Environment.GetEnvironmentVariable("ModPwd");

            // seed admin and assign role
            try
            {
                BlogUser adminUser = new()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Cadence Eva",
                    LastName = "Custin",
                    EmailConfirmed = true,
                };

                BlogUser? user = await userManager.FindByEmailAsync(adminEmail!);
                if (user is null)
                {
                    await userManager.CreateAsync(adminUser, adminPwd!);
                    await userManager.AddToRoleAsync(adminUser, _adminRole!);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******* ERROR *******");
                Console.WriteLine("Error Seeding Admin User");
                Console.WriteLine(ex.Message);
                Console.WriteLine("*********************");
            };

            // seed mod and assign role
            try
            {
                BlogUser modUser = new()
                {
                    UserName = modEmail,
                    Email = modEmail,
                    FirstName = "Antonio",
                    LastName = "Raynor",
                    EmailConfirmed = true,
                };

                BlogUser? user = await userManager.FindByEmailAsync(modEmail!);
                if (user is null)
                {
                    await userManager.CreateAsync(modUser, adminPwd!);
                    await userManager.AddToRoleAsync(modUser, _modRole!);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******* ERROR *******");
                Console.WriteLine("Error Seeding Mod User");
                Console.WriteLine(ex.Message);
                Console.WriteLine("*********************");
            };
        }
    }
}
