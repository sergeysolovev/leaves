using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ExperimentsV2
{
    public class UserResult {}
    public class UserTokensResult {}

    public interface IUserManager
    {
        Operator<UserResult> GetCurrentUserAsync();
        Operator<UserTokensResult> GetUserTokens(Get<UserResult> getUser);
    }

    public static class Tests
    {
        public static Task<object> Method()
        {
            return Task.FromResult(new object());
        }

        public static Task<object> FailedMethod()
        {
            // something exceptional happend
            // but we managed to handle it:
            try
            {
                throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                // here we log something and throw further
                throw ex;
            }
        }

        public static Operator<object> FailedOperation()
        {
            return new Operator<object>(new LazyAsync<object>(() => {
                throw new InvalidOperationException();
            }));
        }

        public static async void Test1()
        {
            IUserManager userManager = null;
            var t = from user in userManager.GetCurrentUserAsync()
                    let x = 1
                    select user;

            var f = new Operator<object>(new LazyAsync<object>(FailedMethod));

            var r = from user in userManager.GetCurrentUserAsync()
                    from anotherUser in userManager.GetCurrentUserAsync()
                    from failure in f
                    from tokens in userManager.GetUserTokens(user)
                    from anotherTokens in userManager.GetUserTokens(anotherUser)
                    let y = user
                    select tokens;

            var operationResult = await r.Value;
        }
    }

    ///

    public delegate Task<TResult> Get<TResult>();

    public class LazyAsync<T> : Lazy<Task<T>>
    {
        private LazyAsync(Func<Task<T>> valueFactory) : base(() => Task.Run(valueFactory)) {}
        //public LazyAsync(Func<T> valueFactory) : base(() => Task.Run(valueFactory)) {}
        public LazyAsync(Get<T> get) : this((Func<Task<T>>)(() => get())) {}
        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
        public Task<T> ExecuteAsync() => Value;
        public static implicit operator Get<T>(LazyAsync<T> lazyAsync) => () => lazyAsync.Value;
    }

    // In this implementation Functor is actually Identity<T>
    public class Functor<T>
    {
        public T Value { get; }
        public Functor(T value) { Value = value; }
    }

    // Operator<T> === LazyAsync<T>
    // Operator<T> is a wrapper
    public class Operator<T> : Functor<LazyAsync<T>>
    {
        public Task<T> ExecuteAsync()       => Value.ExecuteAsync();
        public TaskAwaiter<T> GetAwaiter()  => Value.GetAwaiter();
        public bool IsCompleted             => Value.IsValueCreated;
        public Operator(LazyAsync<T> lazyAsync) : base(lazyAsync) {}
    }

    // v2
    public class Operation<T> : Functor<Get<T>>
    {
        private readonly LazyAsync<T> lazyAsync;

        public Task<T> ExecuteAsync() => lazyAsync.Value;
        public TaskAwaiter<T> GetAwaiter() => lazyAsync.GetAwaiter();
        public bool IsCompleted => lazyAsync.IsValueCreated;

        public Operation(Get<T> executeAsync) : base(executeAsync)
        {
            this.lazyAsync = new LazyAsync<T>(Value);
        }
    }

    public static class FunctorExtensions
    {
        public static Functor<TResult> SelectMany<TSource, TNext, TResult>(
            this Functor<TSource> source,
            Func<TSource, Functor<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
        {
            return resultSelector(source.Value, selector(source.Value).Value).ToFunctor();
        }

        public static Functor<TResult> Select<TSource, TResult>(
            this Functor<TSource> source,
            Func<TSource, TResult> selector)
        {
            return Bind(source, f => ToFunctor(selector(f)));
        }

        private static Functor<TResult> ToFunctor<TResult>(this TResult value)
        {
            return new Functor<TResult>(value);
        }

        private static Functor<TResult> Bind<TSource, TResult>(
            this Functor<TSource> source,
            Func<TSource, Functor<TResult>> selector)
        {
            return selector(source.Value);
        }
    }

    public static class OperationExtensions
    {
        public static Operation<TResult> ToIdentityOperation<TResult>(this TResult result)
        {
            return ToOperation(() => Task.FromResult(result));
        }

        public static Operation<TResult> ToOperation<TResult>(
            this Functor<Get<TResult>> functor)
        {
            return ToOperation(functor.Value);
        }

        public static Operation<TResult> ToOperation<TResult>(
            this Get<TResult> executeAsync)
        {
            return new Operation<TResult>(executeAsync);
        }
        public static Operator<TResult> ToOperator<TResult>(
            this LazyAsync<TResult> executeAsync)
        {
            return new Operator<TResult>(executeAsync);
        }

        /// Bind – Operation, Operator
        public static Operation<TResult> Bind<TSource, TResult>(
            this Operation<TSource> source,
            Func<Get<TSource>, Operation<TResult>> binder)
        {
            return binder(source.Value);
        }
        public static Operator<TResult> Bind<TSource, TResult>(
            this Operator<TSource> source,
            Func<LazyAsync<TSource>, Operator<TResult>> binder)
        {
            return binder(source.Value);
        }

        /// Select – Operation, Operator
        public static Operation<TResult> Select<TSource, TResult>(
            Operation<TSource> source,
            Func<Get<TSource>, Get<TResult>> selector)
        {
            return Bind(source, f => ToOperation(selector(f)));
        }
        public static Operator<TResult> Select<TSource, TResult>(
            Operator<TSource> source,
            Func<LazyAsync<TSource>, LazyAsync<TResult>> selector)
        {
            return Bind(source, f => ToOperator(selector(f)));
        }

        // valid Select for Operation<T>, Operator<T> 7.15.3
        public static Operation<TResult> Select<TSource, TResult>(
            this Operation<TSource> source,
            Func<TSource, TResult> selector)
        {
            return Select<TSource, TResult>(source, f => () => Select(f(), selector));
        }
        public static Operator<TResult> Select<TSource, TResult>(
            this Operator<TSource> source,
            Func<TSource, TResult> selector)
        {
            return Select<TSource, TResult>(source, la
                => new LazyAsync<TResult>(() => Select(la.Value, selector)));
        }

        // valid Select for Task<T> 7.15.3
        private static async Task<TResult> Select<TSource, TResult>(
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

