namespace Database.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Percent { get; set; }
        public string MinLoanPeriod { get; set; }
        public string MaxLoadPeriod { get; set; }
        public bool IsOnline { get; set; }
    }
}
