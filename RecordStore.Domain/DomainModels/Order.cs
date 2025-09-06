using RecordStore.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string? OwnerId { get; set; }
        public RecordStoreApplicationUser? Owner { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public ICollection<RecordInOrder>? RecordsInOrder { get; set; }
    }
}
