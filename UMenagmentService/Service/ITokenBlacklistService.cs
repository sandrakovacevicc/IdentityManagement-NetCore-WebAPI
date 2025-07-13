using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagment.Service.Service
{
    public interface ITokenBlacklistService
    {
        Task BlacklistTokenAsync(string token, TimeSpan expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}
