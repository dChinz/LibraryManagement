using Library.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext _context;

        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // =========================
            // SÁCH
            // =========================

            var books = await _context.Books
                .Where(b => b.DeletedAt == default(DateTime) || b.DeletedAt == null)
                .ToListAsync();

            // Số đầu sách
            var totalBookTitles = books.Count;

            // Tổng số bản sách
            var totalBooks = books.Sum(b => Convert.ToDouble(b.TotalCopies));

            // Tổng số bản sách có sẵn
            var availableBooks = books.Sum(b => Convert.ToDouble(b.AvailableCopies));


            // =========================
            // ĐỘC GIẢ
            // =========================

            var totalMembers = await _context.Members
                .Where(m => m.DeletedAt == default(DateTime) || m.DeletedAt == null)
                .CountAsync();


            // =========================
            // ĐANG MƯỢN
            // =========================

            var borrowingCount = await _context.BorrowRecords
                .CountAsync(b =>
                    b.ReturnDate == null &&
                    b.DueDate >= DateTime.Now);


            // =========================
            // QUÁ HẠN
            // =========================

            var overdueCount = await _context.BorrowRecords
                .CountAsync(b =>
                    b.ReturnDate == null &&
                    b.DueDate < DateTime.Now);


            // =========================
            // THỂ LOẠI
            // =========================

            var categoryCount = await _context.Categories
                .Where(c => c.DeletedAt == default(DateTime) || c.DeletedAt == null)
                .CountAsync();


            // =========================
            // LƯỢT MƯỢN THÁNG NÀY
            // =========================

            var now = DateTime.Now;

            var firstDayOfMonth = new DateTime(
                now.Year,
                now.Month,
                1
            );

            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var monthlyBorrowCount = await _context.BorrowRecords
                .CountAsync(b =>
                    b.BorrowDate >= firstDayOfMonth &&
                    b.BorrowDate < firstDayOfNextMonth);


            // =========================
            // MƯỢN SÁCH GẦN ĐÂY
            // =========================

            var recentBorrows = await _context.BorrowRecords
                .Include(b => b.Member)
                .Include(b => b.Book)
                .OrderByDescending(b => b.BorrowDate)
                .Take(5)
                .ToListAsync();


            // =========================
            // TÍNH PHẦN TRĂM
            // =========================

            double availablePercent = 0;
            double borrowingPercent = 0;
            double overduePercent = 0;

            if (totalBooks > 0)
            {
                availablePercent =
                    Math.Round(
                        availableBooks / totalBooks * 100,
                        1
                    );

                borrowingPercent =
                    Math.Round(
                        (totalBooks - availableBooks)
                        / totalBooks * 100,
                        1
                    );
            }

            if (borrowingCount > 0)
            {
                overduePercent =
                    Math.Round(
                        (double)overdueCount / borrowingCount * 100,
                        1
                    );
            }


            // =========================
            // TRUYỀN DỮ LIỆU SANG VIEW
            // =========================

            ViewBag.TotalBookTitles = totalBookTitles;

            ViewBag.TotalBooks = totalBooks;
            ViewBag.AvailableBooks = availableBooks;

            ViewBag.TotalMembers = totalMembers;

            ViewBag.BorrowingCount = borrowingCount;
            ViewBag.OverdueCount = overdueCount;

            ViewBag.CategoryCount = categoryCount;

            ViewBag.MonthlyBorrowCount = monthlyBorrowCount;

            ViewBag.AvailablePercent = availablePercent;
            ViewBag.BorrowingPercent = borrowingPercent;
            ViewBag.OverduePercent = overduePercent;

            ViewBag.RecentBorrows = recentBorrows;


            return View();
        }
    }
}