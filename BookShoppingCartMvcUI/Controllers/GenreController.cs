using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepo;

        public GenreController(IGenreRepository genreRepo)
        {
            _genreRepo = genreRepo;
        }

        // عرض كل الأنواع
        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepo.GetGenres();
            return View(genres);
        }

        // عرض فورم إضافة نوع جديد
        [HttpGet]
        public IActionResult AddGenre()
        {
            return View();
        }

        // استقبال الإضافة
        [HttpPost]
        public async Task<IActionResult> AddGenre(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                // ده هيوريك رسالة توضح ليه الـ Validation فشلت
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = string.Join(" | ", errors);
                return View(genre);
            }

            var success = await _genreRepo.AddGenre(genre);
            TempData["Success"] = success ? "Genre added successfully." : "Failed to add genre.";
            return RedirectToAction(nameof(Index));   // ← ده اللي هيوديك لصفحة Index بعد النجاح
        }
        // عرض فورم التعديل
        [HttpGet]
        public async Task<IActionResult> EditGenre(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre is null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // استقبال التعديل
        [HttpPost]
        public async Task<IActionResult> EditGenre(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }

            var success = await _genreRepo.UpdateGenre(genre);
            TempData["Success"] = success ? "Genre updated successfully." : "Failed to update genre.";
            return RedirectToAction(nameof(Index));
        }
        // حذف نوع
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var success = await _genreRepo.DeleteGenre(id);
            TempData["Success"] = success ? "Genre deleted successfully." : "Failed to delete genre.";
            return RedirectToAction(nameof(Index));
        }
    }
}