using System;
using System.Collections.Generic;
using AbcLeaves.Api.Models;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class UserResult : OperationResultBase
    {
        [JsonIgnore]
        public string Email { get; protected set; }

        public AppUser User { get; protected set; }

        public UserResult() : base()
        {
        }

        protected UserResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected UserResult(IOperationResult result)
            : base(result)
        {
        }

        protected UserResult(AppUser user) : base(true)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            User = user;
            Email = user.Email;
        }

        public static UserResult Success(AppUser user) => new UserResult(user);

        public static UserResult Fail(string error) => new UserResult(error, null);

        public static UserResult Fail(string error, Dictionary<string, object> details)
            => new UserResult(error, details);

        public static UserResult FailFrom(IOperationResult result)
            => new UserResult(result);
    }
}
