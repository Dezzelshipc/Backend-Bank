using Database.Models;

namespace Database.Interfaces
{
    public interface IBranchesRepository : IRepository<Branch>
    {
        public int Find(Branch branch);
        public IEnumerable<Branch> GetBranches(int organisationId);
    }
}
