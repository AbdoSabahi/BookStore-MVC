using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly IGenreRepository _genreRepo;

        public BookController(IBookRepository bookRepo, IGenreRepository genreRepo)
        {
            _bookRepo = bookRepo;
            _genreRepo = genreRepo;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookRepo.GetBooks();
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> AddBook()
        {
            await PopulateGenres();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = string.Join(" | ", errors);
                await PopulateGenres();
                return View(book);
            }

            if (imageFile != null)
            {
                book.Image = await SaveImage(imageFile);
            }

            var success = await _bookRepo.AddBook(book);
            TempData["Success"] = success ? "Book added successfully." : "Failed to add book.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookRepo.GetBookById(id);
            if (book is null)
            {
                return NotFound();
            }

            book.Quantity = book.Stock?.Quantity ?? 0;
            await PopulateGenres();
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = string.Join(" | ", errors);
                await PopulateGenres();
                return View(book);
            }

            if (imageFile != null)
            {
                book.Image = await SaveImage(imageFile);
            }

            var success = await _bookRepo.UpdateBook(book);
            TempData["Success"] = success ? "Book updated successfully." : "Failed to update book.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _bookRepo.DeleteBook(id);
            TempData["Success"] = success ? "Book deleted successfully." : "Failed to delete book.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateGenres()
        {
            var genres = await _genreRepo.GetGenres();
            ViewBag.Genres = genres.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName
            });
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }
        public async Task<IActionResult> TopSelling()
        {
            var books = await _bookRepo.GetTopSellingBooks();
            return View(books);
        }
    }
}