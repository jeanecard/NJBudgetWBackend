using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business.Interface;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Repositories.Interface;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Collections.Generic;

//Au debut du oi son donne l'argent a tous les comptes (opération epargne) en cous du ois on fait les depenses.
namespace NJBudgetWBackend.Business
{
    public class BudgetProcessor : IBudgetProcessor
    {
        private readonly IAppartenanceService _apService = null;
        private readonly IStatusProcessor _statusProcessor = null;

        private BudgetProcessor()
        {
        }
        public BudgetProcessor(
            IAppartenanceService apService,
            IStatusProcessor sProcessor)
        {
            _apService = apService;
            _statusProcessor = sProcessor;
        }
        /// <summary>
        /// Calcul le budget consommé, épargné et restant sur le mois month de l'année year.
        /// </summary>
        /// <param name="compte"></param>
        /// <param name="operations"></param>
        /// <param name="month"></param>
        public void ProcessBudgetSpentAndLeft(
            out float budgetConsomme,
            out float budgetProvisonne,
            out float budgetRestant,
            out float budgetEpargne,
            out float depensePure,
            in float budgetExpected,
            IEnumerable<IOperation> operations,
            byte month,
            ushort year)
        {
            if (month == 0 || month > 12 || budgetExpected < 0)
            {
                throw new ArgumentException("Ah ah, ils vous ont refiler toutes leurs merdes");
            }
            budgetConsomme = 0;
            budgetProvisonne = 0;
            budgetEpargne = 0;
            depensePure = 0;
            budgetRestant = budgetExpected;
            bool isOperationProcessable;
            if (operations != null)
            {
                foreach (IOperation iter in operations)
                {
                    isOperationProcessable =
                        !iter.IsOperationSystem
                        &&
                        (iter.DateOperation.Month == month && iter.DateOperation.Year == year);

                    if (isOperationProcessable)
                    {
                        budgetConsomme += Math.Abs(iter.Value);
                        if (iter.Value > 0)
                        {
                            if (iter.OperationAllowed == OperationTypeEnum.EpargneAndDepense
                                || iter.OperationAllowed == OperationTypeEnum.EpargneOnly)
                            {
                                budgetEpargne += iter.Value;
                            }
                            else if (iter.OperationAllowed == OperationTypeEnum.ProvisionAndDepense
                                || iter.OperationAllowed == OperationTypeEnum.ProvisionOnly)
                            {
                                budgetProvisonne += iter.Value;
                            }
                        }
                        else
                        {
                            depensePure -= iter.Value;
                        }
                    }
                }
                budgetRestant = budgetExpected - budgetConsomme;
            }
        }

