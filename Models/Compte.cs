using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetBackEnd.Models
{
    public class Compte
    {
        public Group Group { get; set; }
        public float BudgetExpected { get; set; }
        public float BudgetSpent { get; set; }
        public float BudgetLeft { get; set; }
        public IEnumerable<Operation> Operations { get; set; }
        public CompteStatusEnum State { get; set; }

        public OperationTypeEnum OperationAllowed { get; set; }
    }


}
