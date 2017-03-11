using System;
using System.Collections.Generic;
using ABC.Leaves.Api.Models;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.Domain
{
    public class LeaveApplyResult : IOperationResult
    {
        private LeaveApplyResult() {}

        public static LeaveApplyResult Success(Leave leave)
        {
            if (leave == null)
            {
                throw new ArgumentNullException(nameof(leave));
            }
            return new LeaveApplyResult {
                Succeeded = true,
                Leave = leave
            };
        }

        public static LeaveApplyResult Fail(string errorMessage)
        {
            return new LeaveApplyResult {
                ErrorMessage = errorMessage
            };
        }

        public static LeaveApplyResult Fail(string errorMessage,
            Dictionary<string, object> validationErrors)
        {
            return new LeaveApplyResult {
                ErrorMessage = errorMessage,
                ValidationErrors = validationErrors
            };
        }

        public static LeaveApplyResult FailFrom(IOperationResult fromResult)
        {
            return Fail(fromResult.ErrorMessage);
        }

        [JsonIgnore]
        public Leave Leave { get; protected set; }
        public int? LeaveId => Leave?.Id;
        public Dictionary<string, object> ValidationErrors { get; protected set; }
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }
    }
}
