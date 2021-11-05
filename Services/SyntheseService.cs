using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business;
using NJBudgetWBackend.Business.Interface;
using NJBudgetWBackend.Commun;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Repositories.Interface;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class SyntheseService : ISyntheseService
    {
        private IBudgetProcessor _budgetBusiness = null;
        private IGroupRepository _groupRepo = null;
        private IOperationsRepository _opeRepo = null;
        private IAppartenanceService _apService = null;
        private IStatusProcessor _statusProcessor = null;
        private SyntheseService()
        {

        }
        public SyntheseService(
            IBudgetProcessor processor,
            IGroupRepository groupRepo,
            IOperationsRepository opeRepo,
            IAppartenanceService apService,
            IStatusProcessor statusProcessor)
        {
            _budgetBusiness = processor;
            _groupRepo = groupRepo;
            _opeRepo = opeRepo;
            _apService = apService;
            _statusProcessor = statusProcessor;
        }
        /// <summary>
        /// 1- Récupération des opérations sur le mois
        /// 2- Mise à jour de la synthese par Appartenance
        /// 3- Mise à jour du status global
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<SyntheseDepenseGlobalModel> GetSyntheseByMonthByAppartenanceAsync(DateTime date)
        {
            //1-
            var days = Tools.GetFirstAndLastDayMonthOfThisYear((byte)(date.Month));
            if (days == null)
            {
                return null;
            }
            SyntheseDepenseGlobalModel retour = new SyntheseDepenseGlobalModel();
            using var opeTask = _opeRepo.GetOperationsAsync(days.Value.Item1, date);
            await opeTask;
            if (opeTask.IsCompletedSuccessfully)
            {
                using var groupsTask = _groupRepo.GetGroupsAsync();
                await groupsTask;
                if (groupsTask.IsCompletedSuccessfully)
                {
                    retour = _budgetBusiness.ProcessSyntheseOperations(
                        opeTask.Result,
                        groupsTask.Result,
                        (byte)(date.Month), 
                        (ushort)date.Year);
                }
            }
            return retour;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appartenanceId"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<SyntheseDepenseByAppartenanceModel> GetSyntheseByMonthForAppartenanceAsync(Guid appartenanceId, DateTime date)
        {
            if (appartenanceId == Guid.Empty)
            {
                return null;
            }
            var days = Tools.GetFirstAndLastDayMonthOfThisYear((byte)(date.Month));

            if (days == null)
            {
                return null;
            }
            SyntheseDepenseByAppartenanceModel retour = new SyntheseDepenseByAppartenanceModel();
            retour.AppartenanceId = appartenanceId;

            var appartenance = _apService.GetById(appartenanceId);
            if (appartenance != null)
            {
                retour.AppartenanceCaption = appartenance.Caption;
                Task<IEnumerable<GroupRawDB>> groupsTask = _groupRepo.GetGroupsByAppartenanceAsync(appartenanceId);
                await groupsTask;
                if (groupsTask.IsCompletedSuccessfully)
                {
                    List<SyntheseDepenseByAppartenanceModelItem> items = new List<SyntheseDepenseByAppartenanceModelItem>();
                    foreach (GroupRawDB iter in groupsTask.Result)
                    {
                        Task<IEnumerable<Operation>> operationTask = _opeRepo.GetOperationsAsync(
                            iter.Id, 
                            days.Value.Item1, 
                            date,
                            iter.OperationAllowed);
                        await operationTask;
                        if (operationTask.IsCompletedSuccessfully)
                        {

                            float budgetDepense = 0, budgetProvisonne = 0, budgetRestant = 0, budgetEpargne = 0 , depensePure = 0;
                            _budgetBusiness.ProcessBudgetSpentAndLeft(
                                out budgetDepense,
                                out budgetProvisonne,
                                out budgetRestant,
                                out budgetEpargne,
                                out depensePure,
                                iter.BudgetExpected,
                                operationTask.Result,
                                (byte)(date.Month),
                                (ushort)date.Year);
                            var item = new SyntheseDepenseByAppartenanceModelItem()
                            {
                                BudgetPourcentageDepense = 0,
                                BudgetValueDepense = budgetDepense,
                                BudgetValuePrevu = iter.BudgetExpected,
                                GroupCaption = iter.Caption,
                                GroupId = iter.Id,
                                Status = _statusProcessor.ProcessState(iter.OperationAllowed, iter.BudgetExpected, operationTask.Result),
                                DepensePure = depensePure,
                                Epargne = budgetEpargne,
                                Provision = budgetProvisonne,
                                Balance = iter.BudgetExpected - depensePure - budgetProvisonne + budgetEpargne
                        };
                            items.Add(item);
                        }
                        else
                        {
                            throw new Exception("Quoi ???");
                        }
                    }
                    retour.Data = items;
                }
                else
                {
                    throw new Exception("C'était pas ma guerre !!");
                }
            }
            else
            {
                throw new Exception("Mais pour qui le prenez vous, Dieu ? ...Non, dieu aurait pitié, pas lui.");
            }
            return retour;
        }

        public async Task<SyntheseMoisModel> GetSyntheseGlobalMonth(DateTime inputDate)
        {
            SyntheseMoisModel retour = new SyntheseMoisModel();

            var globalTask = GetSyntheseByMonthByAppartenanceAsync(inputDate);
            await globalTask;
            if(globalTask.IsCompletedSuccessfully)
            {
                foreach (SyntheseDepenseGlobalModelItem iterator in globalTask.Result.Data)
                {
                    retour.BudgetValueDepense += iterator.BudgetValueDepense;
                    retour.BudgetValuePrevu += iterator.BudgetValuePrevu;
                    retour.Provision += iterator.Provision;
                    retour.DepensePure += iterator.DepensePure;
                    retour.Epargne += iterator.Epargne;
                }
                retour.Status = globalTask.Result.Status;
                retour.Balance = retour.BudgetValuePrevu - retour.DepensePure - retour.Provision + retour.Epargne; 
            }
            else
            {
                throw new Exception("Ludicrious speed !!");
            }
            return retour;
        }
    }
}