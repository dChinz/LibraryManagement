using Library.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    public class ReportController : Controller
    {
        private readonly LibraryContext _context;

        public ReportController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;

            var firstDayOfMonth = new DateTime(
                now.Year,
                now.Month,
                1);

            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);


            // =========================
            // SÁCH
            // =========================

            var books = await _context.Books
                .Where(b => b.DeletedAt == default(DateTime) || b.DeletedAt == null)
                .ToListAsync();

            var totalBooks = books.Sum(b =>
                Convert.ToDouble(b.TotalCopies));

            var availableBooks = books.Sum(b =>
                Convert.ToDouble(b.AvailableCopies));


            // =========================
            // ĐỘC GIẢ
            // =========================

            var totalMembers = await _context.Members
                .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
                .CountAsync();

            var activeMembers = await _context.Members
                .Where(m =>
                    m.DeletedAt == null &&
                    m.Status == Library.Models.MemberStatus.ACTIVE)
                .CountAsync();

            var expiryMembers = await _context.Members
                .Where(m =>
                    m.DeletedAt == null &&
                    m.Status == Library.Models.MemberStatus.EXPIRED)
                .CountAsync();


            // =========================
            // MƯỢN SÁCH
            // =========================

            var borrowingBooks = await _context.BorrowRecords
                .CountAsync(b =>
                    b.ReturnDate == null &&
                    b.DueDate >= now);


            var overdueBooks = await _context.BorrowRecords
                .CountAsync(b =>
                    b.ReturnDate == null &&
                    b.DueDate < now);


            var returnedBooks = await _context.BorrowRecords
                .CountAsync(b =>
                    b.ReturnDate != null);


            // =========================
            // THỂ LOẠI
            // =========================

            var categories = await _context.Categories
                .Where(c => c.DeletedAt == default(DateTime) || c.DeletedAt == null)
                .CountAsync();


            // =========================
            // MƯỢN TRONG THÁNG
            // =========================

            var monthlyBorrows = await _context.BorrowRecords
                .CountAsync(b =>
                    b.BorrowDate >= firstDayOfMonth &&
                    b.BorrowDate < firstDayOfNextMonth);


            // =========================
            // TRẢ TRONG THÁNG
            // =========================

            var monthlyReturns = await _context.BorrowRecords
                .CountAsync(b =>
                    b.ReturnDate.HasValue &&
                    b.ReturnDate.Value >= firstDayOfMonth &&
                    b.ReturnDate.Value < firstDayOfNextMonth);


            // =========================
            // TIỀN PHẠT
            // =========================

            var totalFines = await _context.Fines
                .SumAsync(f => (decimal?)f.Amount) ?? 0;

            var paidFines = await _context.Fines
                .Where(f => !f.IsPaid)
                .SumAsync(f => (decimal?)f.Amount) ?? 0;

            var unpaidFines = await _context.Fines
                .Where(f => f.IsPaid)
                .SumAsync(f => (decimal?)f.Amount) ?? 0;


            // =========================
            // VIEWBAG
            // =========================

            ViewBag.TotalBooks = totalBooks;
            ViewBag.AvailableBooks = availableBooks;
            ViewBag.BorrowingBooks = borrowingBooks;
            ViewBag.ReturnedBooks = returnedBooks;

            ViewBag.TotalMembers = totalMembers;
            ViewBag.ActiveMembers = activeMembers;
            ViewBag.ExpiryMembers = expiryMembers;

            ViewBag.OverdueBooks = overdueBooks;

            ViewBag.Categories = categories;

            ViewBag.MonthlyBorrows = monthlyBorrows;
            ViewBag.MonthlyReturns = monthlyReturns;

            ViewBag.TotalFines = totalFines;
            ViewBag.PaidFines = paidFines;
            ViewBag.UnpaidFines = unpaidFines;


            return View();
        }
    }
}