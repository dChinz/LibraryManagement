using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book
    {
        public Book()
        {
            BorrowRecords = new HashSet<BorrowRecord>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã sách không được để trống")]
        [Display(Name = "Mã sách")]
        public string Isbn { get; set; }

        [Required(ErrorMessage = "Tên sách không được để trống")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Tác giả không được để trống")]
        [Display(Name = "Tác giả")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Nhà xuất bản không được để trống")]
        [Display(Name = "Nhà xuất bản")]
        public string Publisher { get; set; }

        [Required(ErrorMessage = "Năm xuất bản không được để trống")]
        public int? PublishYear { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        [Display(Name = "Thể loại")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tổng số sách")]
        public int? TotalCopies { get; set; }

        public int? AvailableCopies { get; set; }

        [Unicode(true)]
        public string? Description { get; set; }

        [Display(Name = "Ảnh")]
        public string? CoverImagePath { get; set; }

        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<BorrowRecord> BorrowRecords { get; set; }
    }
}
