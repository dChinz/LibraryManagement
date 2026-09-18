namespace Library.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime DueDate {  get; set; }
        public DateTime? ReturnDate { get; set; }
        public BookStatus Status { get; set; } = BookStatus.BORROWING;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Member? Member { get; set; }
        public virtual Book? Book { get; set; }

    }
    public enum BookStatus
    {
        BORROWING,
        RETURNED,
        OVERDUE,
        LOST
    }
}
