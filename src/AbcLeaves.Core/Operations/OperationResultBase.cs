using System;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public abstract class OperationResultBase : OperationResultBase<DefaultOperationContext>
    {
        protected static DefaultOperationContext defaultContext => new DefaultOperationContext();

        public OperationResultBase() : base()
        {
        }

        public OperationResultBase(DefaultOperationContext context) : base(context)
        {
        }

        protected OperationResultBase(bool succeeded) : base(defaultContext, succeeded)
        {
        }

        protected OperationResultBase(IOperationResult result) : base(result)
        {
        }

        protected OperationResultBase(string error, Dictionary<string, object> details)
            : base(defaultContext, error, details)
        {
        }
    }

    public abstract class OperationResultBase<TContext> : IOperationResult<TContext>
        where TContext : IOperationContext, new()
    {
        public TContext Context { get; set; }
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public Dictionary<string, object> Details { get; protected set; } = new Dictionary<string, object>();

        public void Succeed(TContext context)
        {
            Succeeded = true;
            Context = context;
        }

        public OperationResultBase() : this(new TContext(), default(bool))
        {
        }

        public OperationResultBase(TContext context) : this(context, default(bool))
        {
        }

        protected OperationResultBase(TContext context, bool succeeded)
        {
            SetContext(context);
            Succeeded = succeeded;
        }

        protected OperationResultBase(
            TContext context,
            string error,
            Dictionary<string, object> details)
        {
            SetContext(context);
            ErrorMessage = error;
            if (details != null)
            {
                Details = details;
            }
        }

        protected OperationResultBase(IOperationResult result)
        {
            FailFromInternal(result);
        }

        // todo: implement fail from passing context

        void IOperationResult.FailFrom(IOperationResult result)
        {
            FailFromInternal(result);
        }

        protected virtual void FailFromInternal(IOperationResult result)
        {
            if (result.Succeeded)
            {
                throw new InvalidOperationException();
            }
            ErrorMessage = result.ErrorMessage;
            if (result.Details != null)
            {
                Details = new Dictionary<string, object>(result.Details);
            }
        }

        private void SetContext(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }
    }
}