using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Services.Interface;
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
        private readonly IOperationService _opeService = null;
        private readonly IGroupService _groupService = null;
        private readonly IAuthZService _authZService = null;

        private OperationController()
        {

        }
        public OperationController(
            IOperationService service,
            IGroupService groupService,
            IAuthZService authZService)
        {
            _opeService = service;
            _groupService = groupService;
            _authZService = authZService;
        }
        // POST api/<OperationController>
        [HttpPost("add-operation")]
        public async Task<Compte> Add([FromBody] Operation value)
        {
            if(value == null )
            {
                return null;
            }

            if(!this.Request.Headers.TryGetValue("background-id", out StringValues values) 
                || !_authZService.IsAuthZ(values.ToString()))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            using var opTask = _opeService.AddAsync(value);
            await opTask;
            if(opTask.IsCompletedSuccessfully)
            {
                DateTime now = DateTime.Now;
                using var compteTask = _groupService.GetCompteAsync(value.CompteId, (byte)now.Month, (ushort)now.Year);
                await compteTask;
                return compteTask.Result;
            }
            else
            {
                throw new Exception("Moi toutes ces histoires de merde, ça me fout la tête comme une callebasse !");
            }
        }

        // POST api/<OperationController>
        [HttpPost("remove-operation")]

        public async Task<Compte> Remove([FromBody] Operation value)
        {
            if (value == null)
            {
                return null;
            }


            if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
                || !_authZService.IsAuthZ(values.ToString()))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            using var opTask = _opeService.RemoveAsync(value);
            await opTask;
            if (opTask.IsCompletedSuccessfully)
            {
                DateTime now = DateTime.Now;
                using var compteTask = _groupService.GetCompteAsync(value.CompteId, (byte)now.Month, (ushort)now.Year);
                await compteTask;
                return compteTask.Result;
            }
            else
            {
                throw new Exception("Tu connais quelqu'un qui tire plus vite ? ... non Personne :-)");
            }
        }

        // DELETE api/<OperationController>/5
        [HttpDelete("delete-operation/{idOperation}")]
        public async Task<Compte> Delete(Guid idOperation)
        {
            if (idOperation != Guid.Empty)
            {

                if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
                    || !_authZService.IsAuthZ(values.ToString()))
                {
                    HttpContext.Response.StatusCode = 401;
                    return null;
                }

                using var opTask = _opeService.DeleteAsync(idOperation);
                await opTask;
                if (!opTask.IsCompletedSuccessfully)
                {
                    throw new Exception("Les années ne font pas des sages, elles ne font que des viellards");
                }
                else
                {
                    DateTime now = DateTime.Now;
                    using var compteTask = _groupService.GetCompteAsync(opTask.Result, (byte)now.Month, (ushort)now.Year);
                    await compteTask;
                    return compteTask.Result;
                }
            }
            return null;
        }
    }
}
