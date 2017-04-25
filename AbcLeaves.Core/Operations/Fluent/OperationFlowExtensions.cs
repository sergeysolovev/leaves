using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public static class OperationFlowStateExtensions
    {
        public static OperationFlowState<TReturn> Fold<TReturn>(
            this OperationFlowState<TReturn, TReturn> state
        )
            where TReturn : IOperationResult
        {
            return new OperationFlowState<TReturn> {
                Current = state.Current,
                Return = state.Return
            };
        }

        public async static Task<OperationFlowState<TReturn>> Fold<TReturn>(
            this Task<OperationFlowState<TReturn, TReturn>> asyncState
        )
            where TReturn : IOperationResult
        {
            return Fold(await asyncState);
        }

        public static OperationFlowState<TReturn, TReturn> Unfold<TReturn>(
            this OperationFlowState<TReturn> state
        )
            where TReturn : IOperationResult
        {
            return new OperationFlowState<TReturn, TReturn>{
                Current = state.Current,
                Return = state.Return
            };
        }

        public static async Task<OperationFlowState<TReturn, TReturn>> Unfold<TReturn>(
            this Task<OperationFlowState<TReturn>> asyncState
        )
            where TReturn : IOperationResult
        {
            return Unfold(await asyncState);
        }

        public static async Task<TReturn> Return<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync
        )
            where TReturn : IOperationResult
        {
            return await stateAsync.Unfold().Return();
        }

        public static async Task<TReturn> Return<TReturn>(
            this Task<OperationFlowState<TReturn, TReturn>> stateAsync
        )
            where TReturn : IOperationResult
        {
            var state = await stateAsync;
            if (state.Return != null)
            {
                return state.Return;
            }
            if (state.Current == null)
            {
                throw new ArgumentNullException(nameof(state.Current));
            }
            return state.Current;
        }

        // todo: get rid of new() so we can work with abstractions
        public static async Task<OperationFlowState<TResult>> ExecuteSequence<TResult, TContext>(
            this IEnumerable<IOperation<TResult, TContext>> operations,
            TContext contextSeed
        )
            where TResult : IOperationResult<TContext>, new()
            where TContext : IOperationContext
        {
            var identity = IdentityOperation<TResult, TContext>.Create();
            var seed = identity.Begin(contextSeed);
            return await operations
                .Aggregate(seed, (state, operation) =>
                    state.EndWith(currentResult =>
                        operation.ExecuteAsync(currentResult.Context)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWithClosure<TReturn, TOut>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<OperationFlowState<TReturn>, Task<OperationFlowState<TReturn, TOut>>> closure
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync.Unfold().ProceedWithClosure(x => closure(Fold(x)));
        }

        public static async Task<OperationFlowState<TReturn>> ProceedWithClosure<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<OperationFlowState<TReturn>, Task<OperationFlowState<TReturn>>> closure
        )
            where TReturn : IOperationResult, new()
        {
            return await stateAsync.ProceedWithClosure(x => Unfold(closure(x))).Fold();
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWithClosure<TReturn, TOut>(
            this OperationFlowState<TReturn> state,
            Func<OperationFlowState<TReturn>, Task<OperationFlowState<TReturn, TOut>>> closure
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task.FromResult(state).ProceedWithClosure(closure);
        }

        public static async Task<OperationFlowState<TReturn>> ProceedWithClosure<TReturn>(
            this OperationFlowState<TReturn> state,
            Func<OperationFlowState<TReturn>, Task<OperationFlowState<TReturn>>> closure
        )
            where TReturn : IOperationResult, new()
        {
            return await Task.FromResult(state).ProceedWithClosure(closure);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWithClosure<TReturn, TIn, TOut>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<OperationFlowState<TReturn, TIn>, Task<OperationFlowState<TReturn, TOut>>> closure
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            var state = await stateAsync;
            if (state.Return != null)
            {
                return new OperationFlowState<TReturn, TOut>(state.Return);
            }
            if (closure == null)
            {
                throw new ArgumentNullException(nameof(closure));
            }
            if (state.Current == null)
            {
                throw new ArgumentNullException(nameof(state.Current));
            }
            TIn inputResult = state.Current;
            if (inputResult.Succeeded)
            {
                return await closure.Invoke(state);
            }
            else
            {
                var outputResult = await FailFromAsync<TIn, TOut>()(inputResult);
                return new OperationFlowState<TReturn, TOut>(outputResult);
            }
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWithClosure<TReturn, TIn, TOut>(
            this OperationFlowState<TReturn, TIn> state,
            Func<OperationFlowState<TReturn, TIn>, Task<OperationFlowState<TReturn, TOut>>> closure
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task.FromResult(state).ProceedWithClosure(closure);
        }

        public static async Task<OperationFlowState<TReturn>> ProceedWithClosure<TReturn, TIn>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<OperationFlowState<TReturn, TIn>, Task<OperationFlowState<TReturn>>> closure
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await stateAsync.ProceedWithClosure(x => Unfold(closure(x))).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> ProceedWithClosure<TReturn, TIn>(
            this OperationFlowState<TReturn, TIn> state,
            Func<OperationFlowState<TReturn, TIn>, Task<OperationFlowState<TReturn>>> closure
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await Task.FromResult(state).ProceedWithClosure(closure);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedAnyResultWith<TReturn, TOut>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, Task<TOut>> onProceed
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult
        {
            return await stateAsync.Unfold().ProceedAnyResultWith(onProceed);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedAnyResultWith<TReturn, TOut>(
            this OperationFlowState<TReturn> state,
            Func<TReturn, Task<TOut>> onProceed
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult
        {
            return await Task.FromResult(state).ProceedAnyResultWith(onProceed);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedAnyResultWith<TReturn, TIn, TOut>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, Task<TOut>> onProceed
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult
        {
            var state = await stateAsync;
            if (state.Return != null)
            {
                return new OperationFlowState<TReturn, TOut>(state.Return);
            }
            if (onProceed == null)
            {
                throw new ArgumentNullException(nameof(onProceed));
            }
            TIn inputResult = state.Current;
            TOut outputResult = await onProceed.Invoke(inputResult);
            return new OperationFlowState<TReturn, TOut>(outputResult);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedAnyResultWith<TReturn, TIn, TOut>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, TOut> onProceed
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync
                .ProceedAnyResultWith(x => Task.FromResult(onProceed(x)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TOut>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, Task<TOut>> onSuccess,
            Func<TReturn, Task<TOut>> onFail = null
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync.Unfold().ProceedWith(onSuccess, onFail);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TOut>(
            this OperationFlowState<TReturn> stateAsync,
            Func<TReturn, Task<TOut>> onSuccess,
            Func<TReturn, Task<TOut>> onFail = null
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task.FromResult(stateAsync).ProceedWith(onSuccess, onFail);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TOut>(
            this OperationFlowState<TReturn> state,
            Func<TReturn, TOut> onSuccess,
            Func<TReturn, TOut> onFail = null
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task
                .FromResult(state)
                .ProceedWith(x => Task.FromResult(onSuccess(x)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TOut>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, TOut> onSuccess,
            Func<TReturn, TOut> onFail = null
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync.ProceedWith(x => Task.FromResult(onSuccess(x)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TOut>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            TOut successResult
        )
            where TReturn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync.ProceedWith(x => Task.FromResult(successResult));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, Task<TOut>> onSuccess,
            Func<TIn, Task<TOut>> onFail = null
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            var state = await stateAsync;
            if (state.Return != null)
            {
                return new OperationFlowState<TReturn, TOut>(state.Return);
            }
            if (onSuccess == null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }
            TIn inputResult = state.Current;
            TOut outputResult;
            if (inputResult.Succeeded)
            {
                outputResult = await onSuccess.Invoke(inputResult);
            }
            else
            {
                // OperationFlowState = (Return: TResult, Failure: TResult, Success: different types) <- OperationResult
                // OperationFlow <- Operation (cz we can execute it)
                // we need to know how to create only TResult, not everything!
                //

                // what if on fail we set RETURN=input (already has instance)
                // CURRENT=null

                // ? how to convert to (TReturn)
                // ? how about ExitOnFailWith

                onFail = onFail ?? FailFromAsync<TIn, TOut>();
                outputResult = await onFail.Invoke(inputResult);
            }
            return new OperationFlowState<TReturn, TOut>(outputResult);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>(
            this OperationFlowState<TReturn, TIn> state,
            Func<TIn, Task<TOut>> onSuccess,
            Func<TIn, Task<TOut>> onFail = null
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task
                .FromResult(state)
                .ProceedWith(onSuccess, onFail);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>(
            this OperationFlowState<TReturn, TIn> state,
            Func<TIn, TOut> onSuccess,
            Func<TIn, TOut> onFail = null
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task
                .FromResult(state)
                .ProceedWith(x => Task.FromResult(onSuccess(x)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, TOut> onSuccess,
            Func<TIn, TOut> onFail = null
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync
                .ProceedWith(x => Task.FromResult(onSuccess(x)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            TOut successResult
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync.ProceedWith(x => Task.FromResult(successResult));
        }

        public static async Task<OperationFlowState<TReturn>> EndWith<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, Task<TReturn>> onSuccess
        )
            where TReturn : IOperationResult, new()
        {
            return await stateAsync.Unfold().EndWith(onSuccess);
        }

        public static async Task<OperationFlowState<TReturn>> EndWith<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, TReturn> onSuccess
        )
            where TReturn : IOperationResult, new()
        {
            return await stateAsync.Unfold().EndWith(onSuccess);
        }

        public static async Task<OperationFlowState<TReturn>> EndWith<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            TReturn returnResult
        )
            where TReturn : IOperationResult, new()
        {
            return await stateAsync.Unfold().EndWith(returnResult);
        }

        public static async Task<OperationFlowState<TReturn>> EndWith<TReturn, TIn>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, Task<TReturn>> onSuccess
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await stateAsync.ProceedWith(x => onSuccess(x)).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> EndWith<TReturn, TIn>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, TReturn> onSuccess
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await stateAsync.ProceedWith(x => onSuccess(x)).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> EndWith<TReturn, TIn>(
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            TReturn returnResult
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await stateAsync.ProceedWith(returnResult).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> ExitOnFailWith<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, Task<TReturn>> onReturn
        )
            where TReturn : IOperationResult
        {
            return await stateAsync.Unfold().ExitOnFailWith(onReturn).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> ExitOnFailWith<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            Func<TReturn, TReturn> onReturn
        )
            where TReturn : IOperationResult
        {
            return await stateAsync.Unfold().ExitOnFailWith(onReturn).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> ExitOnFailWith<TReturn>(
            this Task<OperationFlowState<TReturn>> stateAsync,
            TReturn returnResult
        )
            where TReturn : IOperationResult
        {
            return await stateAsync.Unfold().ExitOnFailWith(returnResult).Fold();
        }

        public static async Task<OperationFlowState<TReturn, TCurrent>> ExitOnFailWith<TReturn, TCurrent>(
            this Task<OperationFlowState<TReturn, TCurrent>> stateAsync,
            Func<TCurrent, Task<TReturn>> onReturn
        )
            where TReturn : IOperationResult
            where TCurrent : IOperationResult
        {
            var state = await stateAsync;
            if (state.Return != null)
            {
                return state;
            }
            if (onReturn == null)
            {
                throw new ArgumentNullException(nameof(onReturn));
            }
            TCurrent inputResult = state.Current;
            if (!inputResult.Succeeded)
            {
                state.Return = await onReturn.Invoke(inputResult);
            }
            return state;
        }

        public static async Task<OperationFlowState<TReturn, TCurrent>> ExitOnFailWith<TReturn, TCurrent>(
            this Task<OperationFlowState<TReturn, TCurrent>> stateAsync,
            Func<TCurrent, TReturn> onReturn
        )
            where TReturn : IOperationResult
            where TCurrent : IOperationResult
        {
            return await stateAsync.ExitOnFailWith(x => Task.FromResult(onReturn(x)));
        }

        public static async Task<OperationFlowState<TReturn, TCurrent>> ExitOnFailWith<TReturn, TCurrent>(
            this Task<OperationFlowState<TReturn, TCurrent>> stateAsync,
            TReturn returnResult
        )
            where TReturn : IOperationResult
            where TCurrent : IOperationResult
        {
            return await stateAsync.ExitOnFailWith(x => Task.FromResult(returnResult));
        }

        public static Func<TIn, TOut> FailFrom<TIn, TOut>()
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
            => x => {
                var output = new TOut();
                output.FailFrom(x);
                return output;
            };

        public static Func<TIn, Task<TOut>> FailFromAsync<TIn, TOut>()
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
            => x => Task.FromResult(FailFrom<TIn, TOut>().Invoke(x));
    }
}