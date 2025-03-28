using UMenagmentService.Models;

namespace UMenagmentService.Service
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
