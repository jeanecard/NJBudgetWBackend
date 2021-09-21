using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Commun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NJBudgetWBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppartenanceController : ControllerBase
    {
        // GET: api/<AppartenanceController>
        [HttpGet]
        public IEnumerable<Appartenance> Get()
        {
            return new List<Appartenance> {
                new Appartenance(){ Id = Guid.Parse(Constant.APPARTENANCE_COMMUN_GUID), Caption = Constant.APPARTENANCE_COMMUN_CAPTION},
                new Appartenance(){ Id = Guid.Parse(Constant.APPARTENANCE_JEAN_GUID), Caption = Constant.APPARTENANCE_JEAN_CAPTION},
                new Appartenance(){ Id = Guid.Parse(Constant.APPARTENANCE_NADEGE_GUID), Caption = Constant.APPARTENANCE_NADEGE_CAPTION},
                new Appartenance(){ Id = Guid.Parse(Constant.APPARTENANCE_THOMAS_GUID), Caption = Constant.APPARTENANCE_THOMAS_CAPTION},
            };
        }
    }
}
