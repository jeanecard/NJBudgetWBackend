using NJBudgetWBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface
{
    public interface ICumulativeService
    {
        Task<CumulativeOperation> GetAllCompteCumulAsync(byte forLastnMonths);
        Task<CumulativeOperation> GetCompteCumulAsync(Guid groupId, byte forLastnMonths);
    }
}
