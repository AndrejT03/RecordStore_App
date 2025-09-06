using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class Artist : BaseEntity
    {
        public string? Name { get; set; }
        public string? Picture { get; set; }
        public string? Bio { get; set; }
        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }
        public virtual ICollection<Record>? Records { get; set; }
    }
}