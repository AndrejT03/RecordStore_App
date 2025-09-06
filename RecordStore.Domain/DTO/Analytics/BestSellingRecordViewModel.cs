using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Analytics
{
    public class BestSellingRecordViewModel
    {
        public Guid RecordId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public int UnitsSold { get; set; }
    }
}