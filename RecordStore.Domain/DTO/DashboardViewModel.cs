using RecordStore.Domain.DomainModels;
using RecordStore.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalRecords { get; set; }
        public int TotalReviews { get; set; }

        public List<RecordStoreApplicationUser> AllUsers { get; set; }
        public List<Order> AllOrders { get; set; }
        public List<Record> AllRecords { get; set; }
        public List<Review> AllReviews { get; set; }
    }
}