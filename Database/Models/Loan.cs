namespace Database.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int AmountMonth { get; set; }
        public int Period { get; set; }
    }
}
