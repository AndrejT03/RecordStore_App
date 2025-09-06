using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class HomeViewModel
    {
        public List<Record> Records { get; set; }
        public List<Artist> Artists { get; set; }
    }
}
