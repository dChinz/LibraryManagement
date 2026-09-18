using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class DbInitalizer
    {
        public static void Initalizer(IServiceProvider serviceProvider)
        {
            using (var context = new LibraryContext(serviceProvider.GetRequiredService<DbContextOptions<LibraryContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.Categories.Any())
                {
                    return;
                }

                var categories = new Category[]
                {
                    new Category { Name = "Văn học", Description = "Tiểu thuyết, truyện ngắn, thơ ca", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Khoa học", Description = "Sách khoa học tự nhiên và ứng dụng", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Công nghệ thông tin", Description = "Lập trình, phần mềm, hệ thống", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Kinh tế", Description = "Quản trị, tài chính, kinh doanh", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Thiếu nhi", Description = "Sách dành cho trẻ em", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };

                foreach (var category in categories)
                    context.Categories.Add(category);
                context.SaveChanges();

                var itCategory = context.Categories.First(c => c.Name == "Công nghệ thông tin");
                var vanHocCategory = context.Categories.First(c => c.Name == "Văn học");
                var kinhTeCategory = context.Categories.First(c => c.Name == "Kinh tế");

                var books = new Book[]
                {
                    new Book
                    {
                        Isbn = "978-0132350884",
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        Publisher = "Prentice Hall",
                        PublishYear = 2008,
                        CategoryId = itCategory.Id,
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        Description = "A handbook of agile software craftsmanship.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Isbn = "978-0201633610",
                        Title = "Design Patterns",
                        Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                        Publisher = "Addison-Wesley",
                        PublishYear = 1994,
                        CategoryId = itCategory.Id,
                        TotalCopies = 3,
                        AvailableCopies = 3,
                        Description = "Elements of Reusable Object-Oriented Software.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Isbn = "978-6041012345",
                        Title = "Số đỏ",
                        Author = "Vũ Trọng Phụng",
                        Publisher = "NXB Văn Học",
                        PublishYear = 1936,
                        CategoryId = vanHocCategory.Id,
                        TotalCopies = 4,
                        AvailableCopies = 4,
                        Description = "Tiểu thuyết trào phúng nổi tiếng.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Isbn = "978-6041098765",
                        Title = "Nhà Giả Kim",
                        Author = "Paulo Coelho",
                        Publisher = "NXB Hội Nhà Văn",
                        PublishYear = 1988,
                        CategoryId = vanHocCategory.Id,
                        TotalCopies = 6,
                        AvailableCopies = 6,
                        Description = "Hành trình đi tìm kho báu và ý nghĩa cuộc sống.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Isbn = "978-1119561225",
                        Title = "Principles of Corporate Finance",
                        Author = "Richard Brealey, Stewart Myers",
                        Publisher = "McGraw-Hill",
                        PublishYear = 2019,
                        CategoryId = kinhTeCategory.Id,
                        TotalCopies = 2,
                        AvailableCopies = 2,
                        Description = "Giáo trình tài chính doanh nghiệp kinh điển.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                foreach(var book in books )
                    context.Books.Add( book );
                context.SaveChanges();

                var members = new List<Member>
                {
                    new Member
                    {
                        MemberCode = "MB0001",
                        FullName = "Tran Thi B",
                        Email = "tranthib@example.com",
                        Phone = "0901234567",
                        Address = "123 Le Loi, Q1, TP.HCM",
                        JoinDate = DateTime.UtcNow.AddMonths(-6),
                        ExpiryDate = DateTime.UtcNow.AddMonths(6),
                        Status = MemberStatus.ACTIVE,
                        LostBookCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Member
                    {
                        MemberCode = "MB0002",
                        FullName = "Le Van C",
                        Email = "levanc@example.com",
                        Phone = "0912345678",
                        Address = "45 Nguyen Trai, Q5, TP.HCM",
                        JoinDate = DateTime.UtcNow.AddMonths(-3),
                        ExpiryDate = DateTime.UtcNow.AddMonths(9),
                        Status = MemberStatus.ACTIVE,
                        LostBookCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Member
                    {
                        MemberCode = "MB0003",
                        FullName = "Pham Thi D",
                        Email = "phamthid@example.com",
                        Phone = "0987654321",
                        Address = "78 Tran Hung Dao, Ha Noi",
                        JoinDate = DateTime.UtcNow.AddYears(-1),
                        ExpiryDate = DateTime.UtcNow.AddDays(-10),
                        Status = MemberStatus.EXPIRED,
                        LostBookCount = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                foreach (var member in members)
                    context.Members.Add(member);
                context.SaveChanges();

                var member1 = context.Members.First(m => m.MemberCode == "MB0001");
                var member2 = context.Members.First(m => m.MemberCode == "MB0002");
                var member3 = context.Members.First(m => m.MemberCode == "MB0003");

                var cleanCode = context.Books.First(b => b.Isbn == "978-0132350884");
                var designPatterns = context.Books.First(b => b.Isbn == "978-0201633610");
                var nhaGiaKim = context.Books.First(b => b.Isbn == "978-6041098765");

                var records = new List<BorrowRecord>
                {
                    new BorrowRecord
                    {
                        MemberId = member1.Id,
                        BookId = cleanCode.Id,
                        BorrowDate = DateTime.UtcNow.AddDays(-10),
                        DueDate = DateTime.UtcNow.AddDays(4),
                        ReturnDate = null,
                        Status = BookStatus.BORROWING,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new BorrowRecord
                    {
                        MemberId = member2.Id,
                        BookId = designPatterns.Id,
                        BorrowDate = DateTime.UtcNow.AddDays(-20),
                        DueDate = DateTime.UtcNow.AddDays(-6),
                        ReturnDate = null,
                        Status = BookStatus.OVERDUE,
                        Note = "Quá hạn trả sách",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new BorrowRecord
                    {
                        MemberId = member3.Id,
                        BookId = nhaGiaKim.Id,
                        BorrowDate = DateTime.UtcNow.AddDays(-30),
                        DueDate = DateTime.UtcNow.AddDays(-16),
                        ReturnDate = DateTime.UtcNow.AddDays(-18),
                        Status = BookStatus.RETURNED,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                foreach(var record in records)
                    context.BorrowRecords.Add(record);
                context.SaveChanges();

                var overdueRecord = context.BorrowRecords
                .First(r => r.Status == BookStatus.OVERDUE);
                var fines = new List<Fine>
                {
                    new Fine
                    {
                        BorrowRecordId = overdueRecord.Id,
                        Amount = 50000m,
                        Reason = "Trả sách trễ hạn",
                        IsPaid = false,
                        PaidDate = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                foreach( var fine in fines)
                    context.Fines.Add(fine);
                context.SaveChanges();
            }
        }
    }
}
