using System;
using System.Collections.Generic;
using AbcLeaves.Utils;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class GrantAccessResult
    {
        public bool Succeeded => (Error == null);
        public string Error { get; private set; }
        public bool Forbidden { get; private set; }

        protected GrantAccessResult(bool forbidden)
            => Forbidden = forbidden;

        protected GrantAccessResult(string error)
            => Error = Throw.IfNullOrEmpty(error, nameof(error));

        public static GrantAccessResult Succeed(bool forbidden = false)
            => new GrantAccessResult(forbidden);

        public static GrantAccessResult Fail(string error)
            => new GrantAccessResult(error);
    }
}
