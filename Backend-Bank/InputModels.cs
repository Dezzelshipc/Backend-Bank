namespace Backend_Bank
{
    public class LoginModel
    {
        public LoginModel(string login, string password)
        {
            this.Login = login;
            this.Password = password;
        }

        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserData
    {
        public UserData(string phone, string email, string fullName)
        {
            this.Phone = phone;
            this.Email = email;
            this.FullName = fullName;
        }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }

    public class UserFullData : UserData
    {
        public UserFullData(string login, string phone, string email, string fullName) : base(phone, email, fullName)
        {
            this.Login = login;
        }

        public string Login { get; set; }
    }

    public class LoanData
    {
        public LoanData(int userId, int serviceId, int amountMonth, int period)
        {
            this.UserId = userId;
            this.ServiceId = serviceId;
            this.AmountMonth = amountMonth;
            this.Period = period;
        }

        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int AmountMonth { get; set; }
        public int Period { get; set; }
    }

    public class BranchData
    {
        public BranchData(string branchName, string branchAddress, string phoneNumber)
        {
            this.BranchName = branchName;
            this.BranchAddress = branchAddress;
            this.PhoneNumber = phoneNumber;
        }

        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class OrgData
    {
        public OrgData(string? orgName, string? legalAddress, string? genDirector, DateTime? foundingDate)
        {
            this.OrgName = orgName;
            this.LegalAddress = legalAddress;
            this.GenDirector = genDirector;
            this.FoundingDate = foundingDate;
        }

        public string? OrgName { get; set; }
        public string? LegalAddress { get; set; }
        public string? GenDirector { get; set; }
        public DateTime? FoundingDate { get; set; }
    }
    public class OrgFullData
    {
        public OrgFullData(string login, string password, string orgName, string legalAddress, string genDirector, DateTime foundingDate)
        {
            this.Login = login;
            this.Password = password;
            this.OrgName = orgName;
            this.LegalAddress = legalAddress;
            this.GenDirector = genDirector;
            this.FoundingDate = foundingDate;
        }

        public string Login { get; set; }
        public string Password { get; set; }
        public string OrgName { get; set; }
        public string LegalAddress { get; set; }
        public string GenDirector { get; set; }
        public DateTime FoundingDate { get; set; }
    }

    public class ServiceData
    {
        public ServiceData(string serviceName, string description, string percent, string minLoanPeriod, string maxLoanPeriod, bool isOnline)
        {
            this.ServiceName = serviceName;
            this.Description = description;
            this.Percent = percent;
            this.MinLoanPeriod = minLoanPeriod;
            this.MaxLoanPeriod = maxLoanPeriod;
            this.IsOnline = isOnline;
        }

        public string ServiceName { get; set; }
        public string Description { get; set; }
        public string Percent { get; set; }
        public string MinLoanPeriod { get; set; }
        public string MaxLoanPeriod { get; set; }
        public bool IsOnline { get; set; }
    }
}
