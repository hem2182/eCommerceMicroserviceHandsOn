using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using OrderApi.Application.DTOs;
using OrderApi.Application.Mappers;
using OrderApi.Application.Repository;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderService orderService, IOrderRepository orderRepository)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            if (!orders.Any())
                return NotFound("No Orders are present in the database");

            var (_, ordersDto) = OrderMapper.FromEntity(null, orders);
            return Ok(ordersDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _orderRepository.FindByIdAsync(id);
            if (order is null)
                NotFound(null);

            var (orderDto, _) = OrderMapper.FromEntity(order, null);
            return Ok(orderDto);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetClientOrders(int clientId)
        {
            if (clientId < 0)
                return BadRequest("Invalid client Id");

            var ordersDTO = await _orderService.GetOrdersByClientId(clientId);
            return ordersDTO.Any() ? Ok(ordersDTO) : NotFound(null);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Invalid orderId.");

            var orderDetailsDTO = await _orderService.GetOrderDetails(orderId);
            return orderDetailsDTO.ClietId > 0 ? Ok(orderDetailsDTO) : NotFound("No Order Found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest($"Incomplete data. {ModelState}");

            var orderEntity = OrderMapper.ToEntity(orderDTO);
            var response = await _orderRepository.CreateAsync(orderEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            var order = OrderMapper.ToEntity(orderDTO);
            var response = await _orderRepository.UpdateAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            var order = OrderMapper.ToEntity(orderDTO);
            var response = await _orderRepository.DeleteAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
