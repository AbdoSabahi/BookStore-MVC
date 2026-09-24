using Microsoft.AspNetCore.Identity;
using BookShoppingCartMvcUI.Constants;

namespace BookShoppingCartMvcUI.Repositories
{
    public class AdminManagementRepository : IAdminManagementRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminManagementRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAdmins()
        {
            return await _userManager.GetUsersInRoleAsync(Roles.Admin);
        }

        public async Task<(bool Success, string Message)> MakeAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return (false, "No user found with this email.");
            }

            var isAlreadyAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            if (isAlreadyAdmin)
            {
                return (false, "This user is already an admin.");
            }

            var result = await _userManager.AddToRoleAsync(user, Roles.Admin);
            return result.Succeeded
                ? (true, "User promoted to Admin successfully.")
                : (false, "Failed to promote user.");
        }

        public async Task<(bool Success, string Message)> RemoveAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return (false, "No user found with this email.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, Roles.Admin);
            return result.Succeeded
                ? (true, "Admin role removed successfully.")
                : (false, "Failed to remove admin role.");
        }
    }
}