using Microsoft.Extensions.Configuration;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class AuthZService : IAuthZService
    {
        IConfiguration _configuration = null;

        private AuthZService()
        {

        }
        public AuthZService(IConfiguration conf)
        {
            _configuration = conf;

        }
        public bool IsAuthZ(string login)
        {
            if(String.IsNullOrEmpty(login))
            {
                return false;
            }
            var upperedLoggin = login.ToUpper();
            if (upperedLoggin == _configuration["users:1"]?.ToUpper() ||
                upperedLoggin == _configuration["users:2"]?.ToUpper())
            {
                return true;
            }

            return false;
        }
    }
}
