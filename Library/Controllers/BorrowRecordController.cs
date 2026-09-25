
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BorrowRecordController : Controller
{
    private readonly LibraryContext _context;

    public BorrowRecordController(LibraryContext context)
    {
        _context = context;
    }

    // GET: BORROWRECORDS
    public async Task<IActionResult> Index(int borrowPage = 1, int returnedPage = 1, int pageSize = 10)
    {
        if (borrowPage < 1)
            borrowPage = 1;

        if (returnedPage < 1)
            returnedPage = 1;

        if (pageSize < 1)
            pageSize = 10;

        // ==========================================
        // ĐANG MƯỢN + QUÁ HẠN
        // ==========================================

        var borrowingQuery = _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .Where(b =>
                b.Status.ToString() == "BORROWING" ||
                b.Status.ToString() == "OVERDUE")
            .OrderByDescending(b => b.Id);

        var borrowingTotalItems = await borrowingQuery.CountAsync();

        var borrowingTotalPages = (int)Math.Ceiling(
            (double)borrowingTotalItems / pageSize
        );

        if (borrowingTotalPages > 0 && borrowPage > borrowingTotalPages)
        {
            borrowPage = borrowingTotalPages;
        }

        var borrowingRecords = await borrowingQuery
            .Skip((borrowPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        // ==========================================
        // ĐÃ TRẢ
        // ==========================================

        var returnedQuery = _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .Where(b => b.Status.ToString() == "RETURNED")
            .OrderByDescending(b => b.Id);

        var returnedTotalItems = await returnedQuery.CountAsync();

        var returnedTotalPages = (int)Math.Ceiling(
            (double)returnedTotalItems / pageSize
        );

        if (returnedTotalPages > 0 && returnedPage > returnedTotalPages)
        {
            returnedPage = returnedTotalPages;
        }

        var returnedRecords = await returnedQuery
            .Skip((returnedPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        // ==========================================
        // PAGINATION ĐANG MƯỢN
        // ==========================================

        ViewBag.BorrowingPagination = new PaginationViewModel
        {
            CurrentPage = borrowPage,
            PageSize = pageSize,
            TotalItems = borrowingTotalItems,
            PageParameter = "borrowPage",

            RouteValues = new Dictionary<string, string?>
            {
                ["returnedPage"] = returnedPage.ToString()
            }
        };


        // ==========================================
        // PAGINATION ĐÃ TRẢ
        // ==========================================

        ViewBag.ReturnedPagination = new PaginationViewModel
        {
            CurrentPage = returnedPage,
            PageSize = pageSize,
            TotalItems = returnedTotalItems,
            PageParameter = "returnedPage",

            RouteValues = new Dictionary<string, string?>
            {
                ["borrowPage"] = borrowPage.ToString()
            }
        };


        // Truyền 2 danh sách sang View
        ViewBag.BorrowingRecords = borrowingRecords;
        ViewBag.ReturnedRecords = returnedRecords;

        return View();
    }

    // GET: BORROWRECORDS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var borrowrecord = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (borrowrecord == null)
        {
            return NotFound();
        }

        return View(borrowrecord);
    }

    // GET: BORROWRECORDS/Create
    public async Task<IActionResult> Create()
    {
        var members = await _context.Members
            .Where(m => m.DeletedAt == null && m.Status == MemberStatus.ACTIVE)
            .OrderBy(m => m.FullName)
            .Select(m => new
            {
                m.Id,
                m.MemberCode,
                m.FullName,
                m.Email,
                m.Phone,
                m.Address,
                m.Status
            })
            .ToListAsync();

        ViewBag.Members = members;

        var books = await _context.Books
            .Where(b => b.DeletedAt == default(DateTime) || b.DeletedAt == null)
            .OrderBy(b => b.Title)
            .Select(b => new
            {
                b.Id,
                DisplayName = b.Isbn + " - " + b.Title
            })
            .ToListAsync();

        ViewBag.Books = books;

        return View();
    }

    // POST: BORROWRECORDS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    BorrowRecord borrowrecord,
    List<int> BookIds)
    {
        var members = await _context.Members
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .OrderBy(m => m.FullName)
            .Select(m => new
            {
                m.Id,
                m.MemberCode,
                m.FullName,
                m.Email,
                m.Phone,
                m.Address,
                m.Status
            })
            .ToListAsync();

        ViewBag.Members = members;

        var books = await _context.Books
            .Where(b => b.DeletedAt == default(DateTime) || b.DeletedAt == null)
            .OrderBy(b => b.Title)
            .Select(b => new
            {
                b.Id,
                DisplayName = b.Isbn + " - " + b.Title
            })
            .ToListAsync();

        ViewBag.Books = books;

        ModelState.Remove("BookId");

        if (BookIds == null || BookIds.Count == 0)
        {
            ModelState.AddModelError(
                "BookIds",
                "Vui lòng chọn ít nhất một cuốn sách."
            );
        }

        if (ModelState.IsValid)
        {
            var now = DateTime.Now;

            foreach (var bookId in BookIds)
            {
                var borrow = new BorrowRecord
                {
                    MemberId = borrowrecord.MemberId,
                    BookId = bookId,
                    BorrowDate = now,
                    DueDate = borrowrecord.DueDate,
                    ReturnDate = borrowrecord.ReturnDate,
                    Status = borrowrecord.Status,
                    Note = borrowrecord.Note,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _context.BorrowRecords.Add(borrow);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(borrowrecord);
    }

    // GET: BORROWRECORDS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        ViewBag.MemberId = new SelectList(
            await _context.Members
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .ToListAsync(),
            "Id", "FullName"
            );

        ViewBag.BookId = new SelectList(
            await _context.Books
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .ToListAsync(),
            "Id", "Title"
            );

        var borrowrecord = await _context.BorrowRecords.FindAsync(id);
        if (borrowrecord == null)
        {
            return NotFound();
        }
        return View(borrowrecord);
    }

    // POST: BORROWRECORDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,MemberId,BookId,BorrowDate,DueDate,ReturnDate,Status,Note,CreatedAt,UpdatedAt")] BorrowRecord borrowrecord)
    {
        if (id != borrowrecord.Id)
        {
            return NotFound();
        }

        ViewBag.MemberId = new SelectList(
            await _context.Members
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .ToListAsync(),
            "Id", "FullName"
            );

        ViewBag.BookId = new SelectList(
            await _context.Books
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .ToListAsync(),
            "Id", "Title"
            );

        if (ModelState.IsValid)
        {
            try
            {
                borrowrecord.CreatedAt = DateTime.Now;
                borrowrecord.UpdatedAt = DateTime.Now;

                _context.Update(borrowrecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowRecordExists(borrowrecord.Id))
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
        return View(borrowrecord);
    }

    // GET: BORROWRECORDS/Delete/5
    // GET: BORROWRECORDS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var borrowrecord = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (borrowrecord == null)
        {
            return NotFound();
        }

        return View(borrowrecord);
    }

    // POST: BORROWRECORDS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var borrowrecord = await _context.BorrowRecords.FindAsync(id);
        if (borrowrecord != null)
        {
            _context.BorrowRecords.Remove(borrowrecord);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BorrowRecordExists(int? id)
    {
        return _context.BorrowRecords.Any(e => e.Id == id);
    }
}
