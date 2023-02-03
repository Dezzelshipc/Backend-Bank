using Database.Converters;
using Database.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend_Bank
{
    public class INT
    {
        public int Id { get; set; }
    }

    public class LoginModel
    {
        public LoginModel(string login, string password)
        {
            this.Login = login;
            this.Password = password;
        }

        public string Login { get; set; }
        public string Password { get; set; }

        public bool IsValid()
        {
            return !Login.IsNullOrEmpty() &&
                !Password.IsNullOrEmpty();
        }
        public bool IsNotValid() => !IsValid();
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

        virtual public bool IsValid()
        {
            return !Phone.IsNullOrEmpty() &&
                !Email.IsNullOrEmpty() &&
                !FullName.IsNullOrEmpty();
        }
        public bool IsNotValid() => !IsValid();
    }

    public class UserFullData
    {
        public UserFullData(string? login, string? phone, string? email, string? fullName)
        {
            Login = login;
            Phone = phone;
            Email = email;
            FullName = fullName;
        }

        public string? Login { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class LoanData
    {
        public LoanData(int serviceId, int amount, int period)
        {
            this.ServiceId = serviceId;
            this.Amount = amount;
            this.Period = period;
        }

        public int ServiceId { get; set; }
        public int Amount { get; set; }
        public int Period { get; set; }

        public bool IsValid()
        {
            return ServiceId >= 0 &&
                 Amount >= 0 &&
                 Period >= 0;
        }
        public bool IsNotValid() => !IsValid();
    }

    public class BranchData
    {
        public BranchData(string branchName, string branchAddress, string phoneNumber, Position coordinates)
        {
            BranchName = branchName;
            BranchAddress = branchAddress;
            PhoneNumber = phoneNumber;
            Coordinates = coordinates;
        }

        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string PhoneNumber { get; set; }
        public Position Coordinates { get; set; }

        public bool IsValid()
        {
            return !BranchName.IsNullOrEmpty() &&
                !BranchAddress.IsNullOrEmpty() &&
                !PhoneNumber.IsNullOrEmpty() &&
                Coordinates.IsValid();
        }
        public bool IsNotValid() => !IsValid();
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


        public bool IsValid()
        {
            return !Login.IsNullOrEmpty() &&
                !Password.IsNullOrEmpty() &&
                !OrgName.IsNullOrEmpty() &&
                !LegalAddress.IsNullOrEmpty() &&
                !GenDirector.IsNullOrEmpty() &&
                FoundingDate != null;
        }
        public bool IsNotValid() => !IsValid();
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

        public bool IsValid()
        {
            return !ServiceName.IsNullOrEmpty() &&
                !Description.IsNullOrEmpty() &&
                !Percent.IsNullOrEmpty() &&
                !MinLoanPeriod.IsNullOrEmpty() &&
                !MaxLoanPeriod.IsNullOrEmpty() &&
                IsOnline != null;
        }
        public bool IsNotValid() => !IsValid();
    }

    public class LoanNotification
    {
        public LoanNotification(int id, string description, string status)
        {
            Id = id;
            Description = description;
            Status = status;
        }

        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public bool IsValid()
        {
            if (Status != null)
                return Status.GetStatus() != Loan.Statuses.None;
            return true;
        }

        public bool IsNotValid => !IsValid();
    }
}
