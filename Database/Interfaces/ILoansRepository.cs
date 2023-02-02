using Database.Models;

namespace Database.Interfaces
{
    public interface ILoansRepository : IRepository<Loan>
    {
        public IEnumerable<Loan> GetLoansByUserId(int userId);
        public IEnumerable<Loan> GetLoansByServiceId(int serviceId);
    }
}
