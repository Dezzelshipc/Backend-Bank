using Database.Models;

namespace Database.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        public int Find(Service service);
        public IEnumerable<Service> GetServices(int organisationId);
    }
}
