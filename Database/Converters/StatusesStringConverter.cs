using static Database.Models.Loan;

namespace Database.Converters
{
    public static class StatusesStringConverter
    {
        public static string GetString(this Statuses status)
        {
            return status switch
            {
                Statuses.Pending => "pending",
                Statuses.Approved => "approved",
                Statuses.Declined => "declined",
                _ => "pending"
            };
        }
        public static Statuses GetStatus(this string status)
        {
            return status switch
            {
                "pending" => Statuses.Pending,
                "approved" => Statuses.Approved,
                "declined" => Statuses.Declined,
                _ => Statuses.None
            };
        }
    }
}
