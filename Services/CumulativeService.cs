using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business;
using NJBudgetWBackend.Business.Interface;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Repositories.Interface;
using NJBudgetWBackend.Services.Interface;
using System;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class CumulativeService : ICumulativeService
    {
        private IPeriodProcessor _periodProcessor = null;
        private IOperationsRepository _operationRepo = null;
        private IStatusProcessor _statusProcessor = null;
        private CumulativeService()
        {
            //Dummy for DI
        }
        public CumulativeService(
            IPeriodProcessor processor, 
            IOperationsRepository opeRepo,
            IStatusProcessor statutProcessor)
        {
            _periodProcessor = processor;
            _operationRepo = opeRepo;
            _statusProcessor = statutProcessor;
        }

        public async Task<CumulativeOperation> GetAllCompteCumulAsync(byte forLastnMonths)
        {
            DateTime from = DateTime.Now;
            DateTime to = DateTime.Now;
            _periodProcessor.ProcessRangeBeforeTo(to, forLastnMonths, out from);
            var task = _operationRepo.GetOperationsAsync(from, to);
            await task;
            if (task.IsCompletedSuccessfully)
            {
                CumulativeOperation retour = new CumulativeOperation()
                {
                    From = from,
                    To = to,
                    GroupId = null,
                    Value = 0
                };
                foreach (SyntheseOperationRAwDB iter in task.Result)
                {
                    retour.Value += iter.Value;
                }
                _statusProcessor.ProcessCumulativeOperation(retour);
                return retour;
            }
            else
            {
                throw new Exception("Des gremlins !!! Saloperie de machines étrangères");
            }


        }

        public async Task<CumulativeOperation> GetCompteCumulAsync(Guid groupId, byte forLastnMonths)
        {
            DateTime from = DateTime.Now;
            DateTime to = DateTime.Now;
            _periodProcessor.ProcessRangeBeforeTo(to, forLastnMonths, out from);
            var task = _operationRepo.GetOperationsAsync(groupId, from, to);
            await task;
            if (task.IsCompletedSuccessfully)
            {
                CumulativeOperation retour = new CumulativeOperation()
                {
                    From = from,
                    To = to,
                    GroupId = groupId,
                    Value = 0
                };
                foreach (Operation iter in task.Result)
                {
                    retour.Value += iter.Value;
                }
                _statusProcessor.ProcessCumulativeOperation(retour);
                return retour;
            }
            else
            {
                throw new Exception("J'déteste le fumier !!!");
            }
        }
    }
}
