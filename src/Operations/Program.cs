using System;
using Operations;
using Operations.Linq;

namespace Operations.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = from v1 in Operation.Return<string>("v1")
                    from v2 in Operation.Return<string>("v2")
                    let s = v1 + v2
                    where s == "v1v2"
                    select s;
            var r = x.ExecuteAsync().Result;
            Console.WriteLine(r.Succeeded);
            Console.WriteLine(r.Result);
        }
    }
}
