namespace MediatrOpGen
{
    using MediatR;

    using StructureMap;

    using Xunit;

    public class StructureMapTests
    {
        [Fact]
        public void TestSM()
        {
            var buildStructureMap = BuildStructureMap();
            var instance = buildStructureMap.GetInstance<IMediator>();
            instance.Send(new InputObject());
        }

        private static Container BuildStructureMap()
        {
            var container = new Container(
                cfg =>
                    {
                        cfg.Scan(
                            scanner =>
                                {
                                    scanner.AssemblyContainingType<IMediator>();
                                    scanner.AssemblyContainingType<StructureMapTests>();
                                    scanner.WithDefaultConventions();
                                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                                    scanner.ConnectImplementationsToTypesClosing(typeof(IPreRequestHandler<>));
                                    scanner.ConnectImplementationsToTypesClosing(typeof(IPostRequestHandler<,>));
                                });
                        cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                        cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                        var handlerType = cfg.For(typeof(IRequestHandler<,>));
                        handlerType.DecorateAllWith(typeof(LoggingHandler<,>));
                        handlerType.DecorateAllWith(typeof(MediatorPipeline<,>));
                    });
            return container;
        }
    }
}