using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class RecordLabel : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }
        public Country? Country { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        public ICollection<Record>? Records { get; set; }
    }
}