﻿using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Models;
using System;
using System.Collections.Generic;

namespace NJBudgetWBackend.Business.Interface
{
    public interface IBudgetProcessor
    {

        void ProcessBudgetSpentAndLeft(
            out float budgetConsomme,
            out float budgetProvisonne,
            out float budgetRestant,
            out float budgetEpargne,
            out float depensePure,
            in float budgetExpected,
            IEnumerable<IOperation> operations,
            byte month,
            ushort year);

        SyntheseDepenseGlobalModel ProcessSyntheseOperations(
            IEnumerable<SyntheseOperationRAwDB> operations,
            IEnumerable<GroupRawDB> comptes,
            byte month,
            ushort year);



    }
}
