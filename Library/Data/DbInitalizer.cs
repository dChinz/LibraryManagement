using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class DbInitalizer
    {
        public static void Initalizer(IServiceProvider serviceProvider)
        {
            using (var context = new LibraryContext(
                serviceProvider.GetRequiredService<DbContextOptions<LibraryContext>>()))
            {
                context.Database.EnsureCreated();

                if (context.Categories.Any())
                {
                    return;
                }

                // =========================================================
                // 1. SEED CATEGORY - 20 RECORDS
                // =========================================================

                var categoryNames = new[]
                {
                    "Văn học",
                    "Khoa học",
                    "Công nghệ thông tin",
                    "Kinh tế",
                    "Thiếu nhi",
                    "Lịch sử",
                    "Địa lý",
                    "Tâm lý học",
                    "Triết học",
                    "Ngoại ngữ",
                    "Giáo dục",
                    "Y học",
                    "Kỹ năng sống",
                    "Nghệ thuật",
                    "Du lịch",
                    "Tôn giáo",
                    "Chính trị",
                    "Luật",
                    "Kỹ thuật",
                    "Thể thao"
                };

                var categories = new List<Category>();

                foreach (var name in categoryNames)
                {
                    categories.Add(new Category
                    {
                        Name = name,
                        Description = $"Sách thuộc lĩnh vực {name.ToLower()}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Categories.AddRange(categories);
                context.SaveChanges();


                // =========================================================
                // 2. SEED BOOK - 25 RECORDS
                // =========================================================

                var categoryList = context.Categories
                    .OrderBy(c => c.Id)
                    .ToList();

                var bookTitles = new[]
                {
                    "Clean Code",
                    "Design Patterns",
                    "C# Programming",
                    "ASP.NET Core MVC",
                    "Lập trình Python",
                    "Nhà Giả Kim",
                    "Số Đỏ",
                    "Dế Mèn Phiêu Lưu Ký",
                    "Tuổi Thơ Dữ Dội",
                    "Harry Potter",
                    "Đắc Nhân Tâm",
                    "7 Thói Quen Hiệu Quả",
                    "Tư Duy Nhanh Và Chậm",
                    "Cha Giàu Cha Nghèo",
                    "Nguyên Lý Marketing",
                    "Lược Sử Thời Gian",
                    "Vũ Trụ",
                    "Lịch Sử Việt Nam",
                    "Đại Việt Sử Ký",
                    "Nhập Môn Kinh Tế Học",
                    "Giáo Trình Luật Dân Sự",
                    "Cơ Sở Dữ Liệu",
                    "Mạng Máy Tính",
                    "Trí Tuệ Nhân Tạo",
                    "Machine Learning Cơ Bản"
                };

                var authors = new[]
                {
                    "Robert C. Martin",
                    "Erich Gamma",
                    "Microsoft Press",
                    "Nguyễn Văn A",
                    "Trần Văn B",
                    "Paulo Coelho",
                    "Vũ Trọng Phụng",
                    "Tô Hoài",
                    "Phùng Quán",
                    "J.K. Rowling",
                    "Dale Carnegie",
                    "Stephen R. Covey",
                    "Daniel Kahneman",
                    "Robert Kiyosaki",
                    "Philip Kotler",
                    "Stephen Hawking",
                    "Carl Sagan",
                    "Nguyễn Khắc Thuần",
                    "Lê Văn Lan",
                    "N. Gregory Mankiw",
                    "Nguyễn Minh Tuấn",
                    "Nguyễn Thị C",
                    "Trần Văn D",
                    "Nguyễn Thanh E",
                    "Andrew Ng"
                };

                var books = new List<Book>();

                for (int i = 0; i < 25; i++)
                {
                    int totalCopies = 3 + (i % 6);

                    books.Add(new Book
                    {
                        Isbn = $"978-604-{1000000 + i}",
                        Title = bookTitles[i],
                        Author = authors[i],
                        Publisher = i % 2 == 0
                            ? "NXB Giáo Dục"
                            : "NXB Tổng Hợp",
                        PublishYear = 2000 + (i % 24),
                        CategoryId = categoryList[i % categoryList.Count].Id,
                        TotalCopies = totalCopies,
                        AvailableCopies = totalCopies,
                        Description = $"Thông tin mô tả cho cuốn sách {bookTitles[i]}.",
                        CoverImagePath = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Books.AddRange(books);
                context.SaveChanges();


                // =========================================================
                // 3. SEED MEMBER - 25 RECORDS
                // =========================================================

                var members = new List<Member>();

                for (int i = 1; i <= 25; i++)
                {
                    bool expired = i % 5 == 0;

                    members.Add(new Member
                    {
                        MemberCode = $"MB{i:D4}",
                        FullName = $"Nguyễn Văn Thành {i}",
                        Email = $"member{i}@example.com",
                        Phone = $"09{10000000 + i}",
                        Address = $"{i} Nguyễn Trãi, Hà Nội",
                        JoinDate = DateTime.UtcNow.AddMonths(-(i % 12)),
                        ExpiryDate = expired
                            ? DateTime.UtcNow.AddDays(-(i * 2))
                            : DateTime.UtcNow.AddMonths(3 + (i % 6)),
                        Status = expired
                            ? MemberStatus.EXPIRED
                            : MemberStatus.ACTIVE,
                        LostBookCount = i % 7 == 0 ? 1 : 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Members.AddRange(members);
                context.SaveChanges();


                // =========================================================
                // 4. SEED BORROW RECORD - 25 RECORDS
                // =========================================================

                var memberList = context.Members
                    .OrderBy(m => m.Id)
                    .ToList();

                var bookList = context.Books
                    .OrderBy(b => b.Id)
                    .ToList();

                var records = new List<BorrowRecord>();

                for (int i = 0; i < 25; i++)
                {
                    var borrowDate = DateTime.UtcNow.AddDays(-(i + 5));

                    BookStatus status;
                    DateTime? returnDate = null;
                    DateTime dueDate;
                    string? note = null;

                    // 1 - 10: Đang mượn
                    if (i < 10)
                    {
                        status = BookStatus.BORROWING;
                        dueDate = DateTime.UtcNow.AddDays(7 + i);
                    }

                    // 10 - 20: Quá hạn
                    else if (i < 20)
                    {
                        status = BookStatus.OVERDUE;
                        dueDate = DateTime.UtcNow.AddDays(-(i - 8));
                        note = "Quá hạn trả sách";
                    }

                    // 20 - 25: Đã trả
                    else
                    {
                        status = BookStatus.RETURNED;
                        dueDate = borrowDate.AddDays(14);
                        returnDate = dueDate.AddDays(i % 3);
                    }

                    records.Add(new BorrowRecord
                    {
                        MemberId = memberList[i % memberList.Count].Id,
                        BookId = bookList[i % bookList.Count].Id,
                        BorrowDate = borrowDate,
                        DueDate = dueDate,
                        ReturnDate = returnDate,
                        Status = status,
                        Note = note,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.BorrowRecords.AddRange(records);
                context.SaveChanges();


                // =========================================================
                // 5. SEED FINE - 20 RECORDS
                //    Tạo tiền phạt cho 20 lượt mượn quá hạn
                // =========================================================

                var overdueRecords = context.BorrowRecords
                    .Where(r => r.Status == BookStatus.OVERDUE)
                    .OrderBy(r => r.Id)
                    .Take(20)
                    .ToList();

                var fines = new List<Fine>();

                for (int i = 0; i < overdueRecords.Count; i++)
                {
                    fines.Add(new Fine
                    {
                        BorrowRecordId = overdueRecords[i].Id,
                        Amount = 20000m + (i * 5000m),
                        Reason = "Trả sách trễ hạn",
                        IsPaid = i % 3 == 0,
                        PaidDate = i % 3 == 0
                            ? DateTime.UtcNow.AddDays(-(i % 10))
                            : null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Fines.AddRange(fines);
                context.SaveChanges();


                // =========================================================
                // 6. SEED USER
                //    GIỮ NGUYÊN, KHÔNG THÊM USER MỚI
                // =========================================================

                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        PasswordHash = "$2a$11$vrFN/HSayqHwJcrtbRH2UObki3oWnmyAA1GuRTeExEjy.c/LKSz1u",
                        FullName = "Admin",
                        Email = "admin@gmail.com",
                        Role = UseRole.ADMIN,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    new User
                    {
                        Username = "library",
                        PasswordHash = "$2a$11$3p6izJLWv4VcU0TagjDjwuykm.YsZ4ZjN9De9pUlFfjgn7Ex8DaEW",
                        FullName = "Library",
                        Email = "library@gmail.com",
                        Role = UseRole.LIBRARIAN,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
