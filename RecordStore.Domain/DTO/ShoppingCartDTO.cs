using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public Guid ShoppingCartId { get; set; }
        public List<AddToCartDTO> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => (i.Price ?? 0m) * i.Quantity);

        public decimal UserLoyaltyPoints { get; set; } = 0;
        public bool PointsApplied { get; set; } = false;

        public decimal AppliedDiscount => PointsApplied ? Math.Min(TotalPrice, UserLoyaltyPoints) : 0;
        public decimal FinalPrice => TotalPrice - AppliedDiscount;
    }
}