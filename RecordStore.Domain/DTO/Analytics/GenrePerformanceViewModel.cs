using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Analytics
{
    public class GenrePerformanceViewModel
    {
        public string Genre { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}