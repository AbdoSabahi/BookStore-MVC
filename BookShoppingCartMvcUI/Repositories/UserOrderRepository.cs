using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserOrderRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Order>> GetUserOrders()
        {
            var userId = await GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User is not logged-in");
            }

            var orders = await _db.Orders
                .Include(a => a.OrderDetail)
                .ThenInclude(x => x.Book)
                .ThenInclude(s => s.Genre)
                .Include(a => a.OrderStatus)
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return orders;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var orders = await _db.Orders
                .Include(a => a.OrderDetail)
                .ThenInclude(x => x.Book)
                .ThenInclude(s => s.Genre)
                .Include(a => a.OrderStatus)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();

            return orders;
        }

        public async Task<bool> UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(a => a.Id == model.OrderId);
            if (order is null)
            {
                return false;
            }

            order.OrderStatusId = model.OrderStatusId;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOrder(int orderId)
        {
            var order = await _db.Orders
                .Include(a => a.OrderDetail)
                .FirstOrDefaultAsync(a => a.Id == orderId);

            if (order is null)
            {
                return false;
            }

            _db.OrderDetails.RemoveRange(order.OrderDetail);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<string> GetUserId()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user?.Id;
        }

        public async Task<bool> TogglePaymentStatus(int orderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(a => a.Id == orderId);
            if (order is null)
            {
                return false;
            }

            order.IsPaid = !order.IsPaid;   // ← ده السطر السحري: يقلب true لـ false والعكس
            await _db.SaveChangesAsync();

            return true;
        }
    }
}