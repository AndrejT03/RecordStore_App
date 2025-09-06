using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class RecordInOrder : BaseEntity
    {
        public Guid RecordId { get; set; }
        public Record? Record { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; } 
        public int Quantity { get; set; }
    }
}