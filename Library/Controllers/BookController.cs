using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BookController : Controller
{
    private readonly LibraryContext _context;

    private int pageSize = 10;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    // GET: BOOKS
    // GET: BOOKS
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 10,
        int? mid = null)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var booksQuery = _context.Books
            .Include(b => b.Category)
            .Where(b => b.DeletedAt == null);

        if (mid.HasValue)
        {
            booksQuery = booksQuery
                .Where(b => b.CategoryId == mid.Value);
        }

        booksQuery = booksQuery
            .OrderByDescending(b => b.Id);

        var totalItems = await booksQuery.CountAsync();

        var totalPages = (int)Math.Ceiling(
            (double)totalItems / pageSize
        );

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var books = await booksQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        ViewBag.mid = mid;

        return View(books);
    }

    // GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: BOOKS/Create
    public IActionResult Create()
    {
        ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.DeletedAt == default(DateTime)), "Id", "Name");
        return View();
    }

    // POST: BOOKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            if (book.CoverImage != null && book.CoverImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() +
                               Path.GetExtension(book.CoverImage.FileName);

                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "books");

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await book.CoverImage.CopyToAsync(stream);
                }

                book.CoverImagePath = "/images/books/" + fileName;
            }

            book.CreatedAt = DateTime.Now;
            book.UpdatedAt = DateTime.Now;
            book.DeletedAt = default(DateTime);

            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: BOOKS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.DeletedAt == default(DateTime)), "Id", "Name");
        return View(book);
    }

    // POST: BOOKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (book.CoverImage != null && book.CoverImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(book.CoverImage.FileName);

                    var folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "books");

                    Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.CoverImage.CopyToAsync(stream);
                    }

                    book.CoverImagePath = "/images/books/" + fileName;
                }

                book.UpdatedAt = DateTime.Now;

                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
        return View(book);
    }

    // GET: BOOKS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: BOOKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        book.DeletedAt = DateTime.Now;
        book.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
