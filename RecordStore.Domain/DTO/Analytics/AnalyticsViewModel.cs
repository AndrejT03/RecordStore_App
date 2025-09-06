using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Analytics
{
    public class AnalyticsViewModel
    {
        public IEnumerable<string> SalesChartLabels { get; set; }
        public IEnumerable<decimal> SalesChartData { get; set; }
        public List<BestSellingRecordViewModel> BestSellingRecords { get; set; }
        public List<GenrePerformanceViewModel> GenrePerformance { get; set; }
    }
}