using RecordStore.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class Review : BaseEntity
    {
        [Range(1, 5, ErrorMessage = "Тhe rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime? DateModified { get; set; }

        public Guid RecordId { get; set; }
        public Record? Record { get; set; }

        public string? UserId { get; set; }
        public RecordStoreApplicationUser? User { get; set; }
    }
}