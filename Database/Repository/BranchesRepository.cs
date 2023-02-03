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

        public void Create(Branch item)
        {
            _context.Branches.Add(item);
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

        public IEnumerable<Branch> GetBranches(int organisationId)
        {
            return _context.Branches.Where(a => a.OrganisationId == organisationId);
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

        public IEnumerable<Branch> OnDistande(double distanceKm, Position position)
        {
            IEnumerable<Branch> branches = _context.Branches;
            return branches.Where(b => Distance(position, new Position(b.Longtitude, b.Lattitude)) <= distanceKm);
        }

        private static double Distance(Position pos1, Position pos2)
        {
            double d2r(double deg)
            {
                return deg * Math.PI / 180;
            }

            var radEarth = 6371.0d;
            var dLat = d2r(pos2.Lattitude - pos1.Lattitude);
            var dLon = d2r(pos2.Longtitude - pos1.Longtitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(d2r(pos1.Lattitude)) * Math.Cos(d2r(pos2.Lattitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return radEarth * c;
        }
    }
}
