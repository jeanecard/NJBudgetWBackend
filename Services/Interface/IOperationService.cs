﻿using NJBudgetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services.Interface.Interface
{
    public interface IOperationService
    {
        public Task AddAsync(Operation operation);
        public Task<IEnumerable<IOperation>> RemoveAsync(Operation operation);
        public Task<Guid> DeleteAsync(Guid operationid);


    }
}
