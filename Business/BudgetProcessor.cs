using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business.Interface;
using System;
using System.Collections.Generic;

//Au debut du oi son donne l'argent a tous les comptes (opération epargne) en cous du ois on fait les depenses.
namespace NJBudgetWBackend.Business
{
    public class BudgetProcessor : IBudgetProcessor
    {
        /// <summary>
        /// Calcul le budget consommé, épargné et restant sur le mois month de l'année year.
        /// </summary>
        /// <param name="compte"></param>
        /// <param name="operations"></param>
        /// <param name="month"></param>
        public void ProcessBudgetSpentAndLeft(Compte compte, IEnumerable<Operation> operations, byte month, ushort year)
        {
            if (compte == null || month == 0 || month > 12)
            {
                throw new ArgumentException("Ah ah, ils vous ont refiler toutes leurs merdes");
            }
            compte.BudgetConsummed = 0;
            compte.BudgetProvision = 0;
            compte.BudgetLeft = compte.BudgetExpected;
            if (operations != null)
            {
                foreach (Operation iter in operations)
                {
                    if (iter.DateOperation.Month == month && iter.DateOperation.Year == year)
                    {
                        compte.BudgetConsummed += Math.Abs(iter.Value);
                        if (iter.Value > 0)
                        {
                            compte.BudgetProvision += iter.Value;
                        }
                    }
                }
                compte.BudgetLeft = compte.BudgetExpected - compte.BudgetConsummed;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compte"></param>
        /// <param name="operations"></param>
        public void ProcessState(Compte compte, IEnumerable<Operation> operations)
        {
            if (compte == null || operations == null)
            {
                throw new ArgumentNullException("Ah mais je connais Miwege !");
            }
            float epargne = 0;
            float depense = 0;
            Dictionary<(int, int), int> monthOperations = new Dictionary<(int,int), int>();
            foreach (Operation iter in operations)
            {
                if(!monthOperations.ContainsKey((iter.DateOperation.Year,iter.DateOperation.Month)))
                {
                    monthOperations.Add((iter.DateOperation.Year, iter.DateOperation.Month), 2);
                }
                if (iter.Value < 0)
                {
                    depense += iter.Value;
                }
                else
                {
                    epargne += iter.Value;
                }
            }
            int jourDuMois = DateTime.Now.Day;
            switch (compte.OperationAllowed)
            {
                case OperationTypeEnum.AddOnly:
                    ProcessStateAddOnly(compte, epargne, monthOperations.Count);
                    break;
                case OperationTypeEnum.DeleteOnly:
                    ProcessStateDeleteOnly(compte, depense, monthOperations.Count, (byte)jourDuMois);
                    break;
                case OperationTypeEnum.AddAndDelete:
                    ProcessStateAddAndDelete(compte, depense, epargne, monthOperations.Count);
                    break;
                case OperationTypeEnum.None:
                    ProcessStateNone(compte);
                    break;
                default:
                    break;
            }
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compte"></param>
        public void ProcessStateNone(Compte compte)
        {
            if(compte == null)
            {
                return;
            }
            compte.State = CompteStatusEnum.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compte"></param>
        /// <param name="depense"></param>
        /// <param name="epargne"></param>
        /// <param name="count"></param>
        public void ProcessStateAddAndDelete(Compte compte, float depense, float provision, int nbMonth)
        {
            if(compte == null)
            {
                return;
            }
            float fullBudgetExpected = (nbMonth * compte.BudgetExpected);
            float delta = fullBudgetExpected - Math.Abs(depense) - Math.Abs(provision);
            var absDelta = Math.Abs(delta);
            if (delta >= 0)
            {
                if(provision <= depense)
                {
                    compte.State = CompteStatusEnum.Warning;
                }
                else
                {
                    if(provision >= 0.75 * fullBudgetExpected)
                    {
                        compte.State = CompteStatusEnum.Good;
                    }
                    else if(provision >= 0.25 * fullBudgetExpected)
                    {
                        compte.State = CompteStatusEnum.Warning;
                    }
                    else
                    {
                        compte.State = CompteStatusEnum.Danger;
                    }
                }
            }
            else
            {
                if (provision >= depense)
                {
                    if (absDelta <= 0.25 * fullBudgetExpected)
                    {
                        compte.State = CompteStatusEnum.Warning;
                    }
                    else
                    {
                        compte.State = CompteStatusEnum.Danger;
                    }
                }
                else
                {
                    if (absDelta <= 0.10 * fullBudgetExpected)
                    {
                        compte.State = CompteStatusEnum.Warning;
                    }
                    else
                    {
                        compte.State = CompteStatusEnum.Danger;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compte"></param>
        /// <param name="depense"></param>
        /// <param name="nbMonth"></param>
        public void ProcessStateDeleteOnly(Compte compte, float depense, int nbMonth, byte jourDuMois)
        {
            if(compte == null)
            {
                return;
            }
            float delta = (nbMonth * compte.BudgetExpected) - Math.Abs(depense);
            var absDelta = Math.Abs(delta);
            if (delta >= 0)
            {
                compte.State = CompteStatusEnum.Good;
            }
            else if (absDelta <= compte.BudgetExpected)
            {

                if (absDelta <= 0.15 * compte.BudgetExpected)
                {
                    compte.State = CompteStatusEnum.Good;
                }
                else if (absDelta <= 0.5 * compte.BudgetExpected)
                {
                    compte.State = jourDuMois < 15 ? CompteStatusEnum.Good : CompteStatusEnum.Warning;
                }
                else
                {
                    compte.State = CompteStatusEnum.Warning;
                }
            }
            else
            {
                compte.State = CompteStatusEnum.Danger;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compte"></param>
        /// <param name="epargne"></param>
        /// <param name="count"></param>
        public void ProcessStateAddOnly(Compte compte, float epargne, int count)
        {
            throw new NotImplementedException("Tu ressembles à un Nazgul avec des tongues !");
        }


    }
}
