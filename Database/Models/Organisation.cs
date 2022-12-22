namespace Database.Models
{
    public class Organisation
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string OrgName { get; set; }
        public string LegalAddress { get; set; }
        public string GetDirector { get; set; }
        public DateTime FoundingDate { get; set; }
    }
}
