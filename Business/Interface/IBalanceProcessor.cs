using NJBudgetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Business.Interface
{
    public interface IBalanceProcessor
    {
        void ProcessBalance(
    out float balance,
    float budgetExpectedByMonth,
    in IEnumerable<IOperation> operations,
    in DateTime? processDateConsideration = null);
    }
}
