namespace Database.Models
{
    public class Loan
    {
        public Loan(int userId, int serviceId, int amountMonth, int period) : this(0, userId, serviceId, amountMonth, period) { }

        public Loan(int id, int userId, int serviceId, int amountMonth, int period)
        {
            Id = id;
            UserId = userId;
            ServiceId = serviceId;
            AmountMonth = amountMonth;
            Period = period;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int AmountMonth { get; set; }
        public int Period { get; set; }
    }
}
