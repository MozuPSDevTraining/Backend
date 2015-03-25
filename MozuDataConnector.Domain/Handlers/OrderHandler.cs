using Mozu.Api;
using Mozu.Api.Resources.Commerce;
using Mozu.Api.Resources.Commerce.Orders;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
using Mozu.Api.Contracts.CommerceRuntime.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MozuDataConnector.Domain.Handlers
{
    public class OrderHandler
    {
        private Mozu.Api.IApiContext _apiContext;


        public async Task<Order> GetOrder(int tenantId, int? siteId, int? masterCatalogId, string orderId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderResource = new OrderResource(_apiContext);
            var order = await orderResource.GetOrderAsync(orderId, null);

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrders(int tenantId, int? siteId, int? masterCatalogId, 
            int? startIndex, int? pageSize, string sortBy = null, string filter = null)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderResource = new OrderResource(_apiContext);
            var orders = await orderResource.GetOrdersAsync(startIndex, pageSize, sortBy, filter, null);

            return orders.Items;
        }

        public async Task<Order> CreatePaymentAction(int tenantId, int? siteId, int? masterCatalogId, PaymentAction action, Order existingOrder)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var paymentResource = new PaymentResource(_apiContext);

            var available = await paymentResource.GetAvailablePaymentActionsAsync(existingOrder.Id, existingOrder.Payments[0].Id);

            var order = await paymentResource.CreatePaymentActionAsync(action, existingOrder.Id);

            return order;
        }
    }
}
