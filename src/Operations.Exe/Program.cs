using System;
using Operations;
using Operations.Linq;

namespace Operations.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = from v1 in Operation.Get<string>("v1")
                    from v2 in Operation.None<string>()
                    select v1 + v2;
        }
    }
}
