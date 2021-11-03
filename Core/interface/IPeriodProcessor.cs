using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Business.Interface
{
    public interface IPeriodProcessor
    {
        void ProcessRangeBeforeTo(DateTime to, byte nbMonthInRange , out DateTime from);
    }
}
