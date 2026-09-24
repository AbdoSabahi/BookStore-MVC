using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;               
using BookShoppingCartMvcUI.Models; // Add this line for the ShoppingCart and CartItem classes

namespace BookShoppingCartMvcUI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<ShoppingCart> GetUserCart()
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User is not logged-in");
            }

          
            var cart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Book)
                .ThenInclude(a => a.Genre)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();

            return cart;
        }
        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();

            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");

                var cart = await GetCart(userId);

                if (cart is null)
                {
                    cart = new ShoppingCart { UserId = userId };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();

                var cartItem = _db.CartDetails
                    .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }

                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }

            var cartItemCount = await GetUserItemCount(userId); // ← الاسم اتصلح هنا
            return cartItemCount;
        }

        public async Task<bool> RemoveItem(int bookId, int qty)
        {
           

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                string userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    return false;
                }

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem.Quantity > qty)
                {
                    cartItem.Quantity -= qty; 
                }
                else
                {
                    _db.CartDetails.Remove(cartItem); 
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<ShoppingCart> GetCart(string userId)
        {
           var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(a => a.UserId == userId);
            return cart;
        }
        private string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;
           string userId = _userManager.GetUserId(user);
           return userId;
        }
        public async Task<int> GetUserItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }

            var data = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();

            if (data is null)
            {
                return 0;
            }

            int count = data.CartDetails.Sum(item => item.Quantity);
            return count;
        }


        public async Task<int> DoCheckout(CheckoutModel model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = GetUserId();
                if (String.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not logged-in");
                }

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new Exception("Cart not found for the user.");
                }

                var cartItems = await _db.CartDetails
                    .Include(a => a.Book)
                    .Where(a => a.ShoppingCartId == cart.Id)
                    .ToListAsync();

                if (cartItems.Count == 0)
                {
                    throw new Exception("Cart is empty.");
                }

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = 1,
                    //TotalAmount = cartItems.Sum(item => item.Book.Price * item.Quantity)
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    Address = model.Address,
                    PaymentMethod = model.PaymentMethod,
                    IsPaid = false
                };
                
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Book.Price,
                      

                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.CartDetails.RemoveRange(cartItems);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;


            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
