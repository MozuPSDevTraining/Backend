using Mozu.Api;
using Mozu.Api.Resources.Commerce;
using Mozu.Api.Resources.Commerce.Orders;
using Mozu.Api.Resources.Commerce.Orders.Attributedefinition;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
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

        public async Task<OrderNote> AddOrderNote(int tenantId, int? siteId, int? masterCatalogId, OrderNote orderNote, string orderId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderNoteResource = new OrderNoteResource(_apiContext);

            var newOrderNote = await orderNoteResource.CreateOrderNoteAsync(orderNote, orderId);

            return newOrderNote;
        }

        public void DeleteOrderNote(int tenantId, int? siteId, int? masterCatalogId, string noteId, string orderId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderNoteResource = new OrderNoteResource(_apiContext);

             orderNoteResource.DeleteOrderNote(orderId, noteId);
        }

        public async Task<List<OrderAttribute>> AddOrderAttribute(int tenantId, int? siteId, int? masterCatalogId, OrderAttribute orderAttribute, string orderId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderAttributeResource = new OrderAttributeResource(_apiContext);

            var attributes = new List<Mozu.Api.Contracts.CommerceRuntime.Orders.OrderAttribute>() { orderAttribute };

            var newOrderAttributes = await orderAttributeResource.CreateOrderAttributesAsync(attributes, orderId);

            return newOrderAttributes;
        }

        public async Task<List<OrderAttribute>> GetOrderAttribute(int tenantId, int? siteId, int? masterCatalogId, string orderId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderAttributeResource = new OrderAttributeResource(_apiContext);
            var newOrderAttributes = await orderAttributeResource.GetOrderAttributesAsync(orderId);

            return newOrderAttributes;
        }

        public async Task<Order> AddOrder(int tenantId, int? siteId, int? masterCatalogId, Order order)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderResource = new OrderResource(_apiContext);
            var newOrder = await orderResource.CreateOrderAsync(order);

            return newOrder;
        }

        public async Task<Order> UpdateOrder(int tenantId, int? siteId, int? masterCatalogId, Order order)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var orderResource = new OrderResource(_apiContext);
            var updatedOrder = await orderResource.UpdateOrderAsync(order, order.Id);

            return updatedOrder;
        }

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

        public async Task<Order> PerformPaymentAction(int tenantId, int? siteId, int? masterCatalogId, PaymentAction action, Payment payment, string orderId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var paymentResource = new PaymentResource(_apiContext);

            var paymentId = payment.Id;

            var available = await paymentResource.GetAvailablePaymentActionsAsync(orderId, paymentId);

            var order = await paymentResource.PerformPaymentActionAsync(action, orderId, paymentId);

            return order;
        }


    }
}
