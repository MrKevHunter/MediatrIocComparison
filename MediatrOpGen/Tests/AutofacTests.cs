namespace MediatrOpGen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Autofac;
    using Autofac.Core;
    using Autofac.Features.Variance;

    using MediatR;

    using Xunit;

    public class AutofacTests
    {
        [Fact]
        public void AutofacTest()
        {
            var container = BuildContainer();
            var mediator = container.Resolve<IMediator>();
            mediator.Send(new InputObject());
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder.Register<SingleInstanceFactory>(
                ctx =>
                    {
                        var c = ctx.Resolve<IComponentContext>();
                        return t => c.Resolve(t);
                    });

            builder.Register<MultiInstanceFactory>(
                ctx =>
                    {
                        var c = ctx.Resolve<IComponentContext>();
                        return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
                    });
            this.RegisterImplementors(builder, typeof(IPreRequestHandler<>));
            this.RegisterImplementors(builder, typeof(IPostRequestHandler<,>));

            // register all asynchandlers
            var thisAssembly = this.GetType().GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(thisAssembly)
                .As(
                    t =>
                    t.GetTypeInfo()
                        .ImplementedInterfaces.Where(i => i.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                        .Select(i => new KeyedService("RequestHandler", i)))
                .InstancePerLifetimeScope();

            // register pipeline decorators
            builder.RegisterGenericDecorator(typeof(MediatorPipeline<,>), typeof(IRequestHandler<,>), "RequestHandler")
                .InstancePerLifetimeScope();

            return builder.Build();
        }

        private void RegisterImplementors(ContainerBuilder builder, Type interfaceType)
        {
            var thisAssembly = this.GetType().GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(thisAssembly)
                .Where(x => x == interfaceType)
                .As(t => t.GetTypeInfo().ImplementedInterfaces)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}