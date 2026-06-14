using ExpenseTrackerAPI.Enums;

namespace ExpenseTrackerAPI.DTO
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

    }
}