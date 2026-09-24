namespace BookShoppingCartMvcUI.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<bool> RemoveItem(int bookId, int qty);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetUserItemCount(string userId = "");
        Task<int> DoCheckout(CheckoutModel model);

    }
}
