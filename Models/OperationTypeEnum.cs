using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetBackEnd.Models
{
    public enum OperationTypeEnum
    {
        ProvisionOnly = 1,
        DepenseOnly = 2,
        ProvisionAndDepense = 3,
        None = 4,
        EpargneAndDepense = 5,
        EpargneOnly = 6,
    }
}
