
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Data;
using BCrypt.Net;

public class UserController : Controller
{
    private readonly LibraryContext _context;

    public UserController(LibraryContext context)
    {
        _context = context;
    }

    // GET: USERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Users.ToListAsync());
    }

    // GET: USERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: USERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: USERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            _context.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // GET: USERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }


    // POST: USERS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user, string? newPassword)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        var existingUser = await _context.Users.FindAsync(id);

        if (existingUser == null)
        {
            return NotFound();
        }

        // PasswordHash không được gửi từ form Edit.
        // Xóa lỗi validation của thuộc tính này nếu đang để trống.
        ModelState.Remove(nameof(Library.Models.User.PasswordHash));

        // Kiểm tra mật khẩu mới nếu người dùng có nhập
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            if (newPassword.Length < 6)
            {
                ModelState.AddModelError(
                    "newPassword",
                    "Mật khẩu mới phải có ít nhất 6 ký tự."
                );
            }
        }

        if (!ModelState.IsValid)
        {
            return View(user);
        }

        // Cập nhật thông tin, giữ nguyên mật khẩu cũ nếu không đổi
        existingUser.Username = user.Username;
        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
        existingUser.Role = user.Role;
        existingUser.IsActive = user.IsActive;
        existingUser.UpdatedAt = DateTime.Now;

        // Chỉ mã hóa và lưu khi có mật khẩu mới
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            existingUser.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newPassword);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật tài khoản thành công!";

        return RedirectToAction(nameof(Index));
    }


    // GET: USERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: USERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(int? id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
