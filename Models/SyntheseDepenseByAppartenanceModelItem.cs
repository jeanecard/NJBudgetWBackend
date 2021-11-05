using NJBudgetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Models
{
    public class SyntheseDepenseByAppartenanceModelItem
    {
        public Guid GroupId { get; set; }
        public String GroupCaption { get; set; }

        public float BudgetValuePrevu { get; set; }
        public float Epargne { get; set; }
        public float Provision { get; set; }
        public float DepensePure { get; set; }
        public float Balance { get; set; }

        public float BudgetValueDepense { get; set; }
        public float BudgetPourcentageDepense { get; set; }
        public CompteStatusEnum Status { get; set; }
    }
}
