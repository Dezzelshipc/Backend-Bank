using Microsoft.EntityFrameworkCore;

namespace Database.Models
{
    [Index(nameof(OrganisationId), nameof(Name), IsUnique = true)]
    public class Service : IModel
    {
        public Service(int organisationId, string name, string description, string percent, string minLoanPeriod, string maxLoadPeriod, bool isOnline)
        {
            OrganisationId = organisationId;
            Name = name;
            Description = description;
            Percent = percent;
            MinLoanPeriod = minLoanPeriod;
            MaxLoadPeriod = maxLoadPeriod;
            IsOnline = isOnline;
        }

        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Percent { get; set; }
        public string MinLoanPeriod { get; set; }
        public string MaxLoadPeriod { get; set; }
        public bool IsOnline { get; set; }
    }
}
