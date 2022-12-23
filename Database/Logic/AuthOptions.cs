using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Database.Logic
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFE_ACCESS = 1; // время жизни токена - 1 минута
        public const int LIFE_REFRESH = 7; // время жизни токена - 7 дней 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}