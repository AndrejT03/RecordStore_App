using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class AddTracklistDTO
    {
        [Required]
        public Guid RecordId { get; set; }

        public List<Track> Tracks { get; set; } = new List<Track>();
    }
}