using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Data;

public class FinePerDayController : Controller
{
    private readonly LibraryContext _context;

    public FinePerDayController(LibraryContext context)
    {
        _context = context;
    }

    // GET: FINEPERDAYS
    public async Task<IActionResult> Index()
    {
        return View(await _context.FinePerDays
            .OrderByDescending(f => f.Id)
            .ToListAsync());
    }

    // GET: FINEPERDAYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var fineperday = await _context.FinePerDays
            .FirstOrDefaultAsync(m => m.Id == id);

        if (fineperday == null)
            return NotFound();

        return View(fineperday);
    }

    // GET: FINEPERDAYS/Create
    public IActionResult Create()
    {
        return View(new FinePerDay
        {
            Price = 5000m,
            UpdatedAt = DateTime.Now
        });
    }

    // POST: FINEPERDAYS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Price")] FinePerDay fineperday)
    {
        if (!ModelState.IsValid)
            return View(fineperday);

        // Không cho tạo thêm bản ghi nếu đã có cấu hình mức phạt.
        bool exists = await _context.FinePerDays.AnyAsync();

        if (exists)
        {
            ModelState.AddModelError(
                "",
                "Mức phạt đã được thiết lập. Vui lòng chỉnh sửa mức phạt hiện tại.");

            return View(fineperday);
        }

        fineperday.UpdatedAt = DateTime.Now;

        _context.FinePerDays.Add(fineperday);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: FINEPERDAYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var fineperday = await _context.FinePerDays.FindAsync(id);

        if (fineperday == null)
            return NotFound();

        return View(fineperday);
    }

    // POST: FINEPERDAYS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("Id,Price")] FinePerDay model)
    {
        if (id == null || id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var fineperday = await _context.FinePerDays
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fineperday == null)
            return NotFound();

        // Chỉ cập nhật mức giá và thời điểm đổi giá.
        fineperday.Price = model.Price;
        fineperday.UpdatedAt = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await FinePerDayExists(fineperday.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: FINEPERDAYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var fineperday = await _context.FinePerDays
            .FirstOrDefaultAsync(m => m.Id == id);

        if (fineperday == null)
            return NotFound();

        return View(fineperday);
    }

    // POST: FINEPERDAYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
            return NotFound();

        var fineperday = await _context.FinePerDays.FindAsync(id);

        if (fineperday == null)
            return NotFound();

        _context.FinePerDays.Remove(fineperday);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> FinePerDayExists(int id)
    {
        return await _context.FinePerDays
            .AnyAsync(e => e.Id == id);
    }
}
