using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business;
using NJBudgetWBackend.Business.Interface;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Repositories.Interface;
using NJBudgetWBackend.Services.Interface;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class CumulativeService : ICumulativeService
    {
        private IPeriodProcessor _periodProcessor = null;
        private IOperationsRepository _operationRepo = null;
        private IStatusProcessor _statusProcessor = null;
        private ISyntheseService _synthService = null;
        private CumulativeService()
        {
            //Dummy for DI
        }
        public CumulativeService(
            IPeriodProcessor processor, 
            IOperationsRepository opeRepo,
            IStatusProcessor statutProcessor,
            ISyntheseService synthService)
        {
            _periodProcessor = processor;
            _operationRepo = opeRepo;
            _statusProcessor = statutProcessor;
            _synthService = synthService;
        }

        public async Task<CumulativeOperation> GetAllCompteCumulAsync(byte forLastnMonths)
        {

          if(forLastnMonths == 0)
            {
                return null;
            }
            forLastnMonths = 1;
              DateTime from = DateTime.Now;
            DateTime to = DateTime.Now;
            _periodProcessor.ProcessRangeBeforeTo(to, forLastnMonths, out from);

            CumulativeOperation retour = new CumulativeOperation()
            {
                From = from,
                To = to,
                GroupId = null,
                Value = 0
            };

            for (int i = -1; i < forLastnMonths; i++)
            {
                using Task<SyntheseMoisModel> getTask = _synthService.GetSyntheseGlobalMonth(from);
                await getTask;
                if (getTask.IsCompletedSuccessfully)
                {
                    retour.Value -= getTask.Result.BudgetValueDepense;
                    retour.Value += getTask.Result.BudgetValuePrevu;

                }
                else
                {
                    throw new Exception("Des gremlins !!! Saloperie de machines étrangères");
                }
                from = from.AddMonths(1);
            }
            _statusProcessor.ProcessCumulativeOperation(retour);
            return retour;
        }

        public async Task<CumulativeOperation> GetCompteCumulAsync(Guid groupId, byte forLastnMonths)
        {
            throw new NotImplementedException();
            //DateTime from = DateTime.Now;
            //DateTime to = DateTime.Now;
            //_periodProcessor.ProcessRangeBeforeTo(to, forLastnMonths, out from);
            //var task = _operationRepo.GetOperationsAsync(groupId, from, to);
            //await task;
            //if (task.IsCompletedSuccessfully)
            //{
            //    CumulativeOperation retour = new CumulativeOperation()
            //    {
            //        From = from,
            //        To = to,
            //        GroupId = groupId,
            //        Value = 0
            //    };
            //    foreach (Operation iter in task.Result)
            //    {
            //        retour.Value += iter.Value;
            //    }
            //    _statusProcessor.ProcessCumulativeOperation(retour);
            //    return retour;
            //}
            //else
            //{
            //    throw new Exception("J'déteste le fumier !!!");
            //}
        }
    }
}
