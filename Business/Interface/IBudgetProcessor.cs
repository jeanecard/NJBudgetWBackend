using NJBudgetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Business.Interface
{
    public interface IBudgetProcessor
    {
        void ProcessBudgetSpentAndLeft(Compte compte, IEnumerable<Operation> operations, byte month, ushort year);
        void ProcessState(Compte compte, IEnumerable<Operation> operations);
    }
}
