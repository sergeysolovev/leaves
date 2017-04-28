using System.Collections.Generic;
using AbcLeaves.Api.Models;
using AbcLeaves.Core;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class FindUserResult : UserResult, INotFoundOperationResult
    {
        [JsonIgnore]
        public bool NotFound { get; protected set; }

        public FindUserResult() : base()
        {
        }

        protected FindUserResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected FindUserResult(IOperationResult result)
            : base(result)
        {
        }

        protected FindUserResult(AppUser user) : base(user)
        {
        }

        public static new FindUserResult Success(AppUser user) => new FindUserResult(user);

        public static new FindUserResult Fail(string error) => new FindUserResult(error, null);

        public static new FindUserResult Fail(string error, Dictionary<string, object> details)
            => new FindUserResult(error, details);

        public static new FindUserResult FailFrom(IOperationResult result)
            => new FindUserResult(result);

        public static FindUserResult FailNotFound(string email)
        {
            var error = $"User '{email}' is not found";
            return new FindUserResult(error, null) {
                NotFound = true,
                Email = email
            };
        }

        public static FindUserResult FailNotFoundById(string userId)
        {
            var error = $"User id='{userId}' is not found";
            return new FindUserResult(error, null) {
                NotFound = true
            };
        }
    }
}
