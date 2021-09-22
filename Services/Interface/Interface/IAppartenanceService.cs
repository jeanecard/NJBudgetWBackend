using NJBudgetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface.Interface
{
    public interface IAppartenanceService
    {
        Task<Appartenance> GetAsync(Guid id);
        Task<IEnumerable<Appartenance>> GetAsync();
    }
}
