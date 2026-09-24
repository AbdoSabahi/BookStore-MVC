using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Constants;
using BookShoppingCartMvcUI.Repositories;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminManagementController : Controller
    {
        private readonly IAdminManagementRepository _adminRepo;

        public AdminManagementController(IAdminManagementRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _adminRepo.GetAllAdmins();
            return View(admins);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string email)
        {
            var (success, message) = await _adminRepo.MakeAdmin(email);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveAdmin(string email)
        {
            var (success, message) = await _adminRepo.RemoveAdmin(email);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}