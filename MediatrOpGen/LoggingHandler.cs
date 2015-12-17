namespace MediatrOpGen
{
    using System.Diagnostics;

    using MediatR;

    public class LoggingHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private IRequestHandler<TRequest, TResponse> inner;


        public LoggingHandler(IRequestHandler<TRequest, TResponse> inner)
        {
            this.inner = inner;
        }

        public TResponse Handle(TRequest message)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            var response = this.inner.Handle(message);
#if DEBUG
            Debug.WriteLine($"{typeof(TRequest).Name} Handled in {sw.ElapsedMilliseconds}ms\n{message.ToString()}");
#endif
            return response;
        }
    }
}