using System;
using Operations;

namespace Operations.Exe
{
    // two problems:
    // 1) ability to fuck up method Return, see FuckingBuilder
    // 2) ability to use one builder for another

    public class FuckingBuilder : Builder<object, FuckingBuilder>
    {
        public FuckingBuilder(IOperation<object> source) : base(source) { }

        public override FuckingBuilder Return(IOperation<object> source)
            => this;
    }

    public class MotherFuckingBuilder : Builder<object, FuckingBuilder>
    {
        public MotherFuckingBuilder()
            : base(Operation.ReturnNew<object>()) { }

        public override FuckingBuilder Return(IOperation<object> source)
            => new FuckingBuilder(source);
    }

    class Program
    {
        static void Main(string[] args)
        {
            OperationTests.TestBuildFamily().Wait();
            Console.WriteLine();
        }
    }
}
