using Microsoft.EntityFrameworkCore;

namespace Database.Models
{
    [Index(nameof(Login), IsUnique = true)]
    public class Organisation
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string OrgName { get; set; }
        public string LegalAddress { get; set; }
        public string GenDirector { get; set; }
        public DateTime FoundingDate { get; set; }

        public Organisation(string login, string password, string orgName, string legalAddress, string genDirector, DateTime foundingDate)
        {
            Login = login;
            Password = password;
            OrgName = orgName;
            LegalAddress = legalAddress;
            GenDirector = genDirector;
            FoundingDate = foundingDate;
        }

        public bool IsValid()
        {
            return !(Id < 0 ||
                string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(OrgName) ||
                string.IsNullOrEmpty(LegalAddress) ||
                string.IsNullOrEmpty(GenDirector));
        }
    }
}
