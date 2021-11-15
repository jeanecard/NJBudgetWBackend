using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Commun;
using NJBudgetWBackend.Services.Interface.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class AppartenanceService : IAppartenanceService
    {
        private readonly static Appartenance _commun = new () { Id = Guid.Parse(Constant.APPARTENANCE_COMMUN_GUID), Caption = Constant.APPARTENANCE_COMMUN_CAPTION };
        private readonly static Appartenance _jean = new () { Id = Guid.Parse(Constant.APPARTENANCE_JEAN_GUID), Caption = Constant.APPARTENANCE_JEAN_CAPTION };
        private readonly static Appartenance _nad = new () { Id = Guid.Parse(Constant.APPARTENANCE_NADEGE_GUID), Caption = Constant.APPARTENANCE_NADEGE_CAPTION };
        private readonly static Appartenance _thomas = new () { Id = Guid.Parse(Constant.APPARTENANCE_THOMAS_GUID), Caption = Constant.APPARTENANCE_THOMAS_CAPTION };

        public Appartenance GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            return id.ToString() switch
            {
                Constant.APPARTENANCE_COMMUN_GUID
                    => _commun,
                Constant.APPARTENANCE_JEAN_GUID
                    => _jean,
                Constant.APPARTENANCE_NADEGE_GUID
                    => _nad,
                Constant.APPARTENANCE_THOMAS_GUID
                    => _thomas,
                _ => null,
            };
        }

        public IEnumerable<Appartenance> Get()
        {
            return new List<Appartenance> {
                _commun,
                _jean,
                _nad,
                _thomas,
            };

        }
    }
}
