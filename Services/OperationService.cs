using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business.Interface;
using NJBudgetWBackend.Repositories.Interface;
using NJBudgetWBackend.Services.Interface;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class OperationService : IOperationService
    {
        private readonly IOperationsRepository _opeRepo = null;
        private readonly IGroupService _groupService = null;
        private OperationService()
        {

        }

        public OperationService(
            IOperationsRepository repo,
            IGroupService groupService)
        {
            _opeRepo = repo;
            _groupService = groupService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task AddAsync(Operation operation)
        {
            if (operation == null)
            {
                return;
            }
            operation.Value = Math.Abs(operation.Value);
            using var addTask = _opeRepo.InsertAsync(operation);
            await addTask;
            if (!addTask.IsCompletedSuccessfully)
            {
                throw new Exception("Pourquoi pas une laisse dans le cul, ça irait plus vite !");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationid"></param>
        /// <returns></returns>
        public async Task<Guid> DeleteAsync(Guid operationid)
        {
            if (operationid == Guid.Empty)
            {
                return Guid.Empty;
            }
            using Task<Guid> compteTask = _opeRepo.GetCompteOperationAsync(operationid);
            await compteTask;
            if (compteTask.IsCompletedSuccessfully)
            {
                using var deleteTask = _opeRepo.DeleteAsync(operationid);
                await deleteTask;
                if (!deleteTask.IsCompletedSuccessfully)
                {
                    throw new Exception("Il nous faudrait un plus gros bateau");
                }
                return compteTask.Result;
            }
            else
            {
                throw new Exception("En vrai ?");
            }
        }
        /// <summary>
        /// 1- Obtention de l'état du compte
        /// 2- Si le compte a le budget restant suffisant pour la dépense, on fait l'operation directement.
        /// 3- Sinon
        ///     3.1- Si le budget restant est positif, réalisation d'un opération de retrait avec ce reste
        ///     3.2- Si la balance est positive, 
        ///         3.2.1- Si la balance est suffisante pour couvrir le RAV, réalisation d'un opération SYSTEM de retrait avec le RAV
        ///             et mà0 du resteAVerser
        ///         3.2.2- Sinon réalisation d'un opération SYSTEM de retrait avec la balance et décrementation du RAV
        /// 4- Si il reste à verser n'est pas nul, on fait une dernière opération pour couvrir la somme due.
        /// 5- Lancement des serialisation des opérations recueillies précedement.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Les opérations ayant servies à décomposer l'opération demandée.</returns>
        public async Task<IEnumerable<IOperation>> RemoveAsync(Operation operation)
        {
            if (operation == null)
            {
                return null;
            }
            //1- 
            DateTime now = DateTime.Now;
            using var compteTask = _groupService.GetCompteAsync(operation.CompteId, (byte)now.Month, (ushort)now.Year);
            await compteTask;
            List<Operation> operations = new();
            //2- 
            if (compteTask.Result.BudgetLeft > 0 && compteTask.Result.BudgetLeft >= Math.Abs(operation.Value))
            {
                operations.Add(operation);
            }
            //3- 
            else
            {
                float valeurRestantACouvrir = Math.Abs(operation.Value);
                //3.1- 
                float balance = compteTask.Result.Balance; //bad idea, we need to reprocess balance
                if (compteTask.Result.BudgetLeft > 0)
                {
                    valeurRestantACouvrir -= compteTask.Result.BudgetLeft;
                    Operation operationDuMoisPossible = new (operation);
                    operationDuMoisPossible.Value = -compteTask.Result.BudgetLeft;
                    operations.Add(operationDuMoisPossible);
                    balance -= compteTask.Result.BudgetLeft;//bad idea, we need to reprocess balance .. on refait le boumit du balanceProcessor la c'est pas bon.
                }

                //3.2-
                if (balance > 0)
                {
                    //3.2.1-
                    if (balance >= valeurRestantACouvrir)
                    {
                        Operation operationBalanceComplete = new (operation);
                        operationBalanceComplete.Value = - Math.Abs(valeurRestantACouvrir);
                        operationBalanceComplete.IsOperationSystem = true;
                        operations.Add(operationBalanceComplete);
                        valeurRestantACouvrir = 0;
                    }
                    //3.2.2-
                    else
                    {
                        Operation operationBalancePartielle = new (operation);
                        operationBalancePartielle.Value = -(balance);
                        operationBalancePartielle.IsOperationSystem = true;
                        operations.Add(operationBalancePartielle);
                        valeurRestantACouvrir -= balance;

                    }
                }
                //4-
                if (valeurRestantACouvrir > 0)
                {
                    Operation operationRestantACouvrir = new (operation);
                    operationRestantACouvrir.Value = -valeurRestantACouvrir;
                    operations.Add(operationRestantACouvrir);
                    valeurRestantACouvrir = 0;
                }

            }
            //5-
            var removeTask = PureRemoveAsync(operations);
            await removeTask;
            if (removeTask.IsCompletedSuccessfully)
            {
                return operations;
            }
            else
            {
                throw new Exception("La légende du nain à 9 jambes !");
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Operation> PureRemoveAsync(IEnumerable<Operation> inputs)
        {
            if (inputs != null)
            {
                foreach (Operation iter in inputs)
                {
                    Operation ope = new (iter);
                    ope.Value = -Math.Abs(iter.Value);
                    using var removeTask = _opeRepo.InsertAsync(ope);
                    await removeTask;
                    if (removeTask.IsCompletedSuccessfully)
                    {
                        return ope;
                    }
                    else
                    {
                        throw new Exception("C'est vous le doc, doc !");
                    }
                }
            }
            return null;
        }
    }
}
