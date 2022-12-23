namespace Database.Models
{
    public class TokenModel
    {
        public TokenModel(string? value, bool type)
        {
            Value = value;
            Type = type;
        }

        public string? Value { get; set; }
        public bool Type { get; set; }
    }
}
