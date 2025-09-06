using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class AddToCartDTO
    {
        public Guid RecordId { get; set; }
        public string? Title { get; set; }
        public string? CoverURL { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
    }
}