namespace ExpenseTrackerAPI.DTO 
{
    public class CategoryResponseDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }
        
    }
}