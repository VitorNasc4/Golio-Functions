using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolioFunctions.Services
{
    public interface IEmailService
    {
        public void SendEmail(string email, string subject, string body);
    }
}