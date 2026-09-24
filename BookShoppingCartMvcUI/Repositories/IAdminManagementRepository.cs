namespace BookShoppingCartMvcUI.Repositories
{
    public interface IAdminManagementRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAdmins();
        Task<(bool Success, string Message)> MakeAdmin(string email);
        Task<(bool Success, string Message)> RemoveAdmin(string email);
    }
}