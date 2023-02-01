using Database.Models;

namespace Database.Interfaces
{
    public interface IUsersRepository : IRepository<UserModel>
    {
        public UserModel? GetUserByLogin(string login);
    }
}
