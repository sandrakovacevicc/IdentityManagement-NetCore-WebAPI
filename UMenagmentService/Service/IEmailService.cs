using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMenagmentService.Models;

namespace UMenagmentService.Service
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
