namespace BookShoppingCartMvcUI.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> GetUserOrders();
        Task<IEnumerable<Order>> GetAllOrders();
        Task<bool> UpdateOrderStatus(UpdateOrderStatusModel model);
        Task<bool> DeleteOrder(int orderId);
        Task<bool> TogglePaymentStatus(int orderId);
    }
}
