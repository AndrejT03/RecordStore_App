using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RecordStore.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;

namespace RecordStore.Repository
{
    public class ApplicationDbContext : IdentityDbContext<RecordStoreApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }
        public virtual DbSet<RecordLabel> RecordLabels { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<RecordInShoppingCart> RecordInShoppingCarts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<RecordInOrder> RecordInOrders { get; set; }
    }
}