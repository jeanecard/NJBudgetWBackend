using NJBudgetWBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface.Interface
{
    public interface ISyntheseService
    {
        Task<SyntheseDepenseGlobalModel> GetSyntheseByMonthByAppartenanceAsync(DateTime input);
        Task<SyntheseDepenseByAppartenanceModel> GetSyntheseByMonthForAppartenanceAsync(Guid appartenanceId, DateTime inputDate);
        Task<SyntheseMoisModel> GetSyntheseGlobalMonth(DateTime inputDate);
    }
}
