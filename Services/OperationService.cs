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
        private IOperationsRepository _opeRepo = null;
        private IGroupService _groupService = null;
        private IBalanceProcessor _balanceBusiness = null;
        private OperationService()
        {

        }

        public OperationService(IOperationsRepository repo)
        {
            _opeRepo = repo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task AddAsync(Operation operation)
        {
            if(operation == null)
            {
                return;
            }
            operation.Value = Math.Abs(operation.Value);
            using var addTask = _opeRepo.InsertAsync(operation);
            await addTask;
            if(!addTask.IsCompletedSuccessfully)
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
            if(compteTask.IsCompletedSuccessfully)
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
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<Compte> RemoveAsync(Operation operation)
        {
            if (operation == null)
            {
                return null;
            }
            //1- Obtention de l'état du compte
            DateTime now = DateTime.Now;
            using var compteTask = _groupService.GetCompteAsync(operation.CompteId, (byte)now.Month, (ushort)now.Year);
            await compteTask;
            List<Operation> operations = new List<Operation>();
            //2- Si le compte est suffisant pour la dépense, on fait l'operation
            if(compteTask.Result.BudgetLeft > 0 && compteTask.Result.BudgetLeft >= (operation.Value) )
            {
                operations.Add(operation);
            }
            //3- Sinon
            else
            {
                float valeurRestantACouvrir = Math.Abs(operation.Value);
                //3.1- Réalisation d'un opération de retrait avec le reste du mois disponible
                if (compteTask.Result.BudgetLeft > 0)
                {
                    valeurRestantACouvrir -= compteTask.Result.BudgetLeft;
                    var operationDuMoisPossible = new Operation(operation);
                    operationDuMoisPossible.Value = -compteTask.Result.BudgetLeft;
                    operations.Add(operationDuMoisPossible);
                }
                //3.2- Calcul de la balance pour étudier s'il existe un reste à verser positif possible
                using var operations12DerniersMoisTask = _opeRepo.GetOperationsAsync(
                                            operation.CompteId,
                                            now.AddMonths(-12),
                                            now,
                                            compteTask.Result.OperationAllowed);
                await operations12DerniersMoisTask;
                if (operations12DerniersMoisTask.IsCompletedSuccessfully)
                {
                    float balance = 0;
                    _balanceBusiness.ProcessBalance(out balance, compteTask.Result.BudgetExpected, compteTask.Result.Operations);
                    //3.1- Si la balance est positive
                    if(balance > 0)
                    {
                        if(balance >= valeurRestantACouvrir)
                        {
                            valeurRestantACouvrir = 0;
                            var operationBalanceComplete = new Operation(operation);
                            operationBalanceComplete.Value = - (balance - valeurRestantACouvrir);
                            operationBalanceComplete.IsOperationSystem = true; 
                            operations.Add(operationBalanceComplete);
                        }
                        else
                        {
                            valeurRestantACouvrir -= balance;
                            var operationBalancePartielle = new Operation(operation);
                            operationBalancePartielle.Value = -(balance - valeurRestantACouvrir);
                            operationBalancePartielle.IsOperationSystem = true;
                            operations.Add(operationBalancePartielle);
                        }
                    }
                    if(valeurRestantACouvrir > 0)
                    {
                        var operationRestantACouvrir = new Operation(operation);
                        operationRestantACouvrir.Value = -valeurRestantACouvrir;
                        operations.Add(operationRestantACouvrir);
                    }
                     
                }
                else
                {
                    throw new Exception("Cette fois, ça va être dur de les encercler colonel ..");
                }
            }
            //4-
            var removeTask = PureRemoveAsync(operations);
            await removeTask;
            if(removeTask.IsCompletedSuccessfully)
            {
                //5- On récupère l'état final du compte que l'on retourne.
                using var compteFinalTask = _groupService.GetCompteAsync(operation.CompteId, (byte)now.Month, (ushort)now.Year);
                await compteFinalTask;
                return compteFinalTask.Result;
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
            if(inputs != null)
            {
                foreach(Operation iter in inputs)
                {
                    Operation ope = new Operation(iter);
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
