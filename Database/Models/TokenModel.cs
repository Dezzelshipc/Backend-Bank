using Microsoft.EntityFrameworkCore;

namespace Database.Models
{
    public interface Token
    {
        public const string Access = "access";
        public const string Refresh = "refresh";
    }

    [Index(nameof(ObjectId), nameof(Type), IsUnique = true)]
    public class TokenModel : IModel
    {
        public TokenModel(int id, int objectId, ObjectType type, string token)
        {
            Id = id;
            ObjectId = objectId;
            Type = type;
            Token = token;
        }

        public TokenModel(int objectId, ObjectType type, string token) : this(0, objectId, type, token) { }

        public int Id { get; set; }
        public int ObjectId { get; set; }
        public ObjectType Type { get; set; }
        public string Token { get; set; }
    }

    public class TokenPair<T> where T : class
    {
        public T Access;
        public T Refresh;

        public TokenPair(T access, T refresh)
        {
            Access = access;
            Refresh = refresh;
        }
    }
}
