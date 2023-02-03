using Database.Interfaces;
using Database.Models;

namespace Database.Repository
{

    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationContext _context;

        public TokenRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Create(TokenModel item)
        {
            _context.Tokens.Add(item);
        }

        public bool Delete(int id)
        {
            var org = GetItem(id);
            if (org == default)
                return false;

            _context.Tokens.Remove(org);
            return true;
        }

        public IEnumerable<TokenModel> GetAll()
        {
            return _context.Tokens;
        }

        public TokenModel? GetItem(int id)
        {
            return _context.Tokens.FirstOrDefault(a => a.Id == id);
        }

        public TokenModel? GetTokenById(int objectId, ObjectType objectType)
        {
            return _context.Tokens.FirstOrDefault(a => a.ObjectId == objectId && a.Type == objectType);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Update(TokenModel item)
        {
            _context.Tokens.Update(item);
            return true;
        }
        public bool Delete(TokenModel item)
        {
            if (item == null)
                return false;

            _context.Tokens.Remove(item);
            return true;
        }
    }
}
