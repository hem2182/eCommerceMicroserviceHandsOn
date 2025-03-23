using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Mappers
{
    public static class OrderMapper
    {
        public static Order ToEntity(OrderDTO orderDTO)
        {
            return new Order
            {
                Id = orderDTO.Id,
                ProductId = orderDTO.ProductId,
                ClientId = orderDTO.ClientId,
                OrderedDate = orderDTO.OrderedDate,
                PurchaseQuantity = orderDTO.PurchaseQuantity
            };
        }

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            if (order is not null && orders is null)
            {
                var orderDto = new OrderDTO(order.Id, order.ProductId, order.ClientId, order.PurchaseQuantity, order.OrderedDate);
                return (orderDto, null);
            }
            else if (order is null && orders is not null)
            {
                var ordersDto = orders.Select(o => new OrderDTO(o.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderedDate)).ToList();
                return (null, ordersDto);
            }

            return (null, null);
        }
    }
}
