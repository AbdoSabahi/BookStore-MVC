using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMvcUI.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooks();
        Task<Book?> GetBookById(int id);
        Task<bool> AddBook(Book book);
        Task<bool> UpdateBook(Book book);
        Task<bool> DeleteBook(int id);
        Task<IEnumerable<Book>> GetTopSellingBooks(int count = 5);
    }
}