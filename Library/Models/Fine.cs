namespace Library.Models
{
    public class Fine
    {
        public int Id { get; set; }
        public int BorrowRecordId { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual BorrowRecord? BorrowRecord { get; set; }
    }
}
