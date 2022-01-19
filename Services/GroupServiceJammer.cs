using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Services
{
    public class GroupServiceJammer : IGroupService
    {
        private IGroupService _groupService = null;
        private GroupServiceJammer()
        {
            //Dummy for DI.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="apService"></param>
        public GroupServiceJammer(
            IGroupService groupService
        )
        {
            _groupService = groupService;
        }
        /// <summary>
        /// No need to jam this.
        /// </summary>
        /// <param name="idAppartenance"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Group>> GetGroupsAsync(Guid idAppartenance)
        {
            if (idAppartenance == Guid.Empty)
            {
                return null;
            }
            using var getGroupsAsyncTask = _groupService.GetGroupsAsync(idAppartenance);
            await getGroupsAsyncTask;
            if (getGroupsAsyncTask.IsCompletedSuccessfully)
            {
                return getGroupsAsyncTask.Result;
            }
            return null;
        }
        /// <summary>
        /// 1-
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public async Task<Compte> GetCompteAsync(Guid idGroup, byte month, ushort year)
        {
            if (month == 0 || month > 12 || year < 2020)
            {
                return null;
            }
            Random rand = new Random();
            Compte retour = new Compte()
            {
                Balance = rand.Next(0, 100),
                BudgetConsummed = rand.Next(0, 1000),
                BudgetExpected = rand.Next(0, 2000),
                BudgetLeft = rand.Next(0, 100),
                BudgetProvision = rand.Next(0, 50),
                Group = new Group()
                {
                    Appartenance = new Appartenance()
                    {
                        Caption = "Personne",
                        Id = Guid.NewGuid()
                    },
                    Id = idGroup,
                    Caption = "Group"
                },
                OperationAllowed = OperationTypeEnum.EpargneAndDepense,
                Operations = new List<Operation>() { },
                State = CompteStatusEnum.Good
            };
            await Task.Delay(1);
            return retour;
        }
    }
}
