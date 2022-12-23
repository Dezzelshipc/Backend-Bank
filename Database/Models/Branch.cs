using Microsoft.EntityFrameworkCore;

namespace Database.Models
{
    [Index(nameof(OrganisationId), nameof(Name), IsUnique = true)]
    public class Branch
    {
        public Branch(int organisationId, string name, string address, string phoneNumber)
        {
            OrganisationId = organisationId;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }

        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
