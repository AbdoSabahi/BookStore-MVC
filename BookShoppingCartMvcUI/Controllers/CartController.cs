using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers
{

    [Authorize]
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICartRepository _cartRepo;

        public CartController(IHttpContextAccessor contextAccessor, ICartRepository cartRepo)
        {
            _contextAccessor = contextAccessor;
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddItem(int bookId, int qty = 1, int redirect = 0)
        {
            var totalCartItems = await _cartRepo.AddItem(bookId, qty);

            if (redirect == 1)
            {
                return RedirectToAction(nameof(GetUserCart));
            }

            return Ok(totalCartItems); 
        }
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var totalCartItems = await _cartRepo.RemoveItem(bookId, 1);

            return RedirectToAction(nameof(GetUserCart));
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetUserItemCount();
            return Ok(cartItem);
        }



        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }
[HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
           
            
                var orderId = await _cartRepo.DoCheckout(model);
                return RedirectToAction(nameof(OrderSuccess), new { id = orderId });
           
        }

        public IActionResult OrderSuccess(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}
