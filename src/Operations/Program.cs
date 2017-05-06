using System;
using Operations;

namespace Operations.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            //OperationTests.TestBuildFamily().Wait();
            int s = sizeof(Func<int, Task<IContext<int>>>);
            Console.WriteLine();
        }
    }
}
