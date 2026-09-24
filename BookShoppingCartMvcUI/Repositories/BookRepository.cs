using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMvcUI.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _db.Books
                .Include(a => a.Genre)
                .Include(a => a.Stock)
                .ToListAsync();
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _db.Books
                .Include(a => a.Genre)
                .Include(a => a.Stock)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AddBook(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            var stock = new Stock
            {
                BookId = book.Id,
                Quantity = book.Quantity
            };
            _db.Stocks.Add(stock);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBook(Book book)
        {
            var existingBook = await _db.Books.FirstOrDefaultAsync(a => a.Id == book.Id);
            if (existingBook is null)
            {
                return false;
            }

            existingBook.BookName = book.BookName;
            existingBook.AuthorName = book.AuthorName;
            existingBook.Price = book.Price;
            existingBook.GenreId = book.GenreId;

            if (!string.IsNullOrEmpty(book.Image))
            {
                existingBook.Image = book.Image;
            }

            await _db.SaveChangesAsync();

            // تحديث الكمية في جدول Stock، أو إنشاء صف جديد لو مش موجود
            var stock = await _db.Stocks.FirstOrDefaultAsync(a => a.BookId == book.Id);
            if (stock != null)
            {
                stock.Quantity = book.Quantity;
            }
            else
            {
                stock = new Stock
                {
                    BookId = book.Id,
                    Quantity = book.Quantity
                };
                _db.Stocks.Add(stock);
            }
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBook(int id)
        {
            var book = await _db.Books
                .Include(a => a.Stock)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (book is null)
            {
                return false;
            }

            if (book.Stock != null)
            {
                _db.Stocks.Remove(book.Stock);
            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<Book>> GetTopSellingBooks(int count = 5)
        {
            var topBooks = await _db.OrderDetails
                .GroupBy(od => od.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(count)
                .ToListAsync();

            var bookIds = topBooks.Select(x => x.BookId).ToList();

            var books = await _db.Books
                .Include(a => a.Genre)
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            var orderedBooks = bookIds
                .Select(id => books.First(b => b.Id == id))
                .ToList();

            return orderedBooks;
        }
    }
}