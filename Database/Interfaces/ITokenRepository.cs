using Database.Models;

namespace Database.Interfaces
{
    public interface ITokenRepository : IRepository<TokenModel>
    {
        TokenModel? GetTokenById(int objectId, ObjectType objectType);
    }
}
