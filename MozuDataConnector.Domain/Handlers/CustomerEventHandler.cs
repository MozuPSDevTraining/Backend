using System;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Events;

namespace MozuDataConnector.Domain.Handlers
{
    public class CustomerEventHandler : ICustomerAccountEvents
    {
        public void Created(IApiContext apiContext, Event eventPayLoad)
        {
            var customerId = eventPayLoad.EntityId;
        }

        public Task CreatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            var customerId = eventPayLoad.EntityId;
            return null;
        }

        public void Deleted(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task DeletedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Updated(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task UpdatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            var customerId = eventPayLoad.EntityId;
            return null;
        }
    }
}
