using Microsoft.EntityFrameworkCore;
namespace BookShoppingCartMvcUI.Repositories
{
    public class HomeRepositry : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepositry(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            var books = await (from book in _db.Books
                               join genre in _db.Genres
                               on book.GenreId equals genre.Id
                               join stock in _db.Stocks
                               on book.Id equals stock.BookId into stockJoin
                               from stock in stockJoin.DefaultIfEmpty()
                               where (string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().StartsWith(sTerm))
                                     && (genreId == 0 || book.GenreId == genreId)
                               select new Book
                               {
                                   Id = book.Id,
                                   Image = book.Image,
                                   AuthorName = book.AuthorName,
                                   BookName = book.BookName,
                                   GenreId = book.GenreId,
                                   Price = book.Price,
                                   GenreName = genre.GenreName,
                                   Quantity = stock != null ? stock.Quantity : 0
                               }).ToListAsync();
            if (genreId > 0)
            {
                books = books.Where(a => a.GenreId == genreId).ToList();
            }

            return books;
        }

        public async Task<IEnumerable<Genre>> Genures()
        {
            return await _db.Genres.ToListAsync();
        }
    }
}