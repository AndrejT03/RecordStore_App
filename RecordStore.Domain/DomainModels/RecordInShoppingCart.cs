using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class RecordInShoppingCart : BaseEntity
    {
        public Guid RecordId { get; set; }
        public Record? Record { get; set; } 
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
        public int Quantity { get; set; }
    }
}
