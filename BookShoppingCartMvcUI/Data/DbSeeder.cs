using Microsoft.AspNetCore.Identity;
using BookShoppingCartMvcUI.Constants;

namespace BookShoppingCartMvcUI.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            // 1. جلب الخدمات المطلوبة
            var userMgr = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

            // 2. إنشاء الأدوار الأساسية (ضفنا SuperAdmin)
            string[] roles = { Roles.SuperAdmin, Roles.Admin, Roles.User };

            foreach (var role in roles)
            {
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    await roleMgr.CreateAsync(new IdentityRole(role));
                }
            }

            // 3. إنشاء مستخدم الأدمن الافتراضي
            var adminEmail = "admin@domain.com";
            var adminUser = await userMgr.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userMgr.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // غيرناها من Admin لـ SuperAdmin
                    await userMgr.AddToRoleAsync(adminUser, Roles.SuperAdmin);
                }
            }
        }
    }
}