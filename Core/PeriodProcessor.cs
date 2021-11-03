using NJBudgetWBackend.Business.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Business
{
    public class PeriodProcessor : IPeriodProcessor
    {
        public void ProcessRangeBeforeTo(DateTime to, byte nbMonthInRange, out DateTime from)
        {
            if (nbMonthInRange == 0)
            {
                from = new DateTime(to.Year, to.Month, 1);
            }
            else
            {
                from = new DateTime(to.Year, to.Month, 1).AddMonths(-nbMonthInRange);
            }
        }
    }
}
