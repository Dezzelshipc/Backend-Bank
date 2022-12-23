using Database.Interfaces;
using Database.Models;

namespace Database.Repository
{
    public class BranchesRepository : IBranchesRepository
    {
        private readonly ApplicationContext _context;

        public BranchesRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool Create(Branch item)
        {
            _context.Branches.Add(item);
            return true;
        }

        public bool Delete(int id)
        {
            var br = GetItem(id);
            if (br == default)
                return false;

            _context.Branches.Remove(br);
            return true;
        }

        public int Find(Branch branch)
        {
            return _context.Branches.FirstOrDefault(a => a.Name == branch.Name && a.OrganisationId == branch.OrganisationId).Id;
        }

        public IEnumerable<Branch> GetAll()
        {
            return _context.Branches;
        }

        public Branch? GetItem(int id)
        {
            return _context.Branches.FirstOrDefault(a => a.Id == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Update(Branch item)
        {
            _context.Branches.Update(item);
            return true;
        }
    }
}
