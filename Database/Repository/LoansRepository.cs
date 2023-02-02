using Database.Interfaces;
using Database.Models;

namespace Database.Repository
{
    public class LoansRepository : ILoansRepository
    {
        private readonly ApplicationContext _context;

        public LoansRepository(ApplicationContext context)
        {
            _context = context;
        }
        public void Create(Loan item)
        {
            _context.Loans.Add(item);
        }

        public bool Delete(int id)
        {
            var br = GetItem(id);
            if (br == default)
                return false;

            _context.Loans.Remove(br);
            return true;
        }

        public IEnumerable<Loan> GetAll()
        {
            return _context.Loans;
        }

        public Loan? GetItem(int id)
        {
            return _context.Loans.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Loan> GetLoansByServiceId(int serviceId)
        {
            return _context.Loans.Where(a => a.ServiceId == serviceId);
        }

        public IEnumerable<Loan> GetLoansByUserId(int userId)
        {
            return _context.Loans.Where(a => a.UserId == userId);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Update(Loan item)
        {
            _context.Loans.Update(item);
            return true;
        }
    }
}
