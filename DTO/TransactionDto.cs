using ExpenseTrackerAPI.Enums;

namespace ExpenseTrackerAPI.DTO
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }

    }
}