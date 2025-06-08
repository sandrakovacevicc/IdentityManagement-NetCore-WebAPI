using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagment.Service.Models;

namespace UserManagment.Service.Service
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
