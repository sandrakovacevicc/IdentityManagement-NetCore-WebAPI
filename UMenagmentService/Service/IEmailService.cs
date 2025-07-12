using UserManagment.Service.Models;

namespace UserManagment.Service.Service
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
