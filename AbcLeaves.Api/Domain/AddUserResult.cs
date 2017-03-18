using System.Collections.Generic;
using AbcLeaves.Api.Models;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class AddUserResult : UserResult
    {
        [JsonIgnore]
        public bool AlreadyExists { get; protected set; }

        public AddUserResult() : base()
        {
        }

        protected AddUserResult(IOperationResult result) : base(result)
        {
        }

        protected AddUserResult(AppUser user, bool alreadyExists = false) : base(user)
        {
            AlreadyExists = alreadyExists;
        }

        protected AddUserResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        public static new AddUserResult Success(AppUser user)
            => new AddUserResult(user);

        public static AddUserResult SuccessAlreadyExists(AppUser user)
            => new AddUserResult(user, alreadyExists: true);

        public static new AddUserResult Fail(string error)
            => new AddUserResult(error, null);

        public static new AddUserResult Fail(string error, Dictionary<string, object> details)
            => new AddUserResult(error, details);

        public static new AddUserResult FailFrom(IOperationResult result)
            => new AddUserResult(result);
    }
}
