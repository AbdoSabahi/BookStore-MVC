namespace BookShoppingCartMvcUI.Repositories
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetGenres();
        Task<Genre?> GetGenreById(int id);
        Task<bool> AddGenre(Genre genre);
        Task<bool> UpdateGenre(Genre genre);
        Task<bool> DeleteGenre(int id);
    }
}
