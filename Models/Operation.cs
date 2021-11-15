using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJBudgetBackEnd.Models
{
    public interface IOperation
    {
        public DateTime DateOperation { get; set; }
        public float Value { get; set; }
        public OperationTypeEnum OperationAllowed { get; set; }
        public bool IsOperationSystem { get; set; }
    }

    public class BasicOperation : IOperation
    {
        public BasicOperation()
        {
            IsOperationSystem = false;
        }
        public DateTime DateOperation { get; set; }
        public float Value { get; set; }
        public OperationTypeEnum OperationAllowed { get; set; }
        public bool IsOperationSystem { get; set; }
    }

    public class Operation : IOperation
    {
        public Operation()
        {
            IsOperationSystem = false;
        }
        public Operation (Operation input)
        {
            if(input != null)
            {
                Id = input.Id;
                CompteId = input.CompteId;
                DateOperation = input.DateOperation;
                Value = input.Value;
                Caption = input.Caption;
                User = input.User;
                OperationAllowed = input.OperationAllowed;
                IsOperationSystem = input.IsOperationSystem;
            }
        }
        public bool IsOperationSystem { get; set; }

        public Guid? Id { get; set; }
        public Guid CompteId { get; set; }
        public DateTime DateOperation { get; set; }
        public float Value { get; set; }
        public string Caption { get; set; }
        public String User { get; set; }
        public OperationTypeEnum OperationAllowed { get; set; }
    }
}

