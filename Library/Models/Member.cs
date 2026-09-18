using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Member
    {
        public Member()
        {
            BorrowRecords = new HashSet<BorrowRecord>();
        }
        public int Id { get; set; }
        public string? MemberCode { get; set; }

        [Required(ErrorMessage = "Tên độc giả không được để trống")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"[A-Za-z0-9./+%]+@[A-Za-z0-9.]+\.[A-Za-z]{2,}", ErrorMessage = "Email sai định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Length(10, 10, ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Gia nhập")]
        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Display(Name = "Hết hạn")]
        [DataType(DataType.Date)] 

        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Tình trạng")]
        public MemberStatus Status { get; set; } = MemberStatus.ACTIVE;

        [Display(Name = "Đánh mất sách")]
        public int LostBookCount { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<BorrowRecord> BorrowRecords { get; set; }
    }
    public enum MemberStatus
    {
        ACTIVE,
        EXPIRED,
        SUSPENDED
    }
}
