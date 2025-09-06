using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Interface
{
    public interface IRecordLabelService
    {
        List<RecordLabel> GetAll();
        RecordLabel GetDetails(Guid? id);
        void Create(RecordLabel recordLabel);
        void Update(RecordLabel recordLabel);
        void Delete(Guid id);
    }
}