using Microsoft.AspNetCore.Mvc;
using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NJBudgetWBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private IOperationService _opeService = null;
        // POST api/<OperationController>
        [HttpPost("add-operation")]
        public async Task Add([FromBody] Operation value)
        {
            if(value == null )
            {
                return;
            }
            using var opTask = _opeService.AddAsync(value);
            await opTask;
            if(opTask.IsCompletedSuccessfully)
            {
                return;
            }
            else
            {
                throw new Exception("Moi toutes ces histoires de merde, ça me fout la tête comme une callebasse !");
            }
        }

        // POST api/<OperationController>
        [HttpPost("remove-operation")]

        public async void Remove([FromBody] Operation value)
        {
            if (value == null)
            {
                return;
            }
            using var opTask = _opeService.RemoveAsync(value);
            await opTask;
            if (opTask.IsCompletedSuccessfully)
            {
                return;
            }
            else
            {
                throw new Exception("Tu connais quelqu'un qui tire plus vite ? ... non Personne :-)");
            }
        }

        // DELETE api/<OperationController>/5
        [HttpDelete("{idOperation}")]
        public async void Delete(Guid idOperation)
        {
            if (idOperation == Guid.Empty)
            {
                return;
            }
            using var opTask = _opeService.DeleteAsync(idOperation);
            await opTask;
            if (opTask.IsCompletedSuccessfully)
            {
                return;
            }
            else
            {
                throw new Exception("Les années ne font pas des sages, elles ne font que des viellards");
            }
        }
    }
}
