using RecordStore.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Repository.Interface
{
    public interface IUserRepository
    {
        RecordStoreApplicationUser GetUserById(string id);
        IEnumerable<RecordStoreApplicationUser> GetAllUsers();
        void UpdateUser(RecordStoreApplicationUser user);
    }
}
