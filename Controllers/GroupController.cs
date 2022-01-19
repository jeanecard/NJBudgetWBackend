using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Services;
using NJBudgetWBackend.Services.Interface;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NJBudgetWBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService = null;
        private readonly IAuthZService _authZService = null;
        private readonly IModelJammer _modelJammer = null;


        private GroupController()
        {
            //Dummy for DI.
        }

        public GroupController(
            IGroupService gService, 
            IAuthZService authService,
            IModelJammer jammer)
        {
            _groupService = gService;
            _authZService = authService;
            _modelJammer = jammer;
        }

        /// <summary>
        /// First violent version. We rely on webapi middleware for error.
        /// </summary>
        /// <param name="idAppartenance"></param>
        /// <returns></returns>
        // GET api/<GroupController>/5
        [HttpGet("ByIdAppartenance/{idAppartenance}")]
        public async Task<IEnumerable<Group>> GetGroupsAsync(Guid idAppartenance)
        {

            if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
            || !_authZService.IsAuthZ(values.ToString()))
            {
                GroupServiceJammer jammer = new GroupServiceJammer(_groupService);
                var jammedTask = jammer.GetGroupsAsync(idAppartenance);
                await jammedTask;
                return jammedTask.Result;
            }

            using var getTask = _groupService.GetGroupsAsync(idAppartenance);
            await getTask;
            if (getTask.IsCompletedSuccessfully)
            {
                return getTask.Result;
            }
            else
            {
                throw new Exception("J'aime les gateaux");
            }
        }

        /// <summary>
        /// First violent version. We rely on webapi middleware for error.
        /// month == 0  => current month
        /// </summary>
        /// <param name="idAppartenance"></param>
        /// <returns></returns>
        // GET api/<GroupController>/5
        [HttpGet("ById/{idGroup}")]
        public async Task<Compte> GetCurrentCompteAsync(Guid idGroup)
        {
            if(idGroup == Guid.Empty)
            {
                return null;
            }

            using var getTask = _groupService.GetCompteAsync(idGroup, (byte) DateTime.Now.Month, (ushort) DateTime.Now.Year);
            await getTask;
            if (getTask.IsCompletedSuccessfully)
            {
                if (!this.Request.Headers.TryGetValue("background-id", out StringValues values)
                || !_authZService.IsAuthZ(values.ToString()))
                {
                    return _modelJammer.Jam(getTask.Result);
                }
                else
                {
                    return getTask.Result;
                }
            }
            else
            {
                throw new Exception("Bennie, bennie, bennie !!!!!!");
            }
        }
    }
}
