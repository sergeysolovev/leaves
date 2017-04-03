using System;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public static class OperationExtensions
    {
        public static async Task<TReturn> Return<TReturn>
        (
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

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWithClosure<TReturn, TIn, TOut>
        (
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

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWithClosure<TReturn, TIn, TOut>
        (
            this OperationFlowState<TReturn, TIn> state,
            Func<OperationFlowState<TReturn, TIn>, Task<OperationFlowState<TReturn, TOut>>> closure
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await Task
                .FromResult(state)
                .ProceedWithClosure(closure);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedAnyResultWith<TReturn, TIn, TOut>
        (
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, Task<TOut>> onProceed
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
            if (onProceed == null)
            {
                throw new ArgumentNullException(nameof(onProceed));
            }
            TIn inputResult = state.Current;
            TOut outputResult = await onProceed.Invoke(inputResult);
            return new OperationFlowState<TReturn, TOut>(outputResult);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedAnyResultWith<TReturn, TIn, TOut>
        (
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, TOut> onProceed
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync
                .ProceedAnyResultWith(inputResult => Task.FromResult(onProceed(inputResult)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>
        (
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
                onFail = onFail ?? FailFromAsync<TIn, TOut>();
                outputResult = await onFail.Invoke(inputResult);
            }
            return new OperationFlowState<TReturn, TOut>(outputResult);
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>
        (
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

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>
        (
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
                .ProceedWith(inputResult => Task.FromResult(onSuccess(inputResult)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>
        (
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, TOut> onSuccess,
            Func<TIn, TOut> onFail = null
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync
                .ProceedWith(inputResult => Task.FromResult(onSuccess(inputResult)));
        }

        public static async Task<OperationFlowState<TReturn, TOut>> ProceedWith<TReturn, TIn, TOut>
        (
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            TOut successResult
        )
            where TReturn : IOperationResult
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
        {
            return await stateAsync
                .ProceedWith(inputResult => Task.FromResult(successResult));
        }


        public static async Task<OperationFlowState<TReturn, TReturn>> EndWith<TReturn, TIn>
        (
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            Func<TIn, TReturn> onSuccess
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await stateAsync.ProceedWith(inputResult => onSuccess(inputResult));
        }

        public static async Task<OperationFlowState<TReturn, TReturn>> EndWith<TReturn, TIn>
        (
            this Task<OperationFlowState<TReturn, TIn>> stateAsync,
            TReturn returnResult
        )
            where TReturn : IOperationResult, new()
            where TIn : IOperationResult
        {
            return await stateAsync.ProceedWith(returnResult);
        }

        public static async Task<OperationFlowState<TReturn, TCurrent>> ExitOnFailWith<TReturn, TCurrent>
        (
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

        public static async Task<OperationFlowState<TReturn, TCurrent>> ExitOnFailWith<TReturn, TCurrent>
        (
            this Task<OperationFlowState<TReturn, TCurrent>> stateAsync,
            Func<TCurrent, TReturn> onReturn
        )
            where TReturn : IOperationResult
            where TCurrent : IOperationResult
        {
            return await stateAsync
                .ExitOnFailWith(inputResult => Task.FromResult(onReturn(inputResult)));
        }

        public static async Task<OperationFlowState<TReturn, TCurrent>> ExitOnFailWith<TReturn, TCurrent>
        (
            this Task<OperationFlowState<TReturn, TCurrent>> stateAsync,
            TReturn returnResult
        )
            where TReturn : IOperationResult
            where TCurrent : IOperationResult
        {
            return await stateAsync
                .ExitOnFailWith(inputResult => Task.FromResult(returnResult));
        }

        public static Func<TIn, TOut> FailFrom<TIn, TOut>()
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
            => inputResult => {
                var outputResult = new TOut();
                outputResult.FailFrom(inputResult);
                return outputResult;
            };

        public static Func<TIn, Task<TOut>> FailFromAsync<TIn, TOut>()
            where TIn : IOperationResult
            where TOut : IOperationResult, new()
            => inputResult => Task.FromResult(FailFrom<TIn, TOut>().Invoke(inputResult));
    }
}