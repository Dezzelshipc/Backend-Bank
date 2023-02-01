using Database.Models;

namespace Backend_Bank.Converters
{

    public static class StringObjectTypeConverter
    {
        public static string ToString(this ObjectType type)
        {
            return type switch
            {
                ObjectType.Organisation => "1",
                _ => "0",
            };
        }

        public static ObjectType ToObjectType(this string str)
        {
            return str switch
            {
                "1" => ObjectType.Organisation,
                _ => ObjectType.User,
            };
        }
    }
}
