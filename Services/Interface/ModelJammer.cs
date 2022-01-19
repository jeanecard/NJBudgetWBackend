using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface
{
    public class ModelJammer : IModelJammer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">useless in v1</param>
        /// <returns></returns>
        public SyntheseMoisModel Jam(SyntheseMoisModel source)
        {
            var randomizer = new Random();
            SyntheseMoisModel retour = new SyntheseMoisModel();
            retour.Balance = (float)(randomizer.NextDouble() * 150);
            retour.BudgetValueDepense = (float)(randomizer.NextDouble() * 250);
            retour.BudgetValuePrevu = (float)(randomizer.NextDouble() * 400);
            retour.DepensePure = (float)(randomizer.NextDouble() * 100);
            retour.Epargne = (float)(randomizer.NextDouble() * 50);
            retour.Provision = (float)(randomizer.NextDouble() * 50);
            retour.Status = NJBudgetBackEnd.Models.CompteStatusEnum.Danger;

            return retour;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SyntheseDepenseByAppartenanceModel Jam(SyntheseDepenseByAppartenanceModel model)
        {
            var randomizer = new Random();
            SyntheseDepenseByAppartenanceModel retour = new SyntheseDepenseByAppartenanceModel();
            retour.AppartenanceCaption = model.AppartenanceCaption;
            retour.AppartenanceId = model.AppartenanceId;
            var data = new List<SyntheseDepenseByAppartenanceModelItem>();
            foreach(SyntheseDepenseByAppartenanceModelItem iter in model.Data)
            {
                data.Add(new SyntheseDepenseByAppartenanceModelItem()
                {
                    Balance = (float)(randomizer.NextDouble() * 150),
                    BudgetPourcentageDepense = (float)(randomizer.NextDouble() * 100),
                    BudgetValueDepense = (float)(randomizer.NextDouble() * 300),
                    BudgetValuePrevu = (float)(randomizer.NextDouble() * 450),
                    DepensePure = (float)(randomizer.NextDouble() * 150),
                    Epargne = (float)(randomizer.NextDouble() * 150),
                    GroupCaption = iter.GroupCaption,
                    GroupId = iter.GroupId,
                    Provision = (float)(randomizer.NextDouble() * 150),
                    Status = iter.Status
                });

            }
            retour.Data = data;
            return retour;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SyntheseDepenseGlobalModel Jam(SyntheseDepenseGlobalModel model)
        {
            var randomizer = new Random();

            SyntheseDepenseGlobalModel retour = new SyntheseDepenseGlobalModel();
            var data = new List<SyntheseDepenseGlobalModelItem>();
            foreach (SyntheseDepenseGlobalModelItem iter in model.Data)
            {
                data.Add(new SyntheseDepenseGlobalModelItem()
                {
                    AppartenanceCaption = iter.AppartenanceCaption,
                    AppartenanceId = iter.AppartenanceId,
                    Balance = -(float)(randomizer.NextDouble() * 150),
                    BudgetPourcentageDepense = (float)(randomizer.NextDouble() * 100),
                    BudgetValueDepense = (float)(randomizer.NextDouble() * 250),
                    BudgetValuePrevu = (float)(randomizer.NextDouble() * 550),
                    DepensePure = (float)(randomizer.NextDouble() * 150),
                    Epargne = (float)(randomizer.NextDouble() * 150),
                    Provision = (float)(randomizer.NextDouble() * 150),
                    Status = iter.Status
                });

            }
            retour.Data = data;
            retour.Status = model.Status;
            return retour;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Compte Jam(Compte model)
        {
            Compte retour = new Compte();
            var randomizer = new Random();

            retour.Balance = (float)(randomizer.NextDouble() * 150);
            retour.BudgetConsummed = (float)(randomizer.NextDouble() * 450);
            retour.BudgetExpected = (float)(randomizer.NextDouble() * 650);
            retour.BudgetLeft = -(float)(randomizer.NextDouble() * 150);
            retour.BudgetProvision=  (float)(randomizer.NextDouble() * 150);
            retour.Group = model.Group;
            retour.OperationAllowed = model.OperationAllowed;
                retour.Operations = model.Operations;
            retour.State = model.State;

            return retour;

        }
    }
}
