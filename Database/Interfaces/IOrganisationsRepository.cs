using Database.Models;

namespace Database.Interfaces
{
    public interface IOrganisationsRepository : IRepository<Organisation>
    {
        public Organisation? GetOrganisationByLogin(string login);
    }
}
