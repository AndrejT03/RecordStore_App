using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class Country : BaseEntity
    {
        public string? Name { get; set; } 
        public string? Flag { get; set; }
        public string? Code1 { get; set; }
        public string? Code2 { get; set; }
        public string? Capital { get; set; }
        public string? Region { get; set; }
        public virtual ICollection<Artist>? Artists { get; set; } 
    }
}