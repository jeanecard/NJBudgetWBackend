using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface.Interface
{
    public interface IAuthZService
    {
        bool IsAuthZ(string login);
    }
}
