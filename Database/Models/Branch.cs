using Microsoft.EntityFrameworkCore;

namespace Database.Models
{
    [Index(nameof(OrganisationId), nameof(Name), IsUnique = true)]
    public class Branch
    {
        public Branch(int id, int organisationId, string name, string address, string phoneNumber, double longtitude, double lattitude)
        {
            Id = id;
            OrganisationId = organisationId;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Longtitude = longtitude;
            Lattitude = lattitude;
        }

        public Branch(int organisationId, string name, string address, string phoneNumber, double longtitude, double lattitude)
            : this(0, organisationId, name, address, phoneNumber, longtitude, lattitude) { }

        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public double Longtitude { get; set; }
        public double Lattitude { get; set; }
    }

    public class Position
    {
        public double Longtitude { get; set; }
        public double Lattitude { get; set; }

        public Position(double longtitude, double lattitude)
        {
            Longtitude = longtitude;
            Lattitude = lattitude;
        }

        public bool IsValid()
        {
            return Math.Abs(Longtitude) <= 180 &&
                Math.Abs(Lattitude) <= 90;
        }
    }
}
