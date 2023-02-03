using Database.Models;
using System.Linq.Expressions;

namespace Database.Interfaces
{
    public interface IBranchesRepository : IRepository<Branch>
    {
        public int Find(Branch branch);
        public IEnumerable<Branch> GetBranches(int organisationId);
        public IEnumerable<Branch> OnDistande(double distanceKm, Position position);
    }
}
