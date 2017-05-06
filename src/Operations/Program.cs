using System;
using Operations;

namespace Operations.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            OperationTests.TestBuildFamily().Wait();
        }
    }
}
