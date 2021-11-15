﻿using Dapper;
using Microsoft.Extensions.Configuration;
using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Models;
using NJBudgetWBackend.Repositories.Interface;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Repositories
{
    public class OperationsRepository : IOperationsRepository
    {
        private readonly IConfiguration _config = null;
        private OperationsRepository()
        {

        }

        public OperationsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compteId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Operation>> GetOperationsAsync(
            Guid compteId, 
            DateTime? from, 
            DateTime? to, 
            OperationTypeEnum opeType)
        {
            if (compteId == Guid.Empty)
            {
                return null;
            }
            if (!from.HasValue)
            {
                from = DateTime.MinValue;
            }
            if (!to.HasValue)
            {
                to = DateTime.MaxValue;
            }
            String query = "SELECT* FROM public.\"OPERATION\" WHERE  \"CompteId\" = :id::uuid AND \"DateOperation\" between :from::date and :to::date ORDER BY \"DateOperation\" DESC";
            using var connection = new NpgsqlConnection(PGSqlTools.GetCxString(_config));
            using var operationsTask = connection.QueryAsync<Operation>(
                query,
                new
                {
                    id = compteId.ToString(),
                    from ,
                    to
                });
            await operationsTask;
            if (operationsTask.IsCompletedSuccessfully)
            {
                //!linq !!
                foreach(Operation iterator in operationsTask.Result)
                {
                    iterator.OperationAllowed = opeType;
                }
                return operationsTask.Result;
                //Linq pour mettre à jour un param 
                //    TODO
            }
            else
            {
                throw new Exception("Putain de caisse de meeeeeeeerrrrrrrrrrrrrrrrrdddddddddddddddeeeeeeeeeeeeeeee !!!!!!!!!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compteId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SyntheseOperationRAwDB>> GetOperationsAsync(DateTime? from, DateTime? to)
        {
            if (!from.HasValue)
            {
                from = DateTime.MinValue;
            }
            if (!to.HasValue)
            {
                to = DateTime.MaxValue;
            }
            String query = "SELECT \"CompteId\", \"DateOperation\", \"Value\", \"AppartenanceId\", \"OperationAllowed\", \"BudgetExpected\" " +
                "FROM public.\"OPERATION\"  " +
                "INNER JOIN public.\"GROUP\" ON public.\"OPERATION\".\"CompteId\" = public.\"GROUP\".\"Id\" " +
                "WHERE   \"DateOperation\" between :from::date and :to::date";
            using var connection = new NpgsqlConnection(PGSqlTools.GetCxString(_config));
            using var operationsTask = connection.QueryAsync<SyntheseOperationRAwDB>(
                query,
                new
                {
                    from,
                    to,
                });
            await operationsTask;
            if (operationsTask.IsCompletedSuccessfully)
            {
                return operationsTask.Result;
            }
            else
            {
                throw new Exception("Oh bravo");
            }
        }

  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public async Task InsertAsync(Operation op)
        {
            if (op == null)
            {
                return;
            }
            String query = "INSERT INTO public.\"OPERATION\"(\"Id\", \"CompteId\", \"DateOperation\", \"Value\", \"Caption\", \"User\", \"IsOperationSystem\", \"TransactionId\") " +
                "VALUES(:id, :compteId, :dateOperation, :value, :caption, :user, :isSystem, :transactionId)";

            using var connection = new NpgsqlConnection(PGSqlTools.GetCxString(_config));
            using var insertTask = connection.ExecuteAsync(
                query,
                new
                {
                    id = Guid.NewGuid(),
                    compteId = op.CompteId,
                    dateOperation = op.DateOperation,
                    value = op.Value,
                    caption = op.Caption,
                    user = op.User,
                    isSystem = op.IsOperationSystem,
                    transactionId = op.TransactionId
                });
            await insertTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOperation"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid idOperation)
        {
            //;
            if (idOperation == Guid.Empty)
            {
                return;
            }
            String query = "DELETE FROM public.\"OPERATION\"  WHERE  \"Id\" = :id::uuid";
            using var connection = new NpgsqlConnection(PGSqlTools.GetCxString(_config));
            using var deleteTask = connection.ExecuteAsync(
                           query,
                           new { id = idOperation });
            await deleteTask;
        }

        public async Task<Guid> GetCompteOperationAsync(Guid operationid)
        {
            if (operationid == Guid.Empty)
            {
                return Guid.Empty;
            }
            String query = "SELECT* FROM public.\"OPERATION\" WHERE  \"Id\" = :id::uuid";
            using var connection = new NpgsqlConnection(PGSqlTools.GetCxString(_config));
            using var operationsTask = connection.QueryAsync<Operation>(
                query,
                new
                {
                    id = operationid.ToString()
                });
            await operationsTask;
            if (operationsTask.IsCompletedSuccessfully)
            {
                Operation result = operationsTask.Result.FirstOrDefault();
                return result != null ? result.CompteId : Guid.Empty;
            }
            else
            {
                throw new Exception("Putain de caisse de meeeeeeeerrrrrrrrrrrrrrrrrdddddddddddddddeeeeeeeeeeeeeeee !!!!!!!!!");
            }

        }
    }
}
