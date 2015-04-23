using Autofac;
using Mozu.Api.Events;
using MozuDataConnector.Domain.Handlers;

namespace MozuDataConnector.Domain
{
    public class Bootstrapper
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ApplicationEventHandler>().As<IApplicationEvents>();
            containerBuilder.RegisterType<OrderEventHandler>().As<IOrderEvents>();
            containerBuilder.RegisterType<CustomerEventHandler>().As<ICustomerAccountEvents>();

        }
    }
}
