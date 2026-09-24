using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRebo;

        public UserOrderController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRebo = userOrderRepository;
        }

        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRebo.GetUserOrders();
            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Orders()
        {
            var orders = await _userOrderRebo.GetAllOrders();
            return View(orders);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult UpdateOrderStatus(int orderId)
        {
            var model = new UpdateOrderStatusModel
            {
                OrderId = orderId,
                OrderStatusId = 1
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            var success = await _userOrderRebo.UpdateOrderStatus(model);
            TempData["Success"] = success ? "Order status updated." : "Failed to update.";
            return RedirectToAction(nameof(Orders));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var success = await _userOrderRebo.DeleteOrder(orderId);
            TempData["Success"] = success ? "Order deleted successfully." : "Failed to delete the order.";
            return RedirectToAction(nameof(Orders));
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            var success = await _userOrderRebo.TogglePaymentStatus(orderId);
            TempData["Success"] = success ? "Payment status updated." : "Failed to update.";
            return RedirectToAction(nameof(Orders));
        }
    }
}