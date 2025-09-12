using KMSI_Projects.Data;
using KMSI_Projects.Models;
using KMSI_Projects.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KMSI_Projects.Controllers
{
    /// <summary>
    /// Controller For Master Books
    /// Manages book definitions within companies - follows same pattern as GradeController
    /// </summary>
    [Authorize]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookController> _logger;

        public BookController(ApplicationDbContext context, ILogger<BookController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Get books based on user's access level (same pattern as Grade)
                var books = await _context.Books
                    .Include(b => b.Company)
                    .Where(b => b.CompanyId == currentUser.CompanyId || b.Company.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(b => b.BookCode)
                    .ThenBy(b => b.BookTitle)
                    .ToListAsync();

                ViewBag.CurrentCompanyId = currentUser.CompanyId;
                return View(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading books");
                TempData["ErrorMessage"] = "An error occurred while loading books.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Book ID is required." });
            }

            try
            {
                var book = await _context.Books
                    .Include(b => b.Company)
                    .Include(b => b.GradeBooks).ThenInclude(gb => gb.Grade)
                    .Include(b => b.Inventories).ThenInclude(i => i.Site)
                    .FirstOrDefaultAsync(m => m.BookId == id);

                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Check access permission (same pattern as Grade)
                if (currentUser == null ||
                    (book.CompanyId != currentUser.CompanyId && book.Company.ParentCompanyId != currentUser.CompanyId))
                {
                    return Json(new { success = false, message = "Access denied." });
                }

                var bookViewModel = new BookViewModel
                {
                    BookId = book.BookId,
                    CompanyId = book.CompanyId,
                    BookCode = book.BookCode,
                    BookTitle = book.BookTitle,
                    Author = book.Author,
                    Publisher = book.Publisher,
                    ISBN = book.ISBN,
                    Category = book.Category,
                    Description = book.Description,
                    IsActive = book.IsActive,
                    CompanyName = book.Company.CompanyName,
                    CompanyCode = book.Company.CompanyCode,
                    CreatedDate = book.CreatedDate,
                    UpdatedDate = book.UpdatedDate,
                    GradesCount = book.GradeBooks?.Count ?? 0,
                    StockQuantity = book.Inventories?.Sum(i => i.CurrentStock) ?? 0
                };

                return Json(new { success = true, data = bookViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading book details for ID: {id}");
                return Json(new { success = false, message = "An error occurred while loading book details." });
            }
        }

        // GET: Book/Create
        public async Task<IActionResult> GetCreateForm()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Get companies that user can access (same pattern as Grade)
                var companies = await _context.Companies
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(c => c.CompanyName)
                    .Select(c => new { c.CompanyId, c.CompanyName, c.CompanyCode })
                    .ToListAsync();

                var model = new BookViewModel
                {
                    CompanyId = currentUser.CompanyId,
                    IsActive = true
                };

                return Json(new { success = true, model = model, companies = companies });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create book form");
                return Json(new { success = false, message = "An error occurred while loading the form." });
            }
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] BookViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Check if book code already exists within the same company (same pattern as Grade)
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.BookCode == model.BookCode && b.CompanyId == model.CompanyId);

                    if (existingBook != null)
                    {
                        return Json(new { success = false, message = "Book code already exists in this company." });
                    }

                    var book = new Book
                    {
                        CompanyId = model.CompanyId,
                        BookCode = model.BookCode.ToUpper(),
                        BookTitle = model.BookTitle,
                        Author = model.Author,
                        Publisher = model.Publisher,
                        ISBN = model.ISBN,
                        Category = model.Category,
                        Description = model.Description,
                        IsActive = model.IsActive,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Book {book.BookTitle} created by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Book created successfully.",
                        data = new
                        {
                            bookId = book.BookId,
                            bookCode = book.BookCode,
                            bookTitle = book.BookTitle
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return Json(new { success = false, message = "An error occurred while creating the book." });
            }
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> GetEditForm(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Book ID is required." });
            }

            try
            {
                var book = await _context.Books
                    .Include(b => b.Company)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Check access permission (same pattern as Grade)
                if (currentUser == null ||
                    (book.CompanyId != currentUser.CompanyId && book.Company.ParentCompanyId != currentUser.CompanyId))
                {
                    return Json(new { success = false, message = "Access denied." });
                }

                // Get companies that user can access
                var companies = await _context.Companies
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(c => c.CompanyName)
                    .Select(c => new { c.CompanyId, c.CompanyName, c.CompanyCode })
                    .ToListAsync();

                var model = new BookViewModel
                {
                    BookId = book.BookId,
                    CompanyId = book.CompanyId,
                    BookCode = book.BookCode,
                    BookTitle = book.BookTitle,
                    Author = book.Author,
                    Publisher = book.Publisher,
                    ISBN = book.ISBN,
                    Category = book.Category,
                    Description = book.Description,
                    IsActive = book.IsActive
                };

                return Json(new { success = true, model = model, companies = companies });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading edit book form for ID: {id}");
                return Json(new { success = false, message = "An error occurred while loading the form." });
            }
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] BookViewModel model)
        {
            if (id != model.BookId)
            {
                return Json(new { success = false, message = "Book ID mismatch." });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    var book = await _context.Books.FindAsync(id);
                    if (book == null)
                    {
                        return Json(new { success = false, message = "Book not found." });
                    }

                    // Check if book code already exists within the same company (excluding current book)
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.BookCode == model.BookCode && b.CompanyId == model.CompanyId && b.BookId != id);

                    if (existingBook != null)
                    {
                        return Json(new { success = false, message = "Book code already exists in this company." });
                    }

                    book.CompanyId = model.CompanyId;
                    book.BookCode = model.BookCode.ToUpper();
                    book.BookTitle = model.BookTitle;
                    book.Author = model.Author;
                    book.Publisher = model.Publisher;
                    book.ISBN = model.ISBN;
                    book.Category = model.Category;
                    book.Description = model.Description;
                    book.IsActive = model.IsActive;
                    book.UpdatedBy = currentUserId;
                    book.UpdatedDate = DateTime.Now;

                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Book {book.BookTitle} updated by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Book updated successfully.",
                        data = new
                        {
                            bookId = book.BookId,
                            bookCode = book.BookCode,
                            bookTitle = book.BookTitle
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book ID: {id}");
                return Json(new { success = false, message = "An error occurred while updating the book." });
            }
        }

        // POST: Book/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Check if book is being used in GradeBooks or Inventory
                var isUsed = await _context.GradeBooks.AnyAsync(gb => gb.BookId == id) ||
                           await _context.Inventories.AnyAsync(i => i.BookId == id);

                if (isUsed)
                {
                    // Soft delete if book is being used
                    book.IsActive = false;
                    book.UpdatedBy = currentUserId;
                    book.UpdatedDate = DateTime.Now;

                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Book {book.BookTitle} deactivated by user {currentUserId}");

                    return Json(new { success = true, message = "Book deactivated successfully (was in use)." });
                }
                else
                {
                    // Hard delete if not used
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Book {book.BookTitle} deleted by user {currentUserId}");

                    return Json(new { success = true, message = "Book deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book ID: {id}");
                return Json(new { success = false, message = "An error occurred while deleting the book." });
            }
        }

        // POST: Book/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                book.IsActive = !book.IsActive;
                book.UpdatedBy = currentUserId;
                book.UpdatedDate = DateTime.Now;

                _context.Update(book);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Book {book.BookTitle} status toggled to {(book.IsActive ? "Active" : "Inactive")} by user {currentUserId}");

                return Json(new
                {
                    success = true,
                    message = $"Book status changed to {(book.IsActive ? "Active" : "Inactive")}.",
                    isActive = book.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling book status ID: {id}");
                return Json(new { success = false, message = "An error occurred while updating book status." });
            }
        }
    }
}