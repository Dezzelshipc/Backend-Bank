using Database.Models;
using System.Runtime.CompilerServices;

namespace Database.Interfaces
{
    public interface IUsersRepository : IRepository<UserModel>
    {
        public UserModel? GetUserByLogin(string login);
        public UserModel? GetUserByPhone(string phone);
    }
}
