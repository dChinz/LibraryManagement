using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;

public class FineController : Controller
{
    private readonly LibraryContext _context;

    public FineController(LibraryContext context)
    {
        _context = context;
    }

    // GET: Fine
    public async Task<IActionResult> Index(
        string? searchString,
        int? borrowRecordId,
        int page = 1,
        int pageSize = 10)
    {
        var now = DateTime.Now;
        var today = now.Date;

        // 1. Lấy cấu hình mức phạt hiện tại.
        var fineSetting = await _context.FinePerDays
            .FirstOrDefaultAsync();

        decimal currentFinePerDay = fineSetting?.Price ?? 5000m;

        // Ngày cập nhật mức phạt gần nhất.
        DateTime priceUpdatedAt = fineSetting?.UpdatedAt.Date
            ?? DateTime.MaxValue.Date;

        const decimal oldFinePerDay = 5000m;

        // 2. Lấy các lượt mượn đang quá hạn.
        var overdueRecords = await _context.BorrowRecords
            .Where(r => r.Status == BookStatus.OVERDUE)
            .ToListAsync();

        var overdueIds = overdueRecords
            .Select(r => r.Id)
            .ToList();

        // 3. Lấy các khoản phạt đã tồn tại của những lượt mượn quá hạn.
        var existingFines = await _context.Fines
            .Where(f => overdueIds.Contains(f.BorrowRecordId))
            .ToListAsync();

        // 4. Tính tiền phạt cho từng lượt mượn quá hạn.
        foreach (var record in overdueRecords)
        {
            // Chưa đến ngày quá hạn thì bỏ qua.
            if (record.DueDate.Date >= today)
                continue;

            var fine = existingFines
                .FirstOrDefault(f => f.BorrowRecordId == record.Id);

            // Không tính lại khoản phạt đã thanh toán.
            if (fine != null && fine.IsPaid)
                continue;

            // Ngày đầu tiên tính phạt là ngày sau hạn trả.
            DateTime firstOverdueDate = record.DueDate.Date.AddDays(1);

            // Tính số ngày áp dụng giá cũ.
            // Nếu chưa cấu hình mức phạt, tất cả ngày quá hạn
            // được tính theo mức mặc định 5.000 đồng/ngày.
            int oldDays = 0;

            if (priceUpdatedAt == DateTime.MaxValue.Date)
            {
                oldDays = (today - firstOverdueDate).Days + 1;
            }
            else
            {
                DateTime oldPeriodEnd = priceUpdatedAt.AddDays(-1);

                if (firstOverdueDate <= oldPeriodEnd)
                {
                    DateTime oldEnd = oldPeriodEnd < today
                        ? oldPeriodEnd
                        : today;

                    if (firstOverdueDate <= oldEnd)
                    {
                        oldDays = (oldEnd - firstOverdueDate).Days + 1;
                    }
                }
            }

            // Tính số ngày áp dụng giá hiện tại,
            // bắt đầu từ ngày cập nhật giá.
            int newDays = 0;

            if (priceUpdatedAt != DateTime.MaxValue.Date)
            {
                DateTime newPeriodStart = firstOverdueDate > priceUpdatedAt
                    ? firstOverdueDate
                    : priceUpdatedAt;

                if (newPeriodStart <= today)
                {
                    newDays = (today - newPeriodStart).Days + 1;
                }
            }

            decimal amount =
                oldDays * oldFinePerDay
                + newDays * currentFinePerDay;

            int overdueDays = (today - record.DueDate.Date).Days;

            string reason = $"Trả sách trễ hạn {overdueDays} ngày";

            // 5. Tạo khoản phạt mới hoặc cập nhật khoản chưa thanh toán.
            if (fine == null)
            {
                fine = new Fine
                {
                    BorrowRecordId = record.Id,
                    Amount = amount,
                    Reason = reason,
                    IsPaid = false,
                    PaidDate = null,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _context.Fines.Add(fine);
                existingFines.Add(fine);
            }
            else
            {
                fine.Amount = amount;
                fine.Reason = reason;
                fine.UpdatedAt = now;
            }
        }

        await _context.SaveChangesAsync();

        // 6. Lấy danh sách tiền phạt.
        var query = _context.Fines
            .Include(f => f.BorrowRecord)
                .ThenInclude(b => b.Member)
            .Include(f => f.BorrowRecord)
                .ThenInclude(b => b.Book)
            .AsQueryable();

        // 7. Lọc theo mã lượt mượn nếu được chuyển từ bảng mượn sách.
        if (borrowRecordId.HasValue)
        {
            // Lọc chính xác theo mã lượt mượn.
            query = query.Where(f =>
                f.BorrowRecordId == borrowRecordId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(searchString))
        {
            // Tìm kiếm theo từ khóa.
            searchString = searchString.Trim();

            query = query.Where(f => f.BorrowRecordId.ToString().Contains(searchString));
        }

        // 8. Sắp xếp và phân trang.
        query = query.OrderByDescending(f => f.CreatedAt);

        int totalItems = await query.CountAsync();

        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)pageSize);

        // Đảm bảo trang hiện tại không vượt quá tổng số trang.
        if (totalPages > 0 && page > totalPages)
            page = totalPages;

        var fines = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 9. Truyền dữ liệu sang View.
        ViewBag.SearchString = searchString;
        ViewBag.BorrowRecordId = borrowRecordId;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentFinePerDay = currentFinePerDay;

        // Model phân trang.
        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(fines);
    }

    // GET: Fine/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var fine = await _context.Fines
            .Include(f => f.BorrowRecord)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fine == null)
            return NotFound();

        return View(fine);
    }

    // POST: Fine/FineCollected/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FineCollected(int id)
    {
        var fine = await _context.Fines
            .Include(f => f.BorrowRecord)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fine == null)
        {
            TempData["Error"] = "Không tìm thấy khoản phạt.";
            return RedirectToAction(nameof(Index));
        }

        if (fine.IsPaid)
        {
            TempData["Error"] = "Khoản phạt này đã được thanh toán trước đó.";
            return RedirectToAction(nameof(Index));
        }

        var now = DateTime.Now;

        // 1. Cập nhật khoản phạt.
        fine.IsPaid = true;
        fine.PaidDate = now;
        fine.UpdatedAt = now;

        // 2. Chỉ cập nhật thông tin lượt mượn,
        // không tự chuyển trạng thái khi độc giả chưa trả sách.
        if (fine.BorrowRecord != null)
        {
            fine.BorrowRecord.UpdatedAt = now;

            // Không tự đổi OVERDUE thành RETURNED ở đây.
            // Trạng thái RETURNED cần được cập nhật tại chức năng trả sách.
        }

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Đã xác nhận thu tiền phạt thành công.";

        return RedirectToAction(nameof(Index));
    }
}