        public SyntheseDepenseGlobalModel ProcessSyntheseOperations(
            IEnumerable<SyntheseOperationRAwDB> operations,
            IEnumerable<GroupRawDB> groups,
            byte month,
            ushort year)
        {
            if (operations == null || month == 0 || month > 12 || groups == null)
            {
                return null;
            }
            Dictionary<Guid, (OperationTypeEnum, float)> operationAndBudgetMap = CreateOperationAndBudgetMap(groups);
            List<SyntheseDepenseGlobalModelItem> retourData = new();
            Dictionary<Guid, Dictionary<Guid, List<IOperation>>> operationsByCompteByAppartenance = InitOperationsByCompteByAppartenance(groups);
            List<CompteStatusEnum> statusesByCategories = new();

            //1- Regroupement des opérations apparteance et par compte
            foreach (SyntheseOperationRAwDB iter in operations)
            {
                //1.0- Ne prendre en compte que les opérations réelles de l'utilisateur
                if (!iter.IsOperationSystem)
                {

                    //1.1- Récupération ou création de la liste des opération du compte de l'appartenance
                    List<IOperation> iterOperations = operationsByCompteByAppartenance[iter.AppartenanceId][iter.CompteId];
                    //1.2- Ajout de l'opération.
                    iterOperations.Add(new BasicOperation()
                    {
                        DateOperation = iter.DateOperation,
                        Value = iter.Value,
                        OperationAllowed = iter.OperationAllowed,
                        IsOperationSystem = iter.IsOperationSystem
                    });
                }

            }
            //2- Pour chaque appartenance, calcul du budget alloué et dépensé 
            // qui correspond a la somme de chacune de ces propriétés sur les comptes de l'appartenance.
            foreach (Guid iterGuidAppartenance in operationsByCompteByAppartenance.Keys)
            {
                SyntheseDepenseGlobalModelItem syntheseAppartenance = new()
                {
                    AppartenanceId = iterGuidAppartenance,
                    AppartenanceCaption = _apService.GetById(iterGuidAppartenance)?.Caption,
                    Status = CompteStatusEnum.None,
                    BudgetPourcentageDepense = 0,
                    BudgetValueDepense = 0,
                    BudgetValuePrevu = 0
                };
                List<CompteStatusEnum> statuses = new ();
                foreach (Guid groupIterGuid in operationsByCompteByAppartenance[iterGuidAppartenance].Keys)
                {

                    ProcessBudgetSpentAndLeft(
                        out float budgetDepense,
                        out float budgetProvison,
                        out float _,
                        out float budgetEpargne,
                        out float depensePure,
                        operationAndBudgetMap[groupIterGuid].Item2,
                        operationsByCompteByAppartenance[iterGuidAppartenance][groupIterGuid],
                        month,
                        year);
                    syntheseAppartenance.BudgetValueDepense += budgetDepense;
                    syntheseAppartenance.BudgetValuePrevu += operationAndBudgetMap[groupIterGuid].Item2;
                    syntheseAppartenance.DepensePure += depensePure;
                    syntheseAppartenance.Epargne += budgetEpargne;
                    syntheseAppartenance.Provision += budgetProvison;
                    statuses.Add(_statusProcessor.ProcessState(
                        operationAndBudgetMap[groupIterGuid].Item1,
                        operationAndBudgetMap[groupIterGuid].Item2,
                        operationsByCompteByAppartenance[iterGuidAppartenance][groupIterGuid]));
                }
                syntheseAppartenance.BudgetPourcentageDepense = syntheseAppartenance.BudgetValuePrevu != 0.0f ? (syntheseAppartenance.BudgetValueDepense * 100.0f) / syntheseAppartenance.BudgetValuePrevu : 0.0f;
                syntheseAppartenance.Status = _statusProcessor.ProcessGlobal(statuses);
                syntheseAppartenance.Balance = syntheseAppartenance.BudgetValuePrevu - syntheseAppartenance.DepensePure - syntheseAppartenance.Provision - syntheseAppartenance.Epargne;
                statusesByCategories.Add(syntheseAppartenance.Status);
                retourData.Add(syntheseAppartenance);
            }
            return new SyntheseDepenseGlobalModel()
            {
                Data = retourData,
                Status = _statusProcessor.ProcessGlobalByCategories(retourData)
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        private static  Dictionary<Guid, Dictionary<Guid, List<IOperation>>> InitOperationsByCompteByAppartenance(IEnumerable<GroupRawDB> groups)
        {
            Dictionary<Guid, Dictionary<Guid, List<IOperation>>> retour = new();
            if (groups != null)
            {
                foreach (GroupRawDB iter in groups)
                {
                    if (!retour.ContainsKey(iter.AppartenanceId))
                    {
                        var iterOperations = new List<IOperation>();

                        var dic = new Dictionary<Guid, List<IOperation>>() { };
                        dic.Add(iter.Id, iterOperations);
                        retour.Add(iter.AppartenanceId, dic);
                    }
                    else
                    {
                        if (!retour[iter.AppartenanceId].ContainsKey(iter.Id))
                        {
                            var iterOperations = new List<IOperation>();
                            retour[iter.AppartenanceId].Add(iter.Id, iterOperations);
                        }
                    }
                }
            }
            return retour;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        private static Dictionary<Guid, (OperationTypeEnum, float)> CreateOperationAndBudgetMap(IEnumerable<GroupRawDB> groups)
        {
            Dictionary<Guid, (OperationTypeEnum, float)> retour = new();
            if (groups != null)
            {
                foreach (GroupRawDB iter in groups)
                {
                    if (!retour.ContainsKey(iter.Id))
                    {
                        retour.Add(iter.Id, (iter.OperationAllowed, iter.BudgetExpected));
                    }
                }
            }
            return retour;
        }

    }
}
