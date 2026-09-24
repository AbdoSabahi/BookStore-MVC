using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMvcUI.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _db;

        public GenreRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<Genre?> GetGenreById(int id)
        {
            return await _db.Genres.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AddGenre(Genre genre)
        {
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGenre(Genre genre)
        {
            var existingGenre = await _db.Genres.FirstOrDefaultAsync(a => a.Id == genre.Id);
            if (existingGenre is null)
            {
                return false;
            }

            existingGenre.GenreName = genre.GenreName;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGenre(int id)
        {
            var genre = await _db.Genres.FirstOrDefaultAsync(a => a.Id == id);
            if (genre is null)
            {
                return false;
            }

            _db.Genres.Remove(genre);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}