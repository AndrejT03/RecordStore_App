using RecordStore.Domain.DTO.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
