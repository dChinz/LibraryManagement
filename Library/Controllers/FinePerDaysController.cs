using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Data;

public class FinePerDaysController : Controller
{
    private readonly LibraryContext _context;

    public FinePerDaysController(LibraryContext context)
    {
        _context = context;
    }

    // GET: /FinePerDays
    public async Task<IActionResult> Index()
    {
        var finePerDay = await _context.FinePerDays.FirstOrDefaultAsync();

        if (finePerDay == null)
        {
            finePerDay = new FinePerDay
            {
                Price = 5000,
                UpdatedAt = DateTime.Now
            };

            _context.FinePerDays.Add(finePerDay);
            await _context.SaveChangesAsync();
        }

        return View("~/Views/Setting/Fine.cshtml", finePerDay);
    }

    // POST: /FinePerDays/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        [Bind("Id,Price")] FinePerDay model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Setting/Fine.cshtml", model);
        }

        var finePerDay = await _context.FinePerDays
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (finePerDay == null)
        {
            return NotFound();
        }

        finePerDay.Price = model.Price;
        finePerDay.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật mức tiền phạt thành công.";

        return RedirectToAction("Index", "Fine");
    }
}