using System;
using System.Net.Http;

namespace AbcLeaves.Utils
{
    public class SendMessageResult : IDisposable
    {
        private Exception error;
        private HttpResponseMessage response;
        private bool disposed;

        public HttpResponseMessage Response => disposed ?
            throw new ObjectDisposedException(nameof(response)) :
            response;

        public Exception Error => error;

        public bool Succeeded => (Error == null);

        private SendMessageResult(HttpResponseMessage response)
            => this.response = Throw.IfNull(response, nameof(response));

        private SendMessageResult(Exception error)
            => this.error = Throw.IfNull(error, nameof(error));

        public static SendMessageResult Succeed(HttpResponseMessage response)
            => new SendMessageResult(response);

        public static SendMessageResult Fail(Exception error)
            => new SendMessageResult(error);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Response != null)
                {
                    Response.Dispose();
                    this.disposed = true;
                }
            }
        }
    }
}
