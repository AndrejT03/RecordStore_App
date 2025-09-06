using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.Identity;
using RecordStore.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<RecordStoreApplicationUser> entites;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            this.entites = _context.Set<RecordStoreApplicationUser>();
        }

        public RecordStoreApplicationUser GetUserById(string id)
        {
            return entites.First(ent => ent.Id == id);
        }
        public IEnumerable<RecordStoreApplicationUser> GetAllUsers()
        {
            return entites.ToList(); 
        }
        public void UpdateUser(RecordStoreApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            entites.Update(user);
            _context.SaveChanges();
        }
    }
}