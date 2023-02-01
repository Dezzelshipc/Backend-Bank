using Database.Migrations;

namespace Database.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }


        public UserModel(string login, string password) : this(0, login, password, "" , "", "") { }
        public UserModel(string login, string password, string phone, string email, string fullName) : this(0, login, password, phone, email, fullName) { }
        public UserModel(int id, string login, string password, string phone, string email, string fullName)
        {
            Id = id;
            Login = login;
            Password = password;
            Phone = phone;
            Email = email;
            FullName = fullName;
        }

        public bool IsValid()
        {
            return !(Id < 0 ||
                string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(Phone) ||
                string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(FullName));
        }

        public bool IsSemiValid()
        {
            return !(Id < 0 ||
                string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Password));
        }
    }
}
