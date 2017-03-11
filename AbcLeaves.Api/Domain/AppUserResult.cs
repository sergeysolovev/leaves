using System;
using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Domain
{
    public class AppUserResult : IOperationResult
    {
        public static AppUserResult Success(AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return new AppUserResult { Succeeded = true, User = user };
        }

        public static AppUserResult Fail(string message)
        {
            return new AppUserResult {
                ErrorMessage = message
            };
        }

        public static AppUserResult FailFrom(IOperationResult fromResult)
        {
            return Fail(fromResult.ErrorMessage);
        }

        public AppUser User { get; protected set; }
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }
    }
}
