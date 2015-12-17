namespace MediatrOpGen
{
    using MediatR;

    using Ninject;
    using Ninject.Extensions.Conventions;
    using Ninject.Planning.Bindings.Resolvers;

    using Xunit;

    public class NinjectTest
    {
        [Fact]
        public void TestNinject()
        {
            var kernel = this.BindNinject();
            var mediator = kernel.Get<IMediator>();
            var response = mediator.Send(new InputObject());
        }

        private IKernel BindNinject()
        {
            IKernel standardKernel = new StandardKernel();
            standardKernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            standardKernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
          
            standardKernel.Bind<SingleInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.Get(t));
            standardKernel.Bind<MultiInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.GetAll(t));

            var openGenericRequestHandler = typeof(IRequestHandler<,>);
            var mediatorPipeline = typeof(MediatorPipeline<,>);
            var cachingHandler = typeof(LoggingHandler<,>);
            standardKernel.Bind(
                x =>
                {
                    x.From(this.GetType().Assembly)
                        .SelectAllClasses()
                        .InheritedFrom(openGenericRequestHandler)
                        .BindAllInterfaces()
                        .Configure(syntax => syntax.WhenInjectedInto(cachingHandler));
                });


            standardKernel.Bind(openGenericRequestHandler).To(cachingHandler).WhenInjectedInto(mediatorPipeline);

            standardKernel.Bind(openGenericRequestHandler).To(mediatorPipeline);


            // May need to Bindall interfaces in some cases?
            standardKernel.Bind(
                x =>
                x.From(this.GetType().Assembly)
                    .SelectAllClasses()
                    .InheritedFrom(typeof(IPreRequestHandler<>))
                    .BindAllInterfaces());
            standardKernel.Bind(
                x =>
                x.From(this.GetType().Assembly)
                    .SelectAllClasses()
                    .InheritedFrom(typeof(IPostRequestHandler<,>))
                    .BindAllInterfaces());
            return standardKernel;
        }
    }
}