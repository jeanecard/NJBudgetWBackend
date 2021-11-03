using NJBudgetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Models
{
    public class CumulativeOperation
    {
        public Guid? GroupId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double Value { get; set; }

        public CompteStatusEnum Status { get; set; }
    }
}
