namespace Database.Models
{
    public class UserModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public UserModel(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
