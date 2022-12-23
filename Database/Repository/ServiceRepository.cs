using Database.Interfaces;
using Database.Models;

namespace Database.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationContext _context;

        public ServiceRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool Create(Service item)
        {
            _context.Services.Add(item);
            return true;
        }

        public bool Delete(int id)
        {
            var serv = GetItem(id);
            if (serv == default)
                return false;

            _context.Services.Remove(serv);
            return true;
        }

        public int Find(Service service)
        {
            return _context.Services.FirstOrDefault(a => a.Name == service.Name && a.OrganisationId == service.OrganisationId).Id;
        }

        public IEnumerable<Service> GetAll()
        {
            return _context.Services;
        }

        public Service? GetItem(int id)
        {
            return _context.Services.FirstOrDefault(a => a.Id == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Update(Service item)
        {
            _context.Services.Update(item);
            return true;
        }
    }
}
