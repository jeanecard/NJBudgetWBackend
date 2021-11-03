using Microsoft.AspNetCore.Mvc;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NJBudgetWBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CumulativeController : ControllerBase
    {
        private ICumulativeService _cumulService = null;

        private CumulativeController()
        {
            //Dummy for DI.
        }

        public CumulativeController(ICumulativeService service)
        {
            _cumulService = service;
        }


        // GET: api/<CumulativeController>
        /// <summary>
        /// Get balance of all compte at this instant on the last 6 months
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<CumulativeOperation> GetAsync()
        {
            var cumulTask = _cumulService.GetAllCompteCumulAsync(6);
            await cumulTask;
            if(cumulTask.IsCompletedSuccessfully)
            {
                return cumulTask.Result;
            }
            else
            {
                throw new Exception("Choccoooollllaatttt, sinnnooque !!");
            }
        }

        /// <summary>
        /// Get balance of all compte at this instant on the last 6 months
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("{groupId}")]
        public async Task<CumulativeOperation> Get(Guid groupId)
        {
            var cumulTask = _cumulService.GetCompteCumulAsync(groupId, 6);
            await cumulTask;
            if (cumulTask.IsCompletedSuccessfully)
            {
                return cumulTask.Result;
            }
            else
            {
                throw new Exception("goooonnniees !!");
            }
        }
    }
}
