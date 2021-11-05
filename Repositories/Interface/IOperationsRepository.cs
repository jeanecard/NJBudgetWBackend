using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Repositories.Interface
{
    public interface IOperationsRepository
    {
        Task<IEnumerable<Operation>> GetOperationsAsync(Guid compteId, DateTime? from, DateTime? to, OperationTypeEnum opeType);
        Task<IEnumerable<SyntheseOperationRAwDB>> GetOperationsAsync(DateTime? from, DateTime? to);

        Task InsertAsync(Operation op);
        Task DeleteAsync(Guid idOperation);
        Task<Guid> GetCompteOperationAsync(Guid operationid);
    }
}
