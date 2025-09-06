using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Repository.Interface;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Implementation
{
    public class RecordLabelService : IRecordLabelService
    {
        private readonly IRepository<RecordLabel> _recordLabelRepository;

        public RecordLabelService(IRepository<RecordLabel> recordLabelRepository)
        {
            _recordLabelRepository = recordLabelRepository;
        }
        public List<RecordLabel> GetAll()
        {
            return _recordLabelRepository.GetAll(selector: r => r,
                                            include: r => r
                                            .Include(x => x.Country)).ToList();
        }

        public RecordLabel GetDetails(Guid? id)
        {
            return _recordLabelRepository.Get(
                selector: r => r,
                predicate: r => r.Id == id,
                include: r => r
                    .Include(x => x.Country)
            );
        }

        public void Create(RecordLabel recordLabel)
        {
            _recordLabelRepository.Insert(recordLabel);
        }

        public void Update(RecordLabel recordLabel)
        {
            _recordLabelRepository.Update(recordLabel);
        }

        public void Delete(Guid id)
        {
            var recordLabel = GetDetails(id) ?? throw new Exception("Record not found");
            _recordLabelRepository.Delete(recordLabel);
        }
    }
}