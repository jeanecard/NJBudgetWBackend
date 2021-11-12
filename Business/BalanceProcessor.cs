using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetWBackend.Business
{
    public class BalanceProcessor : IBalanceProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="depensePure"></param>
        /// <param name="budgetProvisonne"></param>
        /// <param name="budgetEpargne"></param>
        /// <param name="balance"></param>
        /// <param name="operations"></param>
        /// <param name="budgetExpectedByMonth"></param>
        public void ProcessBalance(
            out float balance,
            float budgetExpectedByMonth,
            in IEnumerable<IOperation> operations,
            in DateTime? processDateConsideration = null)
        {
            float depensePureMoisCourant = 0;
            float depensePureAvantMoisCourant = 0;

            float budgetEpargneAvantMoisCourant = 0;
            float budgetProvisonneAvantMoisCourant = 0;
            float budgetEpargneMoisCourant = 0;
            float budgetProvisonneMoisCourant = 0;


            DateTime processOnDate = processDateConsideration.HasValue ? processDateConsideration.Value : DateTime.Now;
            balance = 0;

            if (operations != null)
            {
                foreach (IOperation iter in operations)
                {
                    if (iter.DateOperation <= processOnDate)
                    {
                        if (
                            iter.DateOperation.Year == processOnDate.Year
                            && iter.DateOperation.Month == processOnDate.Month
                            )
                        {
                            if (iter.Value > 0)
                            {
                                if (iter.OperationAllowed == OperationTypeEnum.EpargneAndDepense
                                    || iter.OperationAllowed == OperationTypeEnum.EpargneOnly)
                                {
                                    budgetEpargneMoisCourant += iter.Value;
                                }
                                else if (iter.OperationAllowed == OperationTypeEnum.ProvisionAndDepense
                                    || iter.OperationAllowed == OperationTypeEnum.ProvisionOnly)
                                {
                                    budgetProvisonneMoisCourant += iter.Value;
                                }
                            }
                            else
                            {
                                depensePureMoisCourant -= iter.Value;
                            }

                        }
                        else
                        {
                            if (iter.Value > 0)
                            {
                                if (iter.OperationAllowed == OperationTypeEnum.EpargneAndDepense
                                    || iter.OperationAllowed == OperationTypeEnum.EpargneOnly)
                                {
                                    budgetEpargneAvantMoisCourant += iter.Value;
                                }
                                else if (iter.OperationAllowed == OperationTypeEnum.ProvisionAndDepense
                                    || iter.OperationAllowed == OperationTypeEnum.ProvisionOnly)
                                {
                                    budgetProvisonneAvantMoisCourant += iter.Value;
                                }
                            }
                            else
                            {
                                depensePureAvantMoisCourant -= iter.Value;
                            }
                        }
                    }
                }
                float balanceAvantMoisCourant = (budgetProvisonneAvantMoisCourant + budgetEpargneAvantMoisCourant) - depensePureAvantMoisCourant;

                float epargnePotentielOuReelMoisCourant =
                    budgetEpargneMoisCourant + budgetProvisonneMoisCourant > budgetExpectedByMonth ?
                    budgetEpargneMoisCourant + budgetProvisonneMoisCourant : budgetExpectedByMonth;
                float balanceMoisCourant = epargnePotentielOuReelMoisCourant - depensePureMoisCourant;

                balance = balanceAvantMoisCourant + balanceMoisCourant;
            }
        }
    }
}
