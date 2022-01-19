using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Services.Interface;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NJBudgetWBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyntheseController : ControllerBase
    {
        private readonly ISyntheseService _synthService = null;
        private readonly IModelJammer _jammer = null;
        private readonly IAuthZService _authZService = null;

        private SyntheseController()
        {
            //Dummy for DI.
        }

        public SyntheseController(
            ISyntheseService service,
            IModelJammer jammer,
            IAuthZService authService)
        {
            _synthService = service;
            _jammer = jammer;
            _authZService = authService;
        }
        //getExpenseGroupByAppartenance
        // GET: api/<SyntheseController>
        [HttpGet("ByAppartenance/{year}/{month}/{day}")]
        public async Task<SyntheseDepenseGlobalModel> GetByAppartenanceAsync(uint year, byte month, byte day)
        {
            var inputDate = new DateTime((int)year, month, day);

            using var getTask = _synthService.GetSyntheseByMonthByAppartenanceAsync(inputDate);
            await getTask;
            if (getTask.IsCompletedSuccessfully)
            {
                if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
           || !_authZService.IsAuthZ(values.ToString()))
                {
                    return _jammer.Jam(getTask.Result);
                }
                else
                {
                    return getTask.Result;
                }
            }
            else
            {
                throw new Exception("C'est pas faux");
            }
        }

        //getExpenseGroupByAppartenance
        // GET: api/<SyntheseController>
        [HttpGet("ForAppartenance/{appartenanceId}/{year}/{month}/{day}")]
        public async Task<SyntheseDepenseByAppartenanceModel> GetByGroupAsync(Guid appartenanceId, int year, byte month, byte day)
        {
            var inputDate = new DateTime((int)year, month, day);

            using Task<SyntheseDepenseByAppartenanceModel> getTask = _synthService.GetSyntheseByMonthForAppartenanceAsync(appartenanceId, inputDate);
            await getTask;
            if (getTask.IsCompletedSuccessfully)
            {
                if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
            || !_authZService.IsAuthZ(values.ToString()))
                {
                    return _jammer.Jam(getTask.Result);
                }
                else
                {
                    return getTask.Result;

                }
            }
            else
            {
                throw new Exception("Vous m'avez pris pour un enseignant !!");
            }
        }

        [HttpGet("SyntheseMois/{year}/{month}/{day}")]
        public async Task<SyntheseMoisModel> GetGlobalAsync(int year, byte month, byte day)
        {
                var inputDate = new DateTime((int)year, month, day);

            using Task<SyntheseMoisModel> getTask = _synthService.GetSyntheseGlobalMonth(inputDate);
            await getTask;
            if (getTask.IsCompletedSuccessfully)
            {
                if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
            || !_authZService.IsAuthZ(values.ToString()))
                {
                    return _jammer.Jam(getTask.Result);
                }
                else
                {
                    return getTask.Result;

                }
            }
            else
            {
                throw new Exception("Le gras c'est la vie.");
            }
        }
    }
}
