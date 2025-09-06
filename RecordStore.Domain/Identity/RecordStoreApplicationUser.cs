using Microsoft.AspNetCore.Identity;
using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.Identity
{
    public class RecordStoreApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public ShoppingCart? UserCart { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal LoyaltyPoints { get; set; } = 0;
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}