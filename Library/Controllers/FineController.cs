
using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class FineController : Controller
{
    private readonly LibraryContext _context;

    public FineController(LibraryContext context)
    {
        _context = context;
    }

    // GET: FINES
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var query = _context.Fines
            .Include (f => f.BorrowRecord)
            .ThenInclude(f => f.Member)
            .OrderByDescending(f => f.Id);

        int totalItems = await query.CountAsync();

        var fines = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(fines);
    }

    // GET: FINES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fine = await _context.Fines
            .Include(f => f.BorrowRecord)
                .ThenInclude(b => b.Member)
            .Include(f => f.BorrowRecord)
                .ThenInclude(b => b.Book)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fine == null)
        {
            return NotFound();
        }

        return View(fine);
    }

    // GET: FINES/Create
    public async Task<IActionResult> Create()
    {
        var borrowRecords = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();

        var borrowRecordList = borrowRecords.Select(b => new
        {
            Id = b.Id,
            DisplayName =
                "#" + b.Id +
                " - " +
                (b.Member != null ? b.Member.FullName : "Không có độc giả") +
                " - " +
                (b.Book != null ? b.Book.Title : "Không có sách")
        });

        ViewData["BorrowRecordId"] = new SelectList(
            borrowRecordList,
            "Id",
            "DisplayName"
        );

        return View();
    }

    // POST: FINES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("BorrowRecordId,Amount,Reason,IsPaid,PaidDate")]
    Fine fine)
    {
        if (ModelState.IsValid)
        {
            var now = DateTime.Now;

            fine.CreatedAt = now;
            fine.UpdatedAt = now;

            if (fine.IsPaid && !fine.PaidDate.HasValue)
            {
                fine.PaidDate = now;
            }

            if (!fine.IsPaid)
            {
                fine.PaidDate = null;
            }

            _context.Fines.Add(fine);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        var borrowRecords = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();

        var borrowRecordList = borrowRecords.Select(b => new
        {
            Id = b.Id,
            DisplayName =
                "#" + b.Id +
                " - " +
                (b.Member != null ? b.Member.FullName : "Không có độc giả") +
                " - " +
                (b.Book != null ? b.Book.Title : "Không có sách")
        });

        ViewData["BorrowRecordId"] = new SelectList(
            borrowRecordList,
            "Id",
            "DisplayName",
            fine.BorrowRecordId
        );

        return View(fine);
    }

    // GET: FINES/Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fine = await _context.Fines
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fine == null)
        {
            return NotFound();
        }

        var borrowRecords = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();

        var borrowRecordList = borrowRecords.Select(b => new
        {
            Id = b.Id,
            DisplayName =
                "#" + b.Id +
                " - " +
                (b.Member != null ? b.Member.FullName : "Không có độc giả") +
                " - " +
                (b.Book != null ? b.Book.Title : "Không có sách")
        });

        ViewData["BorrowRecordId"] = new SelectList(
            borrowRecordList,
            "Id",
            "DisplayName",
            fine.BorrowRecordId
        );

        return View(fine);
    }

    // POST: FINES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,BorrowRecordId,Amount,Reason,IsPaid,PaidDate")]
    Fine fine)
    {
        if (id != fine.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingFine = await _context.Fines
                .FirstOrDefaultAsync(f => f.Id == id);

            if (existingFine == null)
            {
                return NotFound();
            }

            existingFine.BorrowRecordId = fine.BorrowRecordId;
            existingFine.Amount = fine.Amount;
            existingFine.Reason = fine.Reason;
            existingFine.IsPaid = fine.IsPaid;
            existingFine.PaidDate = fine.IsPaid
                ? (fine.PaidDate ?? DateTime.Now)
                : null;

            existingFine.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        var borrowRecords = await _context.BorrowRecords
            .Include(b => b.Member)
            .Include(b => b.Book)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();

        var borrowRecordList = borrowRecords.Select(b => new
        {
            Id = b.Id,
            DisplayName =
                "#" + b.Id +
                " - " +
                (b.Member != null ? b.Member.FullName : "Không có độc giả") +
                " - " +
                (b.Book != null ? b.Book.Title : "Không có sách")
        });

        ViewData["BorrowRecordId"] = new SelectList(
            borrowRecordList,
            "Id",
            "DisplayName",
            fine.BorrowRecordId
        );

        return View(fine);
    }

    // GET: FINES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fine = await _context.Fines
            .Include(f => f.BorrowRecord)
                .ThenInclude(b => b.Member)
            .Include(f => f.BorrowRecord)
                .ThenInclude(b => b.Book)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fine == null)
        {
            return NotFound();
        }

        return View(fine);
    }

    // POST: FINES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var fine = await _context.Fines.FindAsync(id);

        if (fine != null)
        {
            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
