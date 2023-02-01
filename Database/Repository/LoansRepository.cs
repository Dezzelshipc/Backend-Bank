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
