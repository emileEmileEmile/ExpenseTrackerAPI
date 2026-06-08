namespace ExpenseTrackerAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        
        public User User { get; set; } = null!;



    }

}