using Microsoft.EntityFrameworkCore;

namespace Database.Models
{
    [Index(nameof(OrganisationId), nameof(Name), IsUnique = true)]
    public class Branch
    {
        public Branch(int id, int organisationId, string name, string address, string phoneNumber, double longtitude, double latitude)
        {
            Id = id;
            OrganisationId = organisationId;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Longtitude = longtitude;
            Latitude = latitude;
        }

        public Branch(int organisationId, string name, string address, string phoneNumber, double longtitude, double latitude)
            : this(0, organisationId, name, address, phoneNumber, longtitude, latitude) { }

        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
    }

    public class Position
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }

        public Position(double longtitude, double latitude)
        {
            Longtitude = longtitude;
            Latitude = latitude;
        }

        public Position() { }

        public bool IsValid()
        {
            return Math.Abs(Longtitude) <= 180 &&
                Math.Abs(Latitude) <= 90;
        }
    }
}
