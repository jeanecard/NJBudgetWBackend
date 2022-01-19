using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface
{
    public interface IModelJammer
    {
        SyntheseMoisModel Jam(SyntheseMoisModel model);
        SyntheseDepenseByAppartenanceModel Jam(SyntheseDepenseByAppartenanceModel model);
        SyntheseDepenseGlobalModel Jam(SyntheseDepenseGlobalModel model);
        Compte Jam(Compte moel);
    }
}
