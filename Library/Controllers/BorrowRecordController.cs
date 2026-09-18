
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
    public async Task<IActionResult> Index()    
    {
        var borrow = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .ToListAsync();

        return View(borrow);
    }

    // GET: BORROWRECORDS/Details/5
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
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .OrderBy(m => m.FullName)
            .Select(m => new
            {
                m.Id,
                DisplayName = m.MemberCode + " - " + m.FullName
            })
            .ToListAsync();

        ViewData["MemberId"] = new SelectList(
            members,
            "Id",
            "DisplayName"
        );

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
    public async Task<IActionResult> Create(BorrowRecord borrowrecord, List<int> BookIds)
    {
        var members = await _context.Members
            .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
            .OrderBy(m => m.FullName)
            .Select(m => new
            {
                m.Id,
                DisplayName = m.MemberCode + " - " + m.FullName
            })
            .ToListAsync();

        ViewData["MemberId"] = new SelectList(
            members,
            "Id",
            "DisplayName",
            borrowrecord.MemberId
        );

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

        // BookId không còn được nhập trực tiếp từ form
        ModelState.Remove("BookId");

        if (BookIds == null || BookIds.Count == 0)
        {
            ModelState.AddModelError("BookIds", "Vui lòng chọn ít nhất một cuốn sách.");
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
