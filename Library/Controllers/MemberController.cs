using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MemberController : Controller
{
    private readonly LibraryContext _context;

    public MemberController(LibraryContext context)
    {
        _context = context;
    }

    // GET: MEMBERS
    public async Task<IActionResult> Index()    
    {
        var member = await _context.Members
            .Where(m => m.DeletedAt == null || m.DeletedAt == default(DateTime))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
        return View(member);
    }

    // GET: MEMBERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.Id == id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // GET: MEMBERS/Create
    public IActionResult Create()
    {
        return View(new Member { ExpiryDate = DateTime.Today.AddMonths(1) });
    }

    // POST: MEMBERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (member.ExpiryDate.Date < DateTime.Today)
        {
            ModelState.AddModelError(
                "ExpiryDate",
                "Ngày hết hạn không được nhỏ hơn ngày hiện tại."
            );
        }

        if (ModelState.IsValid)
        {
            member.JoinDate = DateTime.Now;
            member.CreatedAt = DateTime.Now;
            member.UpdatedAt = DateTime.Now;
            member.MemberCode = "TEMP";

            _context.Add(member);
            await _context.SaveChangesAsync();

            member.MemberCode = "MB" + member.Id.ToString("D4");
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(member);
    }

    // GET: MEMBERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var member = await _context.Members.FindAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    // POST: MEMBERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id,  Member member)
    {
        if (id != member.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                member.UpdatedAt = DateTime.Now;

                _context.Update(member);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.Id))
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
        return View(member);
    }

    // GET: MEMBERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.Id == id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST: MEMBERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member != null)
        {
            member.DeletedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MemberExists(int? id)
    {
        return _context.Members.Any(e => e.Id == id);
    }
}
