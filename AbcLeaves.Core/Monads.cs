using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Experiments
{
    public class UserResult {}
    public class UserTokensResult {}

    public interface IUserManager
    {
        Operation<UserResult> GetCurrentUserAsync();
        Operation<UserTokensResult> GetUserTokens(Get<UserResult> getUser);
    }

    public delegate Task<TResult> Get<TResult>();

    // what if instead of delegate Get<T> we used a wrapper?

    public class GetWrapper<TResult>
    {
        public Get<TResult> Receive()
        {
            return () => Task.FromResult(default(TResult));
        }
    }

    public static class Tests
    {
        public static Task<object> Method()
        {
            return Task.FromResult(new object());
        }

        public static async void Test1()
        {
            IUserManager userManager = null;
            var r = from user in userManager.GetCurrentUserAsync()
                    from tokens in userManager.GetUserTokens(user)
                    select tokens;
            var operationResult = await r.Value();
        }
    }

    public class Functor<T>
    {
        public T Value { get; }
        public Functor(T value) { Value = value; }
    }

    // v2
    public class Operation<TResult> : Functor<Get<TResult>>
    {
        private readonly Lazy<Task<TResult>> lazyExecuteAsync;

        public Task<TResult> ExecuteAsync() => lazyExecuteAsync.Value;

        public TaskAwaiter<TResult> GetAwaiter() => ExecuteAsync().GetAwaiter();

        public bool IsCompleted => lazyExecuteAsync.IsValueCreated;

        public Operation(Func<TResult> execute) :
            base(() => Task.Run(execute))
        {
            this.lazyExecuteAsync = new Lazy<Task<TResult>>(() => Value());
        }

        public Operation(Get<TResult> executeAsync) :
            base(() => Task.Run(() => executeAsync()))
        {
            this.lazyExecuteAsync = new Lazy<Task<TResult>>(() => Value());
        }
    }

    // v1
    // public class Operation<TResult> : Functor<Func<Task<TResult>>>
    // {
    //     private readonly Lazy<Task<TResult>> lazyExecuteAsync;

    //     public Task<TResult> ExecuteAsync() => lazyExecuteAsync.Value;

    //     public TaskAwaiter<TResult> GetAwaiter() => ExecuteAsync().GetAwaiter();

    //     public bool IsCompleted => lazyExecuteAsync.IsValueCreated;

    //     public Operation(Func<TResult> execute) :
    //         base(() => Task.Run(execute))
    //     {
    //         this.lazyExecuteAsync = new Lazy<Task<TResult>>(Value);
    //     }

    //     public Operation(Func<Task<TResult>> executeAsync) :
    //         base(() => Task.Run(executeAsync))
    //     {
    //         this.lazyExecuteAsync = new Lazy<Task<TResult>>(Value);
    //     }
    // }

    public static class FunctorExtensions
    {
        public static Functor<TResult> ToFunctor<TResult>(
            this TResult value)
        {
            return new Functor<TResult>(value);
        }

        public static Functor<TResult> SelectMany<TSource, TNext, TResult>(
            this Functor<TSource> source,
            Func<TSource, Functor<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
        {
            return resultSelector(source.Value, selector(source.Value).Value).ToFunctor();
        }
    }

    public static class OperationExtensions
    {
        public static Operation<TResult> ToIdentityOperation<TResult>(this TResult result)
        {
            return ToOperation(() => result);
        }

        public static Operation<TResult> ToOperation<TResult>(
            this Functor<Get<TResult>> functor)
        {
            return ToOperation(functor.Value);
        }

        public static Operation<TResult> ToOperation<TResult>(
            this Func<TResult> execute)
        {
            return new Operation<TResult>(execute);
        }

        public static Operation<TResult> ToOperation<TResult>(
            this Get<TResult> executeAsync)
        {
            return new Operation<TResult>(executeAsync);
        }

        // Pure SelectMany
        public static Operation<TResult> Bind<TSource, TResult>(
            this Operation<TSource> source,
            Func<TSource, Operation<TResult>> selector)
        {
            throw new NotImplementedException();
        }

        // Pure SelectMany
        public static Operation<TResult> SelectMany<TSource, TNext, TResult>(
            this Operation<TSource> source,
            Func<TSource, Operation<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        // public static Operation<TResult> SelectMany<TSource, TNext, TResult>(
        //     this Operation<TSource> source,
        //     Func<Func<Task<TSource>>, Operation<TNext>> selector,
        //     Func<Func<Task<TSource>>, Func<Task<TNext>>, Func<Task<TResult>>> resultSelector)
        // {
        //     return resultSelector(source.Value, selector(source.Value).Value).ToOperation();
        // }

        public static Operation<TResult> Bind<TSource, TResult>(
            this Operation<TSource> source,
            Func<Get<TSource>, Operation<TResult>> binder)
        {
            return binder(source.Value);
        }

        public static Operation<TResult> Select<TSource, TResult>(
            Operation<TSource> source,
            Func<Get<TSource>, Get<TResult>> selector)
        {
            return Bind(source, f => ToOperation(selector(f)));
        }

        public static Operation<TResult> Map<TSource, TResult>(
            Operation<TSource> source,
            Func<Get<TSource>, Get<TResult>> mapper)
        {
            return Select(source, mapper);
        }

        // valid Select for Operation<T> 7.15.3
        public static Operation<TResult> Map<TSource, TResult>(
            this Operation<TSource> source,
            Func<TSource, TResult> mapper)
        {
            return Select<TSource, TResult>(source, f => () => Map(f(), mapper));
        }

        // valid Select for Task<T> 7.15.3
        private static async Task<TResult> Map<TSource, TResult>(
            Task<TSource> source,
            Func<TSource, TResult> mapper)
        {
            return mapper(await source);
        }
    }

    class StructValueResult
    {
        public class Internal { public string TypeName => typeof(StructValueResult).Name; }
        public Internal Value { get; set; }
    }

    class StringResult
    {
        public StringResult(string value) { Value = value; }
        public string Value { get; set; }
    }
}

