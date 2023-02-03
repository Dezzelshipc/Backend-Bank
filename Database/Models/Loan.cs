using Database.Converters;

namespace Database.Models
{

    public class Loan : IModel
    {
        public Loan(int id, int userId, int serviceId, int amount, int period, string desctiption, Statuses status)
        {
            Id = id;
            UserId = userId;
            ServiceId = serviceId;
            Amount = amount;
            Period = period;
            Desctiption = desctiption;
            Status = status;
        }

        public Loan(int userId, int serviceId, int amount, int period) : this(0, userId, serviceId, amount, period, string.Empty, Statuses.Pending) { }

        public enum Statuses
        {
            Pending, Approved, Declined, None
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int Amount { get; set; }
        public int Period { get; set; }
        public string Desctiption { get; set; }
        public Statuses Status { get; set; }

        public dynamic WithFormatStatus()
        {
            return new
            {
                Id,
                UserId,
                ServiceId,
                Amount,
                Period,
                Desctiption,
                Status = Status.GetString()
            };
        }
    }
}
