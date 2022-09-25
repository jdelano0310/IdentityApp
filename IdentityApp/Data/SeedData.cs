using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Authorization;

namespace IdentityApp.Data
{
    public class SeedData
    {
        //need an initialize method
        public static async Task Initialize(IServiceProvider serviceProvider, string password)
        {
            // get database context - can't use dependancy injectoion this is why GetRequiredService is used
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // add an invocie manager
                var managerUid = await EnsureUser(serviceProvider, "manager@test.com", password);
                await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagerRole);

                // add an invoice administrator
                var adminUid = await EnsureUser(serviceProvider, "admin@test.com", password);
                await EnsureRole(serviceProvider, adminUid, Constants.InvoiceAdministratorRole);

            }
        }

        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            string userName, string initPw)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, initPw);
            }

            if (user == null)
            {
                throw new Exception("User did not get created. Password issue maybe?");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(
            IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            IdentityResult ir;
            if (!await roleManager.RoleExistsAsync(role))
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }
    }
}
