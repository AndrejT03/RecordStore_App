using RecordStore.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string? OwnerId { get; set; }
        public RecordStoreApplicationUser? Owner { get; set; }
        public ICollection<RecordInShoppingCart> AllRecords { get; set; } = new List<RecordInShoppingCart>();
    }
}
