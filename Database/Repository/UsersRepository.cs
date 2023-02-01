using Database.Interfaces;
using Database.Models;

namespace Database.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationContext _context;

        public UsersRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Create(UserModel item)
        {
            _context.Users.Add(item);
        }

        public bool Delete(int id)
        {
            var org = GetItem(id);
            if (org == default)
                return false;

            _context.Users.Remove(org);
            return true;
        }

        public IEnumerable<UserModel> GetAll()
        {
            return _context.Users;
        }

        public UserModel? GetItem(int id)
        {
            return _context.Users.FirstOrDefault(a => a.Id == id);
        }

        public UserModel? GetUserByLogin(string login)
        {
            return _context.Users.FirstOrDefault(a => a.Login == login);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Update(UserModel item)
        {
            _context.Users.Update(item);
            return true;
        }
    }
}
